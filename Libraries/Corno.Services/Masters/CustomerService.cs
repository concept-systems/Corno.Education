using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Corno.Models;


namespace Corno.DAL.Classes
{
    public class CustomerService : BaseService, ICustomerService
    {
        private IGenericRepository<Customer> _customerRepository;
        public IGenericRepository<Customer> CustomerRepository
        {
            get
            {
                if (this._customerRepository == null)
                {
                    this._customerRepository = new GenericRepository<Customer>(_unitOfWork);
                }
                return _customerRepository;
            }
        }

        //private IGenericRepository<Zone> _zoneRepository;
        //public IGenericRepository<Zone> ZoneRepository
        //{
        //    get
        //    {
        //        if (this._zoneRepository == null)
        //        {
        //            this._zoneRepository = new GenericRepository<Zone>(_unitOfWork);
        //        }
        //        return _zoneRepository;
        //    }
        //}

        //private IGenericRepository<Country> _countryRepository;
        //public IGenericRepository<Country> CountryRepository
        //{
        //    get
        //    {
        //        if (this._countryRepository == null)
        //        {
        //            this._countryRepository = new GenericRepository<Country>(_unitOfWork);
        //        }
        //        return _countryRepository;
        //    }
        //}

        public CustomerService(IUnitOfWork unitOfWork,
                       IGenericRepository<Customer> customerRepository)
        {
            this._unitOfWork = unitOfWork;
            this._customerRepository = customerRepository;
        }

        /// <summary>
        /// Get All the Customers
        /// </summary>
        public IEnumerable<Customer> GetCustomers(
                Expression<Func<Customer, bool>> filter = null,
                Func<IQueryable<Customer>, IOrderedQueryable<Customer>> orderBy = null,
                string includeProperties = "")
        {
            return CustomerRepository.Get();
        }


        //public IEnumerable<Zone> GetZone(
        //        Expression<Func<Zone, bool>> filter = null,
        //        Func<IQueryable<Zone>, IOrderedQueryable<Zone>> orderBy = null,
        //        string includeProperties = "")
        //{
        //    return ZoneRepository.Get();
        //}

       //public IEnumerable<Country> GetCountry(
       //        Expression<Func<Country, bool>> filter = null,
       //        Func<IQueryable<Country>, IOrderedQueryable<Country>> orderBy = null,
       //        string includeProperties = "")
       // {
       //     return CountryRepository.Get();
       // }

        public string GetAddress(int id)
        {
            Customer customer = _customerRepository.GetByID(id);
            if (null != customer)
            {
                return customer.Address1 + ", " + customer.CityName1 + " - " + customer.Pin1 + ", " + customer.StateName1;
            }

            return string.Empty;
        }
    }
}
