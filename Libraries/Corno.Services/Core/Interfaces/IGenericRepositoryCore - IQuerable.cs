using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Corno.Services.Core.Interfaces;

public interface IGenericRepositoryCore<TEntity> where TEntity : class
{
    IQueryable<TEntity> GetAll();

    IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");

    IQueryable<TDest> Get<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = default);
    TDest FirstOrDefault<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

    TEntity GetById(object id);

    int Count(Func<TEntity, bool> predicate);

    IEnumerable<TEntity> Find(Func<TEntity, bool> predicate);

    void Add(TEntity entity);

    void AddRange(IEnumerable<TEntity> entities);

    void Delete(object id);

    void Delete(TEntity entityToDelete);

    //// <summary>
    ///     Bulk Remove the list of entities in the DB Set.
    /// </summary>
    /// <param name="entities">Collection of entities</param>     void DeleteRange(IEnumerable<TEntity> entities);

    void Update(TEntity entity);
}