using OnlineExam.Models;

namespace Corno.Services
{
    public class CountryService : MasterService<Country>,ICountryService
    {
        public CountryService(IUnitOfWork unitOfWork, IGenericRepository<Country> countryRepository)
            : base(unitOfWork, countryRepository)
        {
        }
    }
}
