using Corno.Services.Corno.Interfaces;

namespace Corno.Education.Areas.Api.Services.Interfaces;

public interface IBaseApiService : IService
{
    object Get(string action, object value, ApiName api);
    object Post(string action, object value, ApiName api);
}