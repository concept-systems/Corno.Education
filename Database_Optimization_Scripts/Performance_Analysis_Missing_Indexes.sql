-- =============================================
-- Performance Analysis and Missing Index Detection Script
-- Purpose: Identify missing indexes and analyze query performance
-- Databases: Corno.Bharati.OnlineExam, BHVEDPSNET
-- =============================================

-- =============================================
-- SECTION 1: MISSING INDEX RECOMMENDATIONS
-- SQL Server tracks missing indexes based on query execution
-- =============================================

PRINT 'Analyzing missing indexes for Corno.Bharati.OnlineExam...'
GO

USE [Corno.Bharati.OnlineExam]
GO

SELECT 
    migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) AS improvement_measure,
    'CREATE INDEX [IX_' + OBJECT_NAME(mid.object_id) + '_' + 
    REPLACE(REPLACE(REPLACE(ISNULL(mid.equality_columns,''),', ','_'),'[',''),']','') +
    CASE 
        WHEN mid.equality_columns IS NOT NULL AND mid.inequality_columns IS NOT NULL THEN '_'
        ELSE ''
    END +
    REPLACE(REPLACE(REPLACE(ISNULL(mid.inequality_columns,''),', ','_'),'[',''),']','') +
    '] ON [' + OBJECT_SCHEMA_NAME(mid.object_id) + '].[' + OBJECT_NAME(mid.object_id) + ']' +
    ' (' + ISNULL(mid.equality_columns,'') +
    CASE 
        WHEN mid.equality_columns IS NOT NULL AND mid.inequality_columns IS NOT NULL THEN ','
        ELSE ''
    END +
    ISNULL(mid.inequality_columns, '') + ')' +
    ISNULL(' INCLUDE (' + mid.included_columns + ')', '') AS create_index_statement,
    migs.user_seeks,
    migs.user_scans,
    migs.avg_total_user_cost,
    migs.avg_user_impact,
    migs.last_user_seek,
    migs.last_user_scan,
    OBJECT_NAME(mid.object_id) AS TableName
FROM sys.dm_db_missing_index_groups mig
INNER JOIN sys.dm_db_missing_index_group_stats migs ON migs.group_handle = mig.index_group_handle
INNER JOIN sys.dm_db_missing_index_details mid ON mig.index_handle = mid.index_handle
WHERE migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) > 10
ORDER BY improvement_measure DESC
GO

PRINT 'Analyzing missing indexes for BHVEDPSNET...'
GO

USE [BHVEDPSNET]
GO

SELECT 
    migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) AS improvement_measure,
    'CREATE INDEX [IX_' + OBJECT_NAME(mid.object_id) + '_' + 
    REPLACE(REPLACE(REPLACE(ISNULL(mid.equality_columns,''),', ','_'),'[',''),']','') +
    CASE 
        WHEN mid.equality_columns IS NOT NULL AND mid.inequality_columns IS NOT NULL THEN '_'
        ELSE ''
    END +
    REPLACE(REPLACE(REPLACE(ISNULL(mid.inequality_columns,''),', ','_'),'[',''),']','') +
    '] ON [' + OBJECT_SCHEMA_NAME(mid.object_id) + '].[' + OBJECT_NAME(mid.object_id) + ']' +
    ' (' + ISNULL(mid.equality_columns,'') +
    CASE 
        WHEN mid.equality_columns IS NOT NULL AND mid.inequality_columns IS NOT NULL THEN ','
        ELSE ''
    END +
    ISNULL(mid.inequality_columns, '') + ')' +
    ISNULL(' INCLUDE (' + mid.included_columns + ')', '') AS create_index_statement,
    migs.user_seeks,
    migs.user_scans,
    migs.avg_total_user_cost,
    migs.avg_user_impact,
    migs.last_user_seek,
    migs.last_user_scan,
    OBJECT_NAME(mid.object_id) AS TableName
FROM sys.dm_db_missing_index_groups mig
INNER JOIN sys.dm_db_missing_index_group_stats migs ON migs.group_handle = mig.index_group_handle
INNER JOIN sys.dm_db_missing_index_details mid ON mig.index_handle = mid.index_handle
WHERE migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) > 10
ORDER BY improvement_measure DESC
GO

-- =============================================
-- SECTION 2: INDEX USAGE STATISTICS
-- Identify unused or rarely used indexes
-- =============================================

PRINT 'Analyzing index usage statistics for Corno.Bharati.OnlineExam...'
GO

USE [Corno.Bharati.OnlineExam]
GO

