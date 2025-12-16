using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Corno.Models;


namespace Corno.DAL.Classes
{
    public class SupplierService : BaseService, ISupplierService
    {
        private IGenericRepository<Supplier > _supplierRepository;
        public IGenericRepository<Supplier> SupplierRepository
        {
            get
            {
                if (this._supplierRepository == null)
                {
                    this._supplierRepository = new GenericRepository<Supplier>(_unitOfWork);
                }
                return _supplierRepository;
            }
        }

        public SupplierService(IUnitOfWork unitOfWork,
                       IGenericRepository<Supplier> supplierRepository)
        {
            this._unitOfWork = unitOfWork;
            this._supplierRepository = supplierRepository;
        }

        /// <summary>
        /// Get All the Customers
        /// </summary>
        public IEnumerable<Supplier> GetSuppliers(
                Expression<Func<Supplier, bool>> filter = null,
                Func<IQueryable<Supplier>, IOrderedQueryable<Supplier>> orderBy = null,
                string includeProperties = "")
        {
            return SupplierRepository.Get();
        }

        public string GetAddress(int id)
        {
            Supplier supplier = _supplierRepository.GetByID(id);
            if (null != supplier)
            {
                return supplier.Address1 + ", " + supplier.CityName1 + " - " + supplier.Pin1 + ", " + supplier.StateName1;
            }

            return string.Empty;
        }
    }
}
