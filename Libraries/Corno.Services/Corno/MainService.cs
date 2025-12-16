using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno;

public class MainService<TEntity> : BaseService, IMainService<TEntity>
where TEntity : class
{
    #region -- Constructors --

    public MainService()
    {
        _entityRepository = Bootstrapper.Bootstrapper.Get<IGenericRepository<TEntity>>();
    }
    #endregion

    #region -- Data Members --
    //private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<TEntity> _entityRepository;
    #endregion

    #region -- Methods --
    public void SetIncludes(string includes)
    {
        _entityRepository.SetIncludes(includes);
    }

    public virtual IQueryable<TEntity> GetQuery()
    {
        return _entityRepository.GetQuery();
    }

    public TEntity GetById(object id)
    {
        return _entityRepository.GetById(id);
    }

    public IQueryable<TDest> Get<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        return _entityRepository.Get(filter, select, orderBy);
    }


    public TDest FirstOrDefault<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        return _entityRepository.Get(filter, select, orderBy).FirstOrDefault();
    }

    public TDest LastOrDefault<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null)
    {
        return _entityRepository.Get(filter, select).OrderByDescending(p => p).FirstOrDefault();
    }

    public virtual void Add(TEntity entity)
    {
        _entityRepository.Add(entity);
    }

    public virtual void AddAndSave(TEntity entity)
    {
        Add(entity);
        Save();
    }

    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        _entityRepository.AddRange(entities);
    }

    public virtual void AddRangeAndSave(IEnumerable<TEntity> entities)
    {
        AddRange(entities);
        Save();
    }

    public virtual void Update(TEntity entityToUpdate)
    {
        _entityRepository.Update(entityToUpdate);
    }

    public virtual void UpdateAndSave(TEntity entityToUpdate)
    {
        Update(entityToUpdate);
        Save();
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {

        _entityRepository.UpdateRange(entities);
    }

    public virtual void UpdateRangeAndSave(IEnumerable<TEntity> entities)
    {
        UpdateRange(entities);
        Save();
    }

    public virtual void Delete(TEntity entityToUpdate)
    {
        _entityRepository.Delete(entityToUpdate);
    }

    public void Save()
    {
        _entityRepository.Save();
        //_unitOfWork.Save();
    }

    #endregion
}