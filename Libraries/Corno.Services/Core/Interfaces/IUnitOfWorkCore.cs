using System;
using Corno.Data.Contexts;

namespace Corno.Services.Core.Interfaces;

public interface IUnitOfWorkCore : IDisposable
{
    CoreContext DbContext { get; }

    void Save();
}