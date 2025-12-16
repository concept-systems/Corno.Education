using System.Data.Entity;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno;

public class BaseService : IBaseService
{
    #region -- Data Mebers --
    protected IUnitOfWork UnitOfWork;
    //protected IUnitOfWorkCore UnitOfWorkExamServer;
    #endregion

    #region -- Public Methods --
    public void Save()
    {
        UnitOfWork.Save();
    }

    public DbContextTransaction BeginTransaction()
    {
        return UnitOfWork.DbContext.Database.BeginTransaction();
    }
    #endregion

    public void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnitOfWork.Dispose();
        }
    }
}