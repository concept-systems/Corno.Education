using Corno.Data.Common;
using Corno.Data.Helpers;
using Corno.Data.ViewModels;
using Corno.Services.Corno.Interfaces;
using LinqKit;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Corno.Services.Corno;

public class MainMasterService<TEntity> : MainService<TEntity>, IMainMasterService<TEntity>
where TEntity : MasterModel
{
    #region -- Methods --
    public virtual MasterViewModel GetViewModel(object id)
    {
        var intId = id.ToInt();
        return FirstOrDefault(m => m.Id == intId, m => new MasterViewModel
        {
            Id = m.Id ?? 0,
            Code = m.Code,
            Name = m.Name,
            Description = m.Description,
            NameWithCode = "(" + m.Code + ")" + " - " + m.Name,
            NameWithId = "(" + m.Id + ")" + " - " + m.Name
        });
    }
    
    public virtual IQueryable<MasterViewModel> GetViewModelList(Expression<Func<TEntity, bool>> filter = default,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = default, string includeProperties = default)
    {
        var masters = Get(filter, m => new MasterViewModel
            {
                Id = m.Id ?? 0,
                Code = m.Code,
                Name = m.Name,
                Description = m.Description
            })
            .AsEnumerable() // Execute the query and bring data into memory
            .Select(m =>
            {
                m.NameWithCode = $"({m.Code}) - {m.Name}";
                m.NameWithId = $"({m.Id}) - {m.Name}";
                return m;
            }).AsQueryable();
        return masters;
    }

    public Expression<Func<TEntity, bool>> AddContainsFilter(Expression<Func<TEntity, bool>> predicate = default, string filter = default)
    {
        if (!string.IsNullOrEmpty(filter) && !filter.Contains(") -"))
            predicate = predicate.And(p => p.Id.ToString().ToLower().Contains(filter.ToLower()) || 
                                           p.Name.ToLower().Contains(filter.ToLower()));
        return predicate;
    }

    #endregion
}