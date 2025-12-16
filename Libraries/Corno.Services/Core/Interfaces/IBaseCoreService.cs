using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Core.Interfaces;

public interface IBaseCoreService : IService
{
    void Save();

    void Dispose(bool disposing);
}