using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Corno.Data.Common;
using Corno.Data.Contexts;
using Corno.Logger;
using Corno.Services.Corno.Interfaces;
using Mapster;

namespace Corno.Services.Corno;

public class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : class
{
    #region -- Constructor --

    public GenericRepository(IUnitOfWork unitOfWork, string includeProperties = default)
    {
        _dbContext = unitOfWork.DbContext;
        _dbSet = unitOfWork.DbContext.Set<TEntity>();

        _includeProperties = includeProperties ?? string.Empty;
    }

    #endregion

    #region -- Data Members --
    private readonly CornoContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    private string _includeProperties;
    #endregion

    #region -- Public Methods --
    public void SetIncludes(string includes)
    {
        _includeProperties = includes;
    }

    public bool HasIncludes()
    {
        return !string.IsNullOrEmpty(_includeProperties);
    }

    public void RefreshDatabase(TEntity entity)
    {
        ((IObjectContextAdapter)_dbContext)
            .ObjectContext
            .Refresh(RefreshMode.StoreWins, entity);
        _dbContext.Entry(entity).Reload();
    }

    public IQueryable<TEntity> GetAll()
    {
        return _dbSet;
    }

    public virtual IQueryable<TEntity> GetQuery()
    {
        return _dbSet.AsQueryable();
    }

    public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = default,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = default, string includeProperties = default)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();

        if (filter != default)
            query = query.Where(filter);

        if (!string.IsNullOrEmpty(includeProperties))
            _includeProperties = includeProperties;

        query = _includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        return orderBy != default ? orderBy(query) : query;
    }

    public virtual IQueryable<TDest> Get<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = default)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        if (!string.IsNullOrEmpty(_includeProperties))
            query = _includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

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

    /*public virtual TDest FirstOrDefault<TDest>(Expression<Func<TEntity, bool>> filter = null, Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        if (null == select)
            throw new Exception("Select cannot by null");

        IQueryable<TEntity> query = _dbSet.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        if (!string.IsNullOrEmpty(_includeProperties))
            query = _includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        if (null != orderBy)
            query = orderBy(query);

        var entity = query.Select(select).FirstOrDefault();
        return entity;
    }*/


    public virtual TEntity GetById(object id)
    {
        var entity = _dbSet.Find(id);
        if (string.IsNullOrEmpty(_includeProperties)) 
            return entity;

        var includes = _includeProperties.Split(',');
        foreach (var include in includes)
            _dbContext.Entry(entity).Collection(include.Trim()).Load();

        return entity;
        //return _dbSet.Find(id);
    }

    public IEnumerable<TEntity> Find(Func<TEntity, bool> predicate)
    {
        return _dbSet.Where(predicate);
    }

    public int Count(Func<TEntity, bool> predicate)
    {
        return _dbSet.Where(predicate).Count();
    }

    public virtual void Add(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        _dbSet.AddRange(entities);
    }

    /*public virtual void Update(TEntity entityToUpdate)
    {
        var entry = _dbContext.Entry(entityToUpdate);

        var pKeyById = _dbSet.Create().GetType().GetProperty("ID") ??
                       _dbSet.Create().GetType().GetProperty("Id");

        var key = pKeyById?.GetValue(entityToUpdate);

        if (entry.State != EntityState.Detached) return;

        var set = _dbContext.Set<TEntity>();
        var attachedEntity = set.Find(key);

        if (attachedEntity != null)
        {
            if (entityToUpdate is IHasChildCollections)
            {
                dynamic dynamicEntity = entityToUpdate;
                _dbContext.UpdateGraph(dynamicEntity, map =>
                {
                    dynamicEntity.ConfigureGraphMapping(map);
                });
            }
            else
            {
                var attachedEntry = _dbContext.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entityToUpdate);

                if (!string.IsNullOrEmpty(_includeProperties))
                {
                    var includes = _includeProperties.Split(',');
                    foreach (var include in includes)
                        attachedEntry.Collection(include.Trim()).Load();
                }

                var isUpdated = (attachedEntry.Entity as BaseModel)?.UpdateDetails(entityToUpdate as BaseModel);
                if (isUpdated == false)
                    entityToUpdate.Adapt(attachedEntry.Entity);
            }
        }
        else
        {
            entry.State = EntityState.Modified;
        }
    }*/


    public virtual void Update(TEntity entityToUpdate)
    {
        //dbSet.Attach(entityToUpdate);
        //context.Entry(entityToUpdate).State = EntityState.Modified;

        var entry = _dbContext.Entry(entityToUpdate);

        // Retrieve the Id through reflection
        var pKeyById = _dbSet.Create().GetType().GetProperty("ID");
        if (default == pKeyById)
            pKeyById = _dbSet.Create().GetType().GetProperty("Id");

        var key = pKeyById?.GetValue(entityToUpdate);

        if (entry.State != EntityState.Detached) return;

        var set = _dbContext.Set<TEntity>();
        var attachedEntity = set.Find(key); // access the key

        if (attachedEntity != default)
        {
            var attachedEntry = _dbContext.Entry(attachedEntity);
            attachedEntry.CurrentValues.SetValues(entityToUpdate);

            if (string.IsNullOrEmpty(_includeProperties)) return;

            var includes = _includeProperties.Split(',');
            foreach (var include in includes)
                attachedEntry.Collection(include.Trim()).Load();
            var isUpdated = (attachedEntry.Entity as BaseModel)?.UpdateDetails(entityToUpdate as BaseModel);
            //(entityToUpdate as BaseModel).Adapt(attachedEntry.Entity as BaseModel);
            if (false == (isUpdated ?? false))
                entityToUpdate.Adapt(attachedEntry.Entity);
        }
        else
        {
            entry.State = EntityState.Modified; // attach the entity
        }
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        foreach(var entity in entities)
            Update(entity);
    }
    
    public virtual void Delete(object id)
    {
        var entityToDelete = _dbSet.Find(id);
        Delete(entityToDelete);
    }

    public virtual void Delete(TEntity entityToDelete)
    {
        if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
        {
            _dbSet.Attach(entityToDelete);
        }
        _dbSet.Remove(entityToDelete);
    }

    public virtual void DeleteRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual void Save()
    {
        try
        {
            // Save changes to the database
            _dbContext.SaveChanges();
        }
        catch (DbEntityValidationException ex)
        {
            // Loop through the validation errors to find the one causing the "string or binary data would be truncated" error
            foreach (var validationErrors in ex.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    /*// Check if the error message contains "string or binary data would be truncated"
                    if (validationError.ErrorMessage.Contains("string or binary data would be truncated"))
                    {*/
                    // Output the property or column name causing the error
                    LogHandler.LogError(new Exception($"Property or column '{validationError.PropertyName}' caused the error: {validationError.ErrorMessage}"));
                    //}
                }
            }

            throw;
        }
        catch (DbUpdateException ex)
        {
            // Check if it's the "string or binary data would be truncated" error
            // Handle the error appropriately, log or throw a custom exception
            LogHandler.LogError(ex.InnerException is SqlException { Number: 8152 }
                ? new Exception("String or binary data would be truncated. Check column lengths.")
                // Handle other types of DbUpdateException
                : LogHandler.GetDetailException(ex));
            throw;
        }
    }
    #endregion
}