SELECT 
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    s.user_seeks,
    s.user_scans,
    s.user_lookups,
    s.user_updates,
    s.last_user_seek,
    s.last_user_scan,
    s.last_user_lookup,
    s.last_user_update,
    CASE 
        WHEN s.user_seeks + s.user_scans + s.user_lookups = 0 THEN 'UNUSED'
        WHEN s.user_seeks + s.user_scans + s.user_lookups < s.user_updates THEN 'UNDERUSED'
        ELSE 'ACTIVE'
    END AS UsageStatus
FROM sys.indexes i
LEFT JOIN sys.dm_db_index_usage_stats s ON i.object_id = s.object_id AND i.index_id = s.index_id AND s.database_id = DB_ID()
WHERE i.object_id > 100  -- Exclude system objects
    AND i.name IS NOT NULL
    AND i.is_primary_key = 0
    AND i.is_unique_constraint = 0
ORDER BY s.user_seeks + s.user_scans + s.user_lookups ASC, TableName, IndexName
GO

PRINT 'Analyzing index usage statistics for BHVEDPSNET...'
GO

USE [BHVEDPSNET]
GO

SELECT 
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    s.user_seeks,
    s.user_scans,
    s.user_lookups,
    s.user_updates,
    s.last_user_seek,
    s.last_user_scan,
    s.last_user_lookup,
    s.last_user_update,
    CASE 
        WHEN s.user_seeks + s.user_scans + s.user_lookups = 0 THEN 'UNUSED'
        WHEN s.user_seeks + s.user_scans + s.user_lookups < s.user_updates THEN 'UNDERUSED'
        ELSE 'ACTIVE'
    END AS UsageStatus
FROM sys.indexes i
LEFT JOIN sys.dm_db_index_usage_stats s ON i.object_id = s.object_id AND i.index_id = s.index_id AND s.database_id = DB_ID()
WHERE i.object_id > 100
    AND i.name IS NOT NULL
    AND i.is_primary_key = 0
    AND i.is_unique_constraint = 0
ORDER BY s.user_seeks + s.user_scans + s.user_lookups ASC, TableName, IndexName
GO

-- =============================================
-- SECTION 3: INDEX FRAGMENTATION ANALYSIS
-- =============================================

PRINT 'Analyzing index fragmentation for Corno.Bharati.OnlineExam...'
GO

USE [Corno.Bharati.OnlineExam]
GO

SELECT 
    OBJECT_NAME(ips.object_id) AS TableName,
    i.name AS IndexName,
    ips.index_type_desc AS IndexType,
    ips.avg_fragmentation_in_percent AS FragmentationPercent,
    ips.page_count AS PageCount,
    CASE 
        WHEN ips.avg_fragmentation_in_percent > 30 THEN 'REBUILD REQUIRED'
        WHEN ips.avg_fragmentation_in_percent > 10 THEN 'REORGANIZE RECOMMENDED'
        ELSE 'OK'
    END AS MaintenanceAction
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
INNER JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE ips.avg_fragmentation_in_percent > 5
    AND ips.page_count > 100
    AND i.name IS NOT NULL
ORDER BY ips.avg_fragmentation_in_percent DESC, ips.page_count DESC
GO

PRINT 'Analyzing index fragmentation for BHVEDPSNET...'
GO

USE [BHVEDPSNET]
GO

SELECT 
    OBJECT_NAME(ips.object_id) AS TableName,
    i.name AS IndexName,
    ips.index_type_desc AS IndexType,
    ips.avg_fragmentation_in_percent AS FragmentationPercent,
    ips.page_count AS PageCount,
    CASE 
        WHEN ips.avg_fragmentation_in_percent > 30 THEN 'REBUILD REQUIRED'
        WHEN ips.avg_fragmentation_in_percent > 10 THEN 'REORGANIZE RECOMMENDED'
        ELSE 'OK'
    END AS MaintenanceAction
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
INNER JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE ips.avg_fragmentation_in_percent > 5
    AND ips.page_count > 100
    AND i.name IS NOT NULL
ORDER BY ips.avg_fragmentation_in_percent DESC, ips.page_count DESC
GO

-- =============================================
-- SECTION 4: TOP QUERIES BY EXECUTION TIME
-- =============================================

PRINT 'Analyzing top queries by execution time for Corno.Bharati.OnlineExam...'
GO

USE [Corno.Bharati.OnlineExam]
GO

