using Corno.Data.Common;
using Corno.Data.Corno.Masters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Corno.Data.Corno.Paper_Setting.Models;

[Serializable]
public class Schedule : UniversityBaseModel
{
    #region -- Constructors -- 
    public Schedule()
    {
        //EnableHeader = true;

        ScheduleDetails = new List<ScheduleDetail>();
    }
    #endregion

    [NotMapped]
    public new int? CentreId { get; set; }
    [NotMapped]
    public new int? CourseTypeId { get; set; }

    public int? CategoryId { get; set; }

    /*[NotMapped]
    public bool EnableHeader { get; set; }*/

    public List<ScheduleDetail> ScheduleDetails { get; set; }

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not Schedule newSchedule) return false;

        foreach (var scheduleDetail in ScheduleDetails)
        {
            var newScheduleDetail = newSchedule.ScheduleDetails.FirstOrDefault(d =>
                d.Id == scheduleDetail.Id);
            scheduleDetail.Copy(newScheduleDetail);
        }

        // Add new entries
        var newScheduleDetails = newSchedule.ScheduleDetails.Where(d => d.Id <= 0).ToList();
        ScheduleDetails.AddRange(newScheduleDetails);

        // Remove items from list1 that are not in list2
        ScheduleDetails.RemoveAll(x => newSchedule.ScheduleDetails.All(y => y.Id != x.Id));

        return true;
    }
    #endregion
}