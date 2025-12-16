using Corno.Data.Common;
using Corno.Data.Corno.Masters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Corno.Data.Corno.Paper_Setting.Models;

[Serializable]
public class Remuneration : UniversityBaseModel
{
    #region -- Constructors -- 
    public Remuneration()
    {
        //EnableHeader = true;

        RemunerationDetails = new List<RemunerationDetail>();
    }
    #endregion

    [NotMapped]
    public new int? CoursePartId { get; set; }
    [NotMapped]
    public new int? CentreId { get; set; }
    [NotMapped]
    public new int? CourseTypeId { get; set; }
    [NotMapped]
    public new int? BranchId { get; set; }

    public List<RemunerationDetail> RemunerationDetails { get; set; }

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not Remuneration newRemuneration) return false;

        foreach (var remunerationDetail in RemunerationDetails)
        {
            var newRemunerationDetail = newRemuneration.RemunerationDetails.FirstOrDefault(d =>
                d.Id == remunerationDetail.Id);
            remunerationDetail.Copy(newRemunerationDetail);
        }

        // Add new entries
        var newRemunerationDetails = newRemuneration.RemunerationDetails.Where(d => d.Id <= 0).ToList();
        RemunerationDetails.AddRange(newRemunerationDetails);

        // Remove items from list1 that are not in list2
        RemunerationDetails.RemoveAll(x => newRemuneration.RemunerationDetails.All(y => y.Id != x.Id));

        return true;
    }
    #endregion
}