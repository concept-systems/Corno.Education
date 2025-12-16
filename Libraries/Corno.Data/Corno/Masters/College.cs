using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Common;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class College : MasterModel
{
    #region -- Constructors -- 
    public College()
    {
        CollegeCourseDetails = new List<CollegeCourseDetail>();
    }
    #endregion

    #region -- Properties --
    public int? CollegeTypeId { get; set; }

    public string ShortName { get; set; }
    public string RegistrationNo { get; set; }
    public string Principal { get; set; }

    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string Pin { get; set; }
    public string Phone { get; set; }
    public string Mobile { get; set; }
    public string Fax { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public DateTime? EstablishmentYear { get; set; }

    public List<CollegeCourseDetail> CollegeCourseDetails { get; set; }
    #endregion

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not College newCollege) return false;

        foreach (var detail in CollegeCourseDetails)
        {
            var newDetail = newCollege.CollegeCourseDetails.FirstOrDefault(d =>
                d.Id == detail.Id);
            detail.Copy(newDetail);
        }

        // Add new entries
        var newDetails = newCollege.CollegeCourseDetails.Where(d => d.Id <= 0).ToList();
        CollegeCourseDetails.AddRange(newDetails);

        // Remove items from list1 that are not in list2
        CollegeCourseDetails.RemoveAll(x => newCollege.CollegeCourseDetails.All(y => y.Id != x.Id));

        return true;
    }
    #endregion
}