using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Common.Interfaces;

public interface IAmountInWordsService : IBaseService
{
    string GetAmountInWords(string amount);
}