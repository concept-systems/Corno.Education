using Corno.Data.Common;
using Mapster;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class StaffSubjectDetail : BaseModel
{
    #region -- Properties --
    public int? StaffId { get; set; }
    public int FacultyId { get; set; }
    public int CollegeId { get; set; }
    public int CourseId { get; set; }
    public int CoursePartId { get; set; }
    public int SubjectId { get; set; }
    public bool? IsInternal { get; set; }
    public bool? IsBarred { get; set; }

    [NotMapped]
    public string FacultyName { get; set; }
    [NotMapped]
    public string CollegeName { get; set; }
    [NotMapped]
    public string CourseName { get; set; }
    [NotMapped]
    public string CoursePartName { get; set; }
    [NotMapped]
    public string SubjectName { get; set; }

    [AdaptIgnore]
    protected virtual Staff Staff { get; set; }

    #endregion

    #region -- Public Methods --

    public override void Reset()
    {
        base.Reset();

        FacultyId = 0;
        CollegeId = 0;
        CourseId = 0;
        CoursePartId = 0;
        SubjectId = 0;
    }

    public void Copy(StaffSubjectDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        //StaffId = other.StaffId;
        CollegeId = other.CollegeId;
        CourseId = other.CourseId;
        CoursePartId = other.CoursePartId;
        SubjectId = other.SubjectId;
        IsInternal = other.IsInternal ?? false;
        IsBarred = other.IsBarred ?? false;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}