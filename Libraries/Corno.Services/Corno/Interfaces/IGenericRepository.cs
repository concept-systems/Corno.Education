using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Corno.Services.Corno.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
    void SetIncludes(string includes);
    bool HasIncludes();

    void RefreshDatabase(TEntity entity);

    IQueryable<TEntity> GetAll();

    IQueryable<TEntity> GetQuery();
    IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");

    IQueryable<TDest> Get<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = default);

    /*TDest FirstOrDefault<TDest>(Expression<Func<TEntity, bool>> filter = null, Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);*/

    TEntity GetById(object id);

    int Count(Func<TEntity, bool> predicate);

    //IEnumerable<TEntity> Find(Func<TEntity, bool> predicate);


    void Add(TEntity entity);

    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);

    void Delete(object id);

    void Delete(TEntity entityToDelete);
    void Save();
}