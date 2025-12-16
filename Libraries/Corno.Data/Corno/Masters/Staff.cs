using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Corno.Data.Common;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class Staff : PersonModel
{
    #region -- Constructors -- 
    public Staff()
    {
        DateOfBirth = new DateTime(1900, 1, 1);
        RecognitionDate = new DateTime(1900, 1, 1);
        RecognitionLetter = new byte[] { 1};

        StaffSubjectDetails = new List<StaffSubjectDetail>();
    }
    #endregion

    #region -- Properties --
    public string Salutation { get; set; }
    public string StaffType { get; set; }
    public string Address1 { get; set; }
    public string Phone { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public string Address2 { get; set; }
    public bool? UseCollegeAddress { get; set; }
    public int? FacultyId { get; set; }

    public string Organization { get; set; }
    public int? CollegeId { get; set; }
    public int? DesignationId { get; set; }
    public string Designation { get; set; }
    public int? DepartmentId { get; set; }
    public int? GenderId { get; set; }
    public string LastDegree { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public int? PositionId { get; set; }
    public string BankAccountType { get; set; }
    public string BankCity { get; set; }
    public string BankAccountNo { get; set; }

    public string IfscCode { get; set; }
    public string BankName { get; set; }
    public string BranchName { get; set; }

    public int? PreviousExperience { get; set; }
    public bool? IsBarred { get; set; }
    public bool? IsEmployee { get; set; }
    public int? MaxScholars { get; set; }
    public DateTime? RecognitionDate { get; set; }
    public byte[] RecognitionLetter { get; set; }

    public List<StaffSubjectDetail> StaffSubjectDetails { get; set; }
    #endregion

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not Staff newStaff) return false;

        foreach (var detail in StaffSubjectDetails)
        {
            var newDetail = newStaff.StaffSubjectDetails.FirstOrDefault(d =>
                d.Id == detail.Id);
            detail.Copy(newDetail);
        }

        // Add new entries
        var newDetails = newStaff.StaffSubjectDetails.Where(d => d.Id <= 0).ToList();
        foreach (var detail in newDetails)
            detail.Id ??= 0;
        StaffSubjectDetails.AddRange(newDetails);

        // Remove items from list1 that are not in list2
        StaffSubjectDetails.RemoveAll(x => newStaff.StaffSubjectDetails.All(y => y.Id != x.Id));

        return true;
    }
    #endregion
}