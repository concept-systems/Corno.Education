using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Common;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class Course : MasterModel
{
    #region -- Constructors -- 
    public Course()
    {
        CourseCategoryDetails = new List<CourseCategoryDetail>();
    }
    #endregion

    #region -- Properties
    public int? FacultyId { get; set; }
    public int? CourseTypeId { get; set; }
    public int? BosId { get; set; }
    public int? StartYear { get; set; }
    public int? CloseYear { get; set; }
    public int? TotalSemesters { get; set; }
    public int? PrintSequenceNo { get; set; }
    public string LetterAddress { get; set; }
    public string NameOnCertificate { get; set; }
    public byte[] Register { get; set; }
    public byte[] Pattern { get; set; }
    public string RegisterFileName { get; set; }
    public string PatternFileName { get; set; }

    public List<CourseCategoryDetail> CourseCategoryDetails { get; set; }
    #endregion

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not Course newCourse) return false;

        foreach (var detail in CourseCategoryDetails)
        {
            var newDetail = newCourse.CourseCategoryDetails.FirstOrDefault(d =>
                d.Id == detail.Id);
            detail.Copy(newDetail);
        }

        // Add new entries
        var newDetails = newCourse.CourseCategoryDetails.Where(d => d.Id <= 0).ToList();
        CourseCategoryDetails.AddRange(newDetails);

        // Remove items from list1 that are not in list2
        CourseCategoryDetails.RemoveAll(x => newCourse.CourseCategoryDetails.All(y => y.Id != x.Id));

        return true;
    }
    #endregion
}