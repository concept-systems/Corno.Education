using Corno.Data.Common;
using Corno.Data.Contexts;
using Corno.Logger;
using Corno.Services.Corno.Interfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;

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


    private static readonly object _dbLock = new();
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

        if (!string.IsNullOrEmpty(includeProperties))
            _includeProperties = includeProperties;

        query = _includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        /*if (!string.IsNullOrEmpty(_includeProperties))
            query = _includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));*/

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
        IQueryable<TEntity> query = _dbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(_includeProperties))
        {
            var includes = _includeProperties.Split(',');
            query = includes.Aggregate(query, (current, include) => current.Include(include.Trim()));
        }

        // Build expression: e => e.Id == id
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var property = Expression.Property(parameter, "Id");
        var constant = Expression.Constant(id);
        var equality = Expression.Equal(property, Expression.Convert(constant, property.Type));
        var lambda = Expression.Lambda<Func<TEntity, bool>>(equality, parameter);

        return query.FirstOrDefault(lambda);
    }


    /*public virtual TEntity GetById(object id)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();
        if (!string.IsNullOrEmpty(_includeProperties))
        {
            var includes = _includeProperties.Split(',');
            query = includes.Aggregate(query, (current, include) => current.Include(include.Trim()));
        }

        // Get the property info for "Id"
        var idProperty = typeof(TEntity).GetProperty("Id");
        if (idProperty == null)
            throw new InvalidOperationException($"Type {typeof(TEntity).Name} does not contain a property named 'Id'.");

        // Materialize the query and find the entity with matching Id
        return query.ToList().FirstOrDefault(e =>
        {
            var value = idProperty.GetValue(e);
            return value != null && value.Equals(id);
        });
    }*/


    /*public virtual TEntity GetById(object id)
    {
        var entity = _dbSet.Find(id);
        if (string.IsNullOrEmpty(_includeProperties)) 
            return entity;

        var includes = _includeProperties.Split(',');
        foreach (var include in includes)
            _dbContext.Entry(entity).Collection(include.Trim()).Load();

        return entity;
        //return _dbSet.Find(id);
    }*/

    /*public IEnumerable<TEntity> Find(Func<TEntity, bool> predicate)
    {
        return _dbSet.Where(predicate);
    }*/

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

    public virtual void Update(TEntity entityToUpdate)
    {
        var entry = _dbContext.Entry(entityToUpdate);

        // Retrieve the Id through reflection
        var pKeyById = _dbSet.Create().GetType().GetProperty("ID");
        if (default == pKeyById)
            pKeyById = _dbSet.Create().GetType().GetProperty("Id");

        if (null == pKeyById)
            throw new Exception($"GenericRepository: Update : Primary key is not defined for entity '{typeof(TEntity).Name}'.");

        var key = pKeyById.GetValue(entityToUpdate);

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
            LogHandler.LogInfo($"Update : else Part : Entity of type '{typeof(TEntity).Name}' with key '{key}' not found in the database. Attaching entity for update.");
            set.Attach(entityToUpdate); // Attach the entity
            var entryToUpdate = _dbContext.Entry(entityToUpdate);
            foreach (var propertyName in entryToUpdate.OriginalValues.PropertyNames)
            {
                if (!string.Equals(propertyName, "Id", StringComparison.OrdinalIgnoreCase))
                {
                    entryToUpdate.Property(propertyName).IsModified = true;
                }
            }
        }
    }


    /*public virtual void Update(TEntity entityToUpdate)
    {
        var entry = _dbContext.Entry(entityToUpdate);

        // Retrieve the Id through reflection
        var pKeyById = _dbSet.Create().GetType().GetProperty("ID");
        if (default == pKeyById)
            pKeyById = _dbSet.Create().GetType().GetProperty("Id");

        if (null == pKeyById)
            throw new Exception($"GenericRepository: Update : Primary key is not defined for entity '{typeof(TEntity).Name}'.");

        var key = pKeyById.GetValue(entityToUpdate);

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
    }*/

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
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
            lock (_dbLock)
            {
                _dbContext.SaveChanges();
            }
        }
        catch (DbEntityValidationException exception)
        {
            // Loop through the validation errors to find the one causing the "string or binary data would be truncated" error
            foreach (var validationErrors in exception.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                    LogHandler.LogError(new Exception($"Property or column '{validationError.PropertyName}' caused the error: {validationError.ErrorMessage}"));
            }
            foreach (var entry in _dbContext.ChangeTracker.Entries())
                entry.State = EntityState.Detached;

            throw;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            foreach (var entry in _dbContext.ChangeTracker.Entries())
                entry.State = EntityState.Detached;
        }

        /*catch (DbUpdateException ex)
        {
            // Check if it's the "string or binary data would be truncated" error
            // Handle the error appropriately, log or throw a custom exception
            LogHandler.LogError(ex.InnerException is SqlException { Number: 8152 }
                ? new Exception("String or binary data would be truncated. Check column lengths.")
                // Handle other types of DbUpdateException
                : LogHandler.GetDetailException(ex));
            throw;
        }*/
    }
    #endregion
}