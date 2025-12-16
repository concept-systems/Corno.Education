using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using OnlineExam.Models;
using Corno.Unity;

namespace Corno.Services
{
    public class CityService : MasterService<City>,ICityService
    {
        public CityService(IUnitOfWork unitOfWork, IGenericRepository<City> cityRepository)
            : base(unitOfWork, cityRepository)
        {
        }
    }
}
