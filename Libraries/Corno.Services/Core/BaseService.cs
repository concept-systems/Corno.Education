using System.Data.Entity;
using Corno.Services.Core.Interfaces;

namespace Corno.Services.Core;

public class BaseCoreService : IBaseCoreService
{
    #region -- Data Mebers --
    protected IUnitOfWorkCore UnitOfWorkCore;
    #endregion

    #region -- Public Methods --
    public void Save()
    {
        UnitOfWorkCore.Save();
    }

    public DbContextTransaction BeginTransaction()
    {
        return UnitOfWorkCore.DbContext.Database.BeginTransaction();
    }
    #endregion

    public void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnitOfWorkCore.Dispose();
        }
    }
}