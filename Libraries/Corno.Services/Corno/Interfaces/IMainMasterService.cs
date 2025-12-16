using System.Linq.Expressions;
using System.Linq;
using System;
using Corno.Data.Common;
using Corno.Data.ViewModels;

namespace Corno.Services.Corno.Interfaces;

public interface IMainMasterService<TEntity> : IMainService<TEntity>
where TEntity : MasterModel
{
    MasterViewModel GetViewModel(object id);

    IQueryable<MasterViewModel> GetViewModelList(Expression<Func<TEntity, bool>> filter = default,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = default, string includeProperties = default);
    Expression<Func<TEntity, bool>> AddContainsFilter(Expression<Func<TEntity, bool>> predicate = null,
        string filter = default);
}