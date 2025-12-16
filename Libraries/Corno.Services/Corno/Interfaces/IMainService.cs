using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System;

namespace Corno.Services.Corno.Interfaces;

public interface IMainService<TEntity> : IBaseService
where TEntity : class
{
    void SetIncludes(string includes);

    IQueryable<TEntity> GetQuery();
    TEntity GetById(object id);

    IQueryable<TDest> Get<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

    TDest FirstOrDefault<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

    TDest LastOrDefault<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null);

    void Add(TEntity entity);
    void AddAndSave(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void AddRangeAndSave(IEnumerable<TEntity> entities);
    void Update(TEntity entityToUpdate);
    void UpdateAndSave(TEntity entityToUpdate);
    void UpdateRange(IEnumerable<TEntity> entities);
    void UpdateRangeAndSave(IEnumerable<TEntity> entities);

    void Delete(TEntity entityToUpdate);
}