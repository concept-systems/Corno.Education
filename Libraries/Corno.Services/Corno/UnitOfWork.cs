using Corno.Data.Contexts;
using Corno.Logger;
using Corno.Services.Corno.Interfaces;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace Corno.Services.Corno;

public class UnitOfWork : IUnitOfWork
{

    #region -- Constructors --
    public UnitOfWork(string connectionString = "Name=DefaultConnection")
    {
        _connectionString = connectionString;
    }
    #endregion

    #region -- Data Members --
    private readonly string _connectionString;
    private CornoContext _dbContext;
    private bool _disposed;
    #endregion

    #region -- Properties --
    public CornoContext DbContext
    {
        get
        {
            if (_dbContext != null)
            {
                /*if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    _dbContext.Database.Connection.Open();*/
                return _dbContext;
            }

            _dbContext = new CornoContext(_connectionString);
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
            //_dbContext ??= new CornoContext(_connectionString);
            // Save changes to the database
            DbContext.SaveChanges();
        }
        catch (DbEntityValidationException exception)
        {
            // Loop through the validation errors to find the one causing the "string or binary data would be truncated" error
            foreach (var validationErrors in exception.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                    LogHandler.LogError(new Exception($"Property or column '{validationError.PropertyName}' caused the error: {validationError.ErrorMessage}"));
            }
            foreach (var entry in DbContext.ChangeTracker.Entries())
                entry.State = EntityState.Detached;

            throw;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            foreach (var entry in DbContext.ChangeTracker.Entries())
                entry.State = EntityState.Detached;
            throw;
        }
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