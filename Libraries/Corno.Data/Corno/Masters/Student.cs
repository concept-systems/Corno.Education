using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Corno.Data.Common;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class Student : PersonModel
{
    #region -- Constructors -- 
    public Student()
    {
        StudentAddressDetails = new List<StudentAddressDetail>();
    }
    #endregion

    #region -- Properties --

    // Personal Details
    public string Prn { get; set; }
    public byte[] Photo { get; set; }
    public int? GenderId { get; set; }
    public string Nationality { get; set; }
    public string AadhaarNo { get; set; }
    public bool IsEmployee { get; set; }
    
    // Address Details
    [NotMapped]
    public StudentAddressDetail StudentAddressDetail { get; set; }

    // Program Details
    public DateTime? AdmissionDate { get; set; }
    public int? FacultyId { get; set; }
    public int? SubjectId { get; set; }
    public int? ResearchCenterId { get; set; }
    public int? TuitionFee { get; set; }

    // Registration Status
    public string RegistrationStatus { get; set; }
    public string RegistrationComment { get; set; }

    // Course Work
    public DateTime? ResultDate { get; set; }
    public string ResultGrade { get; set; }
    public string ResultStatus { get; set; }

    // RAC - Research Advisory Committee
    public int? GuideId { get; set; }
    public string HeadOfDepartment { get; set; }
    public string SeniorFaculty { get; set; }

    // Research Progress
    public DateTime? RacPreSubmissionDate { get; set; }
    public DateTime? GuideReportDate { get; set; }

    // Referee Details
    public int? Referee1Id { get; set; }
    public int? Referee2Id { get; set; }

    // Thesis Evaluation Details
    public bool? IsThesisComplianceRequired { get; set; }
    public string ThesisComments { get; set; }
    /*public DateTime? ThesisSubmissionDate { get; set; }
        public DateTime? SynopsisCommunicationDate { get; set; }
        public DateTime? ThesisCommunicationDate { get; set; }
        public DateTime? Referee1ReportReceivableDate { get; set; }
        public DateTime? Referee2ReportReceivableDate { get; set; }*/

    // Viva Examination Details
    public DateTime? VivaExaminationDate { get; set; }
    public string VivaChairPerson { get; set; }
    public string VivaExaminer { get; set; }
    public DateTime? VivaExamReceivableDate { get; set; }
    public DateTime? VivaExaminerPaidDate { get; set; }
    public string VivaComments{ get; set; }

    // Award of Degree Details
    public DateTime? NotificationDate { get; set; }
    public byte[] NotificationDocument { get; set; }
    public DateTime? HonorariumPaidDate { get; set; }
    public DateTime? ConvocationDate { get; set; }
    public byte[] DegreeCertificate { get; set; }


    public List<StudentAddressDetail> StudentAddressDetails { get; set; }
    public List<StudentResearchDetail> StudentProgressDetails { get; set; }
    #endregion

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not Student newStudent) return false;

        foreach (var detail in StudentAddressDetails)
        {
            var newDetail = newStudent.StudentAddressDetails.FirstOrDefault(d =>
                d.Id == detail.Id);
            detail.Copy(newDetail);
        }

        // Add new entries
        var newDetails = newStudent.StudentAddressDetails.Where(d => d.Id <= 0).ToList();
        foreach (var detail in newDetails)
            detail.Id ??= 0;
        StudentAddressDetails.AddRange(newDetails);

        // Remove items from list1 that are not in list2
        StudentAddressDetails.RemoveAll(x => newStudent.StudentAddressDetails.All(y => y.Id != x.Id));

        return true;
    }
    #endregion
}