using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;

namespace Corno.Services.Helper;

public static class TruncationInspector
{
    private class ColumnLimit
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; } // varchar, nvarchar, char, nchar, varbinary, binary
        public int? MaxChars { get; set; }   // INFORMATION_SCHEMA gives characters (-1 for MAX)
    }

    private class StoreInfo
    {
        public string Schema { get; set; }
        public string Table { get; set; }
        public Dictionary<string, string> PropertyToColumn { get; set; } // propName -> colName
    }

    /// <summary>
    /// Call once per SaveChanges to validate all pending entities (Added/Modified).
    /// Throws InvalidOperationException listing the offending fields if any.
    /// </summary>
    public static void ValidatePendingChanges(DbContext context)
    {
        var issues = new List<string>();

        // Group entries by CLR type so we can reuse mapping and limits
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .ToList();

        var storeInfoCache = new Dictionary<Type, StoreInfo>();
        var limitsCache = new Dictionary<string, Dictionary<string, ColumnLimit>>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in entries)
        {
            var entityType = entry.Entity.GetType();

            if (!storeInfoCache.TryGetValue(entityType, out var storeInfo))
            {
                storeInfo = GetStoreInfoForType(context, entityType);
                storeInfoCache[entityType] = storeInfo;
            }

            // Cache key per table
            var tableKey = $"{storeInfo.Schema}.{storeInfo.Table}";
            if (!limitsCache.TryGetValue(tableKey, out var colLimits))
            {
                colLimits = GetStringAndBinaryColumnLimits(context, storeInfo.Schema, storeInfo.Table);
                limitsCache[tableKey] = colLimits;
            }

            // Validate string and byte[] properties
            foreach (var prop in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;
                if (!storeInfo.PropertyToColumn.TryGetValue(prop.Name, out var colName)) continue;
                if (!colLimits.TryGetValue(colName, out var limit)) continue; // not a string/binary column

                var value = prop.GetValue(entry.Entity);
                if (value == null) continue;

                // Strings: compare length in characters; binaries: compare byte length
                if (value is string s)
                {
                    if (limit.MaxChars.HasValue && limit.MaxChars.Value > 0 && s.Length > limit.MaxChars.Value)
                    {
                        issues.Add($"{entityType.Name}.{prop.Name} (column [{colName}]): length {s.Length} > max {limit.MaxChars.Value}");
                    }
                }
                else if (value is byte[] bytes)
                {
                    // INFORMATION_SCHEMA reports MaxChars for binary as bytes
                    if (limit.MaxChars.HasValue && limit.MaxChars.Value > 0 && bytes.Length > limit.MaxChars.Value)
                    {
                        issues.Add($"{entityType.Name}.{prop.Name} (column [{colName}]): bytes {bytes.Length} > max {limit.MaxChars.Value}");
                    }
                }
            }
        }

        if (issues.Count > 0)
        {
            var message = "String/binary length overflow detected before SaveChanges:\n" +
                          string.Join("\n", issues);
            throw new InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Validate a single entity instance against its mapped table.
    /// Returns a list of human-readable issues (empty if OK).
    /// </summary>
    public static List<string> ValidateEntity(DbContext context, object entity)
    {
        var issues = new List<string>();
        var entityType = entity.GetType();

        var storeInfo = GetStoreInfoForType(context, entityType);
        var colLimits = GetStringAndBinaryColumnLimits(context, storeInfo.Schema, storeInfo.Table);

        foreach (var prop in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead) continue;
            if (!storeInfo.PropertyToColumn.TryGetValue(prop.Name, out var colName)) continue;
            if (!colLimits.TryGetValue(colName, out var limit)) continue;

            var value = prop.GetValue(entity);
            if (value == null) continue;

            if (value is string s)
            {
                if (limit.MaxChars.HasValue && limit.MaxChars.Value > 0 && s.Length > limit.MaxChars.Value)
                {
                    issues.Add($"{entityType.Name}.{prop.Name} (column [{colName}]): length {s.Length} > max {limit.MaxChars.Value}");
                }
            }
            else if (value is byte[] bytes)
            {
                if (limit.MaxChars.HasValue && limit.MaxChars.Value > 0 && bytes.Length > limit.MaxChars.Value)
                {
                    issues.Add($"{entityType.Name}.{prop.Name} (column [{colName}]): bytes {bytes.Length} > max {limit.MaxChars.Value}");
                }
            }
        }

        return issues;
    }

    // ---------- internals ----------

    // Read schema/table and property->column mapping from EF6 metadata
    private static StoreInfo GetStoreInfoForType(DbContext context, Type clrType)
    {
        var octx = ((IObjectContextAdapter)context).ObjectContext;

        // Create ObjectSet to get the C-Space entity set
        dynamic objectSet = octx.GetType()
            .GetMethod("CreateObjectSet", Type.EmptyTypes)
            .MakeGenericMethod(clrType)
            .Invoke(octx, null);

        var entitySet = (EntitySet)objectSet.EntitySet;

        // CSSpace mapping (Conceptual <-> Storage)
        var containerMapping = octx.MetadataWorkspace
            .GetItems<EntityContainerMapping>(DataSpace.CSSpace)
            .Single();

        var setMapping = containerMapping.EntitySetMappings
            .Single(m => m.EntitySet == entitySet);

        // Get first type mapping fragment (covers TPH as well)
        var fragment = setMapping
            .EntityTypeMappings.First()
            .Fragments.First();

        // Table and schema
        var storeSet = fragment.StoreEntitySet;
        var schema = (string)(storeSet.MetadataProperties["Schema"]?.Value ?? "dbo");
        var table = (string)(storeSet.MetadataProperties["Table"]?.Value ?? storeSet.Name);

        // Property -> Column
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pm in fragment.PropertyMappings.OfType<ScalarPropertyMapping>())
        {
            map[pm.Property.Name] = pm.Column.Name;
        }

        return new StoreInfo
        {
            Schema = schema,
            Table = table,
            PropertyToColumn = map
        };
    }

    // Pull string and binary column limits from INFORMATION_SCHEMA (characters for text, bytes for binary)
    private static Dictionary<string, ColumnLimit> GetStringAndBinaryColumnLimits(DbContext context, string schema, string table)
    {
        const string sql = @"
SELECT 
    COLUMN_NAME      AS ColumnName,
    DATA_TYPE        AS DataType,
    CHARACTER_MAXIMUM_LENGTH AS MaxChars
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = @p0 AND TABLE_NAME = @p1
  AND DATA_TYPE IN ('varchar','nvarchar','char','nchar','varbinary','binary');
";

        var rows = context.Database.SqlQuery<ColumnLimit>(sql, schema, table).ToList();

        // Normalize: -1 (MAX) => null (treat as unlimited)
        foreach (var r in rows)
        {
            if (r.MaxChars.HasValue && r.MaxChars.Value < 0)
                r.MaxChars = null;
        }

        return rows.ToDictionary(r => r.ColumnName, r => r, StringComparer.OrdinalIgnoreCase);
    }
}