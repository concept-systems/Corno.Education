namespace Corno.Services.Corno.Interfaces;

public interface IBaseService : IService
{
    void Save();

    void Dispose(bool disposing);
}