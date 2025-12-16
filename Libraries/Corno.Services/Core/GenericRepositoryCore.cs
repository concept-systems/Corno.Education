using Corno.Data.Contexts;
using Corno.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Corno.Services.Core;

public class GenericRepositoryCore<TEntity> : IGenericRepositoryCore<TEntity>
    where TEntity : class
{
    #region -- Constructor --

    public GenericRepositoryCore(IUnitOfWorkCore unitOfWorkCore)
    {
        _dbContext = unitOfWorkCore.DbContext;
        _dbSet = unitOfWorkCore.DbContext.Set<TEntity>();
    }

    #endregion

    #region -- Data Members --

    private readonly CoreContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    #endregion

    #region -- Methods --

    public IQueryable<TEntity> GetAll()
    {
        return _dbSet;
    }

    /// <summary>
    ///     Get the entities by filters
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="orderBy">Order By</param>
    /// <param name="includeProperties">Include Properties</param>
    /// <returns></returns>
    public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "")
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        return orderBy?.Invoke(query) ?? query;
    }

    public virtual IQueryable<TDest> Get<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = default)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();

        // Make No tracking for partial data retrieval
        query = query.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        if (orderBy != null)
        {
            if (select == null)
                return (IQueryable<TDest>)orderBy(query.Select(d => d));

            return orderBy(query).Select(select);
        }

        if (select == null)
            return (IQueryable<TDest>)query.Select(d => d);
        return query.Select(select);
    }

    public virtual TDest FirstOrDefault<TDest>(Expression<Func<TEntity, bool>> filter = null, Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        return Get(filter, select, orderBy).FirstOrDefault();
    }

    /// <summary>
    ///     Get the count of entities on the base of conditions as predicate
    /// </summary>
    /// <param name="predicate">Predicate</param>
    /// <returns></returns>
    public int Count(Func<TEntity, bool> predicate)
    {
        return _dbSet.Where(predicate).Count();
    }

    /// <summary>
    ///     Retrieve Entity by ID
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns></returns>
    public virtual TEntity GetById(object id)
    {
        return _dbSet.Find(id);
    }

    /*public IEnumerable<TEntity> Find(Func<TEntity, bool> predicate)
    {
        return _dbSet.Where(predicate);
    }*/

    /// <summary>
    ///     Insert single entity.
    /// </summary>
    /// <param name="entity">Entity</param>
    public virtual void Add(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    /// <summary>
    ///     Bulk Insert the list of entities in the DB Set.
    /// </summary>
    /// <param name="entities">Collection of entities</param>
    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        _dbSet.AddRange(entities);
    }

    /// <summary>
    ///     Delete entity identified by primary key
    /// </summary>
    /// <param name="id">ID as primary key</param>
    public virtual void Delete(object id)
    {
        var entityToDelete = _dbSet.Find(id);
        Delete(entityToDelete);
    }

    /// <summary>
    ///     Bulk Delete the list of entities in the DB Set.
    /// </summary>
    /// <param name="entities">Collection of entities</param>
    public virtual void Delete(TEntity entityToDelete)
    {
        if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
        {
            _dbSet.Attach(entityToDelete);
        }
        _dbSet.Remove(entityToDelete);
    }

    /// <summary>
    ///     Bulk Remove the list of entities in the DB Set.
    /// </summary>
    /// <param name="entities">Collection of entities</param>
    public virtual void DeleteRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual void Update(TEntity entityToUpdate)
    {
        var entry = _dbContext.Entry(entityToUpdate);

        if (entry.State != EntityState.Detached) return;

        // Get all key properties using reflection
        var keyProperties = typeof(TEntity).GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(KeyAttribute)))
            .ToArray();

        // If no [Key] attributes are found, fallback to convention or throw
        if (!keyProperties.Any())
            throw new Exception($"GenericRepositoryCore : Update : No [Key] attributes found on {typeof(TEntity).Name}. Composite key support requires explicit [Key] attributes.");

        // Extract key values
        var keyValues = keyProperties
            .Select(p => p.GetValue(entityToUpdate))
            .ToArray();

        var set = _dbContext.Set<TEntity>();
        var attachedEntity = set.Find(keyValues);

        if (attachedEntity != null)
        {
            var attachedEntry = _dbContext.Entry(attachedEntity);
            attachedEntry.CurrentValues.SetValues(entityToUpdate);
        }
        else
        {
            entry.State = EntityState.Modified;
        }
    }


    /*public virtual void Update(TEntity entityToUpdate)
    {
        //dbSet.Attach(entityToUpdate);
        //context.Entry(entityToUpdate).State = EntityState.Modified;

        var entry = _dbContext.Entry(entityToUpdate);

        // Retreive the Id through reflection
        var pkeyById = ((_dbSet.Create().GetType().GetProperty("ID") ?? 
                         _dbSet.Create().GetType().GetProperty("Id")) ??
                        _dbSet.Create().GetType().GetProperty("Num_PK_RECORD_NO")) ??
                       _dbSet.Create().GetType().GetProperty("Num_PK_ENTRY_ID")??
                       _dbSet.Create().GetType().GetProperty("Chr_FK_PRN_NO");

        if (pkeyById == null)
        {
            var attachedEntry = _dbContext.Entry(entityToUpdate);
            attachedEntry.CurrentValues.SetValues(entityToUpdate);
            return;
            //throw new Exception("GenericRepositoryExamServer: Update : Primary key is not defined for entity.");
        }
        var pkey = pkeyById.GetValue(entityToUpdate);

        if (entry.State != EntityState.Detached) return;

        var set = _dbContext.Set<TEntity>();
        var attachedEntity = set.Find(pkey); // access the key

        if (attachedEntity != null)
        {
            var attachedEntry = _dbContext.Entry(attachedEntity);
            attachedEntry.CurrentValues.SetValues(entityToUpdate);
        }
        else
        {
            entry.State = EntityState.Modified; // attach the entity
        }
    }*/

    #endregion
}