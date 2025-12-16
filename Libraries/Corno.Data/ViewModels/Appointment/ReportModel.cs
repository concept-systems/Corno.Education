using System;
using Corno.Data.Corno.Masters;
using Corno.Data.Corno.Paper_Setting.Models;

namespace Corno.Data.ViewModels.Appointment;

public class ReportModel
{
    #region -- Propertis --
    public Corno.Paper_Setting.Models.Appointment Appointment { get; set; }
    public Schedule Schedule { get; set; }

    public int? PrintSequenceNo { get; set; }
    public string LetterAddress { get; set; }
    public DateTime? MeetingDate { get; set; }
    public DateTime? ToDate { get; set; }
    public TimeSpan? Time { get; set; }
    public string OutWardNo { get; set; }
    public int OutWardNoInt { get; set; }

    public int? StandardSets { get; set; }

    public string SubjectType { get; set; }

    public int? StaffId { get; set; }
    public string StaffName { get; set; }
    public string StaffAddress { get; set; }
    public string DesignationName { get; set; }
    public string Phone { get; set; }
    public string MobileNo { get; set; }
    public string EmailId { get; set; }
    public string Address2 { get; set; }
    public string CollegeEmailId { get; set; }

    public bool? IsChairman { get; set; }
    public bool? IsInternal { get; set; }
    public bool? IsModerator { get; set; }
    public bool? IsManuscript { get; set; }
    public int? SetsToBeDrawn { get; set; }
    public int? NoOfAttempts { get; set; }
    public int? OriginalId { get; set; }
    public int? SmsCount { get; set; }
    public int? EmailCount { get; set; }
    public DateTime? EmailDate { get; set; }
    public string OriginalName { get; set; }
    public string OriginalAddress { get; set; }
    public int? BosId { get; set; }
    public string BosName { get; set; }

    public string InWords { get; set; }

    // For subject only
    public bool PaperCategoryApplicable { get; set; } = false;

    public MasterViewModel Instance { get; set; } = new();
    public MasterViewModel Faculty { get; set; } = new();
    public College College { get; set; } = new();
    public MasterViewModel Course { get; set; } = new();
    public MasterViewModel CoursePart { get; set; } = new();
    public MasterViewModel Branch { get; set; } = new();
    public MasterViewModel Category { get; set; } = new();
    public MasterViewModel Subject { get; set; } = new();

    public ReportBillModel BillModel { get; set; } = new();
    public ReportBankModel BankModel { get; set; } = new();
    #endregion
}