SELECT TOP 20
    qs.execution_count,
    qs.total_elapsed_time / 1000000.0 AS total_elapsed_time_seconds,
    qs.avg_elapsed_time / 1000000.0 AS avg_elapsed_time_seconds,
    qs.total_logical_reads,
    qs.total_logical_reads / qs.execution_count AS avg_logical_reads,
    qs.total_physical_reads,
    qs.total_physical_reads / qs.execution_count AS avg_physical_reads,
    SUBSTRING(qt.text, (qs.statement_start_offset/2)+1, 
        ((CASE qs.statement_end_offset
            WHEN -1 THEN DATALENGTH(qt.text)
            ELSE qs.statement_end_offset
        END - qs.statement_start_offset)/2) + 1) AS statement_text,
    qt.text AS full_query_text,
    qp.query_plan
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
CROSS APPLY sys.dm_exec_query_plan(qs.plan_handle) qp
WHERE qt.dbid = DB_ID()
ORDER BY qs.total_elapsed_time DESC
GO

PRINT 'Analyzing top queries by execution time for BHVEDPSNET...'
GO

USE [BHVEDPSNET]
GO

SELECT TOP 20
    qs.execution_count,
    qs.total_elapsed_time / 1000000.0 AS total_elapsed_time_seconds,
    qs.avg_elapsed_time / 1000000.0 AS avg_elapsed_time_seconds,
    qs.total_logical_reads,
    qs.total_logical_reads / qs.execution_count AS avg_logical_reads,
    qs.total_physical_reads,
    qs.total_physical_reads / qs.execution_count AS avg_physical_reads,
    SUBSTRING(qt.text, (qs.statement_start_offset/2)+1, 
        ((CASE qs.statement_end_offset
            WHEN -1 THEN DATALENGTH(qt.text)
            ELSE qs.statement_end_offset
        END - qs.statement_start_offset)/2) + 1) AS statement_text,
    qt.text AS full_query_text,
    qp.query_plan
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
CROSS APPLY sys.dm_exec_query_plan(qs.plan_handle) qp
WHERE qt.dbid = DB_ID()
ORDER BY qs.total_elapsed_time DESC
GO

-- =============================================
-- SECTION 5: TABLE SIZE ANALYSIS
-- =============================================

PRINT 'Analyzing table sizes for Corno.Bharati.OnlineExam...'
GO

USE [Corno.Bharati.OnlineExam]
GO

SELECT 
    t.NAME AS TableName,
    s.Name AS SchemaName,
    p.rows AS RowCounts,
    SUM(a.total_pages) * 8 AS TotalSpaceKB,
    SUM(a.used_pages) * 8 AS UsedSpaceKB,
    (SUM(a.total_pages) - SUM(a.used_pages)) * 8 AS UnusedSpaceKB
FROM sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
LEFT OUTER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE t.NAME NOT LIKE 'dt%' 
    AND t.is_ms_shipped = 0
    AND i.OBJECT_ID > 255
GROUP BY t.Name, s.Name, p.Rows
ORDER BY TotalSpaceKB DESC
GO

PRINT 'Analyzing table sizes for BHVEDPSNET...'
GO

USE [BHVEDPSNET]
GO

SELECT 
    t.NAME AS TableName,
    s.Name AS SchemaName,
    p.rows AS RowCounts,
    SUM(a.total_pages) * 8 AS TotalSpaceKB,
    SUM(a.used_pages) * 8 AS UsedSpaceKB,
    (SUM(a.total_pages) - SUM(a.used_pages)) * 8 AS UnusedSpaceKB
FROM sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
LEFT OUTER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE t.NAME NOT LIKE 'dt%' 
    AND t.is_ms_shipped = 0
    AND i.OBJECT_ID > 255
GROUP BY t.Name, s.Name, p.Rows
ORDER BY TotalSpaceKB DESC
GO

-- =============================================
-- SECTION 6: STATISTICS INFORMATION
-- =============================================

PRINT 'Analyzing statistics for Corno.Bharati.OnlineExam...'
GO

USE [Corno.Bharati.OnlineExam]
GO

SELECT 
    OBJECT_NAME(s.object_id) AS TableName,
    s.name AS StatisticsName,
    s.auto_created,
    s.user_created,
    s.no_recompute,
    STATS_DATE(s.object_id, s.stats_id) AS LastUpdated,
    sp.rows,
    sp.rows_sampled,
    sp.modification_counter
FROM sys.stats s
CROSS APPLY sys.dm_db_stats_properties(s.object_id, s.stats_id) sp
WHERE OBJECT_NAME(s.object_id) IS NOT NULL
    AND sp.modification_counter > 1000
ORDER BY sp.modification_counter DESC
GO

PRINT 'Performance analysis completed!'
GO






