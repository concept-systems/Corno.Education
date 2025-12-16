using System;
using Corno.Data.Common;

namespace Corno.Data.Core;

public class ConvocationFeeCommon : BaseModel
{
    //public int InstanceID { get; set; }
    //public Nullable<int> CourseTypeID { get; set; }
    //public Nullable<double> Fee { get; set; }
}

public class ConvocationFee : ConvocationFeeCommon
{
    public int InstanceId { get; set; }
    public int FacultyId { get; set; }
    public int? CourseTypeId { get; set; }
    public double? Fee { get; set; }
    public double? PostalIndia { get; set; }
    public double? PostalOverseas { get; set; }
    public double? LateFees { get; set; }
    public double? FeesUrgent { get; set; }
    public int? ConvocationNo { get; set; }
    public DateTime? ConvocationDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class ConvocationFeeViewModel : ConvocationFeeCommon
{
    public ConvocationFeeViewModel()
    {
        UndergraduateDiplomaFee = 0;
        GraduateDegreeFee = 0;
        PostGraduateDiplomaFee = 0;
        PostGraduateDegreeFee = 0;
        MPhilFee = 0;
        PhDFee = 0;
        ForStudentsInIndiaFee = 0;
        ForStudentsInAbroadFee = 0;
    }

    public double UndergraduateDiplomaFee { get; set; }
    public double GraduateDegreeFee { get; set; }
    public double PostGraduateDiplomaFee { get; set; }
    public double PostGraduateDegreeFee { get; set; }
    public double MPhilFee { get; set; }
    public double PhDFee { get; set; }
    public double ForStudentsInIndiaFee { get; set; }
    public double ForStudentsInAbroadFee { get; set; }
}