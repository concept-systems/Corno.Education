using System;
using Corno.Data.Contexts;

namespace Corno.Services.Corno.Interfaces;

public interface IUnitOfWork : IDisposable
{
    CornoContext DbContext { get; }

    void Save();
}