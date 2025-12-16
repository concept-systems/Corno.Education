using Corno.Data.Common;
using Mapster;
using System;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class StudentProgramDetail : BaseModel
{
    #region -- Constructors -- 
    public StudentProgramDetail()
    {
    }
    #endregion

    #region -- Properties --
    public int? StudentId { get; set; }

    public int AdmissionYear { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public int? FacultyId { get; set; }
    public int? ResearchCenterId { get; set; }
    public int? SubjectId { get; set; }
    public string Mode { get; set; }
    public int TuitionFeePerYear { get; set; }

    [AdaptIgnore]
    protected virtual Student Student { get; set; }
    #endregion

    #region -- Public Methods --
    public void Copy(StudentProgramDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        AdmissionYear = other.AdmissionYear;
        AdmissionDate = other.AdmissionDate;
        FacultyId = other.FacultyId;
        ResearchCenterId = other.ResearchCenterId;
        SubjectId = other.SubjectId;
        Mode = other.Mode;
        TuitionFeePerYear = other.TuitionFeePerYear;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}