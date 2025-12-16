using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Corno.Models;

namespace Corno.DAL.Classes
{
    public interface ICustomerService : IBaseService
    {
        IGenericRepository<Customer> CustomerRepository { get; }
       // IGenericRepository<Zone> ZoneRepository { get; }
       // IGenericRepository<Country> CountryRepository { get; }

        #region -- Methods --
        void Save();

        IEnumerable<Customer> GetCustomers(
            Expression<Func<Customer, bool>> filter = null,
            Func<IQueryable<Customer>, IOrderedQueryable<Customer>> orderBy = null,
            string includeProperties = "");

        //IEnumerable<Country> GetCountry(
        //    Expression<Func<Country, bool>> filter = null,
        //    Func<IQueryable<Country>, IOrderedQueryable<Country>> orderBy = null,
        //    string includeProperties = "");

       //IEnumerable<Zone> GetZone(
       //    Expression<Func<Zone, bool>> filter = null,
       //    Func<IQueryable<Zone>, IOrderedQueryable<Zone>> orderBy = null,
       //    string includeProperties = "");

        string GetAddress(int id);
        #endregion
    }
}
