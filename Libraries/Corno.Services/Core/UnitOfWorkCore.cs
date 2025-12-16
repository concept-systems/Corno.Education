using Corno.Data.Contexts;
using Corno.Logger;
using Corno.Services.Core.Interfaces;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Corno.Services.Helper;

namespace Corno.Services.Core;

public class UnitOfWorkCore : IUnitOfWorkCore
{
    #region -- Constructors --
    public UnitOfWorkCore(string connectionString = "Name=CoreContext")
    {
        ConnectionString = connectionString;
    }
    #endregion

    #region -- Data Members --
    protected string ConnectionString;
    private CoreContext _dbContext;

    private bool _disposed;
    #endregion

    #region -- Properties --
    public CoreContext DbContext
    {
        get
        {
            if (_dbContext != null)
            {
                /*if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    _dbContext.Database.Connection.Open();*/
                return _dbContext;
            }

            _dbContext = new CoreContext();
            _dbContext.Configuration.LazyLoadingEnabled = false;
            return _dbContext;
        }
    }
    #endregion

    #region -- Methods --
    public void Save()
    {
        try
        {
            TruncationInspector.ValidatePendingChanges(_dbContext); // throws with detailed fields if any

            // Save changes to the database
            _dbContext.SaveChanges();
        }
        catch (DbEntityValidationException exception)
        {
            // Loop through the validation errors to find the one causing the "string or binary data would be truncated" error
            foreach (var validationErrors in exception.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                    LogHandler.LogError(new Exception($"Property or column '{validationError.PropertyName}' caused the error: {validationError.ErrorMessage}"));
            }
            foreach (var entry in _dbContext.ChangeTracker.Entries())
                entry.State = EntityState.Detached;

            throw;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            foreach (var entry in _dbContext.ChangeTracker.Entries())
                entry.State = EntityState.Detached;
            throw;
        }
        /*catch (DbEntityValidationException ex)
        {
            // Loop through the validation errors to find the one causing the "string or binary data would be truncated" error
            foreach (var validationErrors in ex.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    LogHandler.LogError(new Exception($"Property or column '{validationError.PropertyName}' caused the error: {validationError.ErrorMessage}"));
                }
            }

            throw;
        }
        catch (DbUpdateException exception)
        {
            foreach (var entry in exception.Entries)
            {
                var entityType = entry.Entity.GetType().Name;
                var keyNames = entry.Entity.GetType()
                    .GetProperties()
                    .Where(p => Attribute.IsDefined(p, typeof(KeyAttribute)))
                    .Select(p => p.Name);

                var keyValues = keyNames.ToDictionary(
                    name => name,
                    name => entry.Entity.GetType().GetProperty(name)?.GetValue(entry.Entity, null)
                );

                // Log entity type and key values
                var newException = new Exception($"Error updating entity {entityType} with keys: {string.Join(", ", keyValues.Select(kv => $"{kv.Key}={kv.Value}"))}");
                LogHandler.LogError(newException);
            }

            throw;
        }*/
        /*catch (DbUpdateException ex)
        {
            // Check if it's the "string or binary data would be truncated" error
            if (ex.InnerException is SqlException { Number: 8152 })
                // Handle the error appropriately, log or throw a custom exception
                LogHandler.LogError(new Exception("String or binary data would be truncated. Check column lengths."));
            else
                // Handle other types of DbUpdateException
                LogHandler.LogError(new Exception("An error occurred while saving changes to the database: " + ex.Message));
            throw;
        }*/
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _dbContext?.Dispose();  // <-- returns the connection to the pool
            _dbContext = null;
        }
        _disposed = true;
    }
    #endregion
}