using System;
using System.Collections.Generic;
using CsvHelper.Configuration.Attributes;

namespace Corno.Data.Corno;

public class AbcExport
{
    public AbcExport()
    {
        AbcExportDetails = new List<AbcExportDetail>();
        AbcSubjectHeaders = new List<AbcSubjectHeader>();
    }

    [Name("ORG_CODE")]
    public short CollegeCode { get; set; }
    [Name("ORG_NAME")]
    public string CollegeName { get; set; }
    [Name("ACADEMIC_COURSE_ID")]
    public short CourseCode { get; set; }
    [Name("COURSE_NAME")]
    public string CourseName { get; set; }
    [Name("STREAM")]
    public string Branch { get; set; }
    [Name("SESSION")]
    public string Instance { get; set; }
    [Name("REGN_NO")]
    public string Prn { get; set; }
    [Name("RROLL")]
    public long? SeatNo { get; set; }
    [Name("CNAME")]
    public string StudentName { get; set; }
    [Name("GENDER")]
    public string Gender { get; set; }

    [Name("DOB")]
    public string DateOfBirth { get; set; }
    [Name("MRKS_REC_STATUS")]
    public string MarksRecordStatus { get; set; }
    [Name("RESULT")]
    public string CoursePartResult { get; set; }
    [Name("YEAR")]
    public short? Year { get; set; }
    [Name("Month")]
    public short? Month { get; set; }
    [Name("DOR")]
    public DateTime? ResultDate { get; set; }
    [Name("SEM")]
    public short? SemesterNo { get; set; }
    [Name("CGPA")]
    public decimal? Cgpa { get; set; }
    [Name("SGPA")]
    public decimal? Sgpa { get; set; }
    [Name("ABC_ACCOUNT_ID")]
    public string AbcAccountId { get; set; }


    [Name("TOT_GRADE_POINTS")]
    public double? TotalGradePoints { get; set; }
    [Name("TOT_CREDIT")]
    public double? TotalCredit { get; set; }
    [Name("TOT_CREDIT_POINTS")]
    public decimal? TotalCreditPoints { get; set; }
    /*[Name("SUB1")]
    public int SubjectCode { get; set; }
    [Name("SUB1NM")]
    public string SubjectName { get; set; }
    [Name("SUB1_GRADE")]
    public string SubjectGrade { get; set; }
    [Name("SUB1_GRADE_POINTS")]
    public double? SubjectGradePoints { get; set; }
    [Name("SUB1_CREDIT")]
    public double? SubjectCredit { get; set; }
    [Name("SUB1_CREDIT_POINTS\r\n")]
    public double? SubjectCreditPoints { get; set; }*/

    public List<AbcExportDetail> AbcExportDetails { get; set; }
    public List<AbcSubjectHeader> AbcSubjectHeaders { get; set; }
}

public class AbcExportDetail
{
    [Name("SUB1")]
    public int SubjectCode { get; set; }
    [Name("SUB1NM")]
    public string SubjectName { get; set; }
    [Name("SUB1_GRADE")]
    public string SubjectGrade { get; set; }
    [Name("SUB1_GRADE_POINTS")]
    public double? SubjectGradePoints { get; set; }
    [Name("SUB1_CREDIT")]
    public double? SubjectCredit { get; set; }
    [Name("SUB1_CREDIT_POINTS")]
    public double? SubjectCreditPoints { get; set; }
}

public class AbcSubjectHeader
{
    public string SubjectCode { get; set; }
    public string SubjectName { get; set; }
    public string SubjectGrade { get; set; }
    public string SubjectGradePoints { get; set; }
    public string SubjectCredit { get; set; }
    public string SubjectCreditPoints { get; set; }
}