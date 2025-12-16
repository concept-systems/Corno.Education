using Corno.Data.Common;
using Mapster;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class CollegeCourseDetail : BaseModel
{
    #region -- Constructors --
    public CollegeCourseDetail()
    {
        SubjectId = 0;
    }
    #endregion

    #region -- Properties --
    public int? CollegeId { get; set; }

    public int FacultyId { get; set; }
    public int CourseId { get; set; }
    public int SubjectId { get; set; }

    [NotMapped]
    public string FacultyName { get; set; }
    [NotMapped]
    public string CourseName { get; set; }
    [NotMapped]
    public string SubjectName { get; set; }

    [AdaptIgnore]
    protected virtual College College { get; set; }
    #endregion

    #region -- Public Methods --
    public void Copy(CollegeCourseDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        FacultyId = other.FacultyId;
        CourseId = other.CourseId;
        SubjectId = other.SubjectId;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}