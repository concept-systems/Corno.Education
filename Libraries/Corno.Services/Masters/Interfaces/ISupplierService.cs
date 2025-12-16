using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Corno.Models;

namespace Corno.DAL.Classes
{
    public interface ISupplierService : IBaseService
    {
        IGenericRepository<Supplier > SupplierRepository { get; }

        #region -- Methods --
        void Save();

        IEnumerable<Supplier> GetSuppliers(
            Expression<Func<Supplier, bool>> filter = null,
            Func<IQueryable<Supplier>, IOrderedQueryable<Supplier>> orderBy = null,
            string includeProperties = "");

        string GetAddress(int id);
        #endregion
    }
}
