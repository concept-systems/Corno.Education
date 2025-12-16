using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Data.Corno.Online_Education;
using Corno.Data.Dtos.Import;
using Corno.Data.Dtos.Online_Education;
using Corno.Data.ViewModels;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.Services.Common.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Online_Education.Interfaces;
using Corno.Services.File.Interfaces;
using Mapster;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using Corno.Data.Helpers;

namespace Corno.Services.Corno.Online_Education;

public class OnlineEducationStudentService : MainService<OnlineStudent>, IOnlineEducationStudentService
{
    #region -- Constructors --
    public OnlineEducationStudentService(IExcelFileService<OnlineStudentImportDto> excelFileService,
        IAmountInWordsService amountInWordsService)
    {
        _excelFileService = excelFileService;
        _amountInWordsService = amountInWordsService;

        TypeAdapterConfig<OnlineStudentImportDto, OnlineStudent>.NewConfig()
            .Ignore(p => p.Id);

        _progressDto = new ProgressDto();
    }
    #endregion

    #region -- Data Members --
    private readonly IExcelFileService<OnlineStudentImportDto> _excelFileService;
    private readonly IAmountInWordsService _amountInWordsService;

    private readonly ProgressDto _progressDto;
    #endregion

    #region -- Methods --

    public void UpdateExamFees(ExamViewModel dto)
    {
        if (dto.CollegeId != 45)
            return;

        var student = FirstOrDefault(p => p.Prn == dto.PrnNo, p => p);
        if (null == student)
            throw new Exception($"Your PRN '{dto.PrnNo}' data is not available.");

        var feeInfo = FeeInfo.GetFeeInfo(student.FeeId ?? 0);
        dto.FeeId = student.FeeId;
        //LogHandler.LogInfo($"Passing Certificate Fee  : {dto.CertificateOfPassingFee}");
        var dtoCertificateOfPassingFee = dto.CertificateOfPassingFee ?? 0;
        switch (dto.FeeId)
        {
            case 1:
            case 2:
                dtoCertificateOfPassingFee = 0;
                break;
        }
        switch (feeInfo.RegularFee)
        {
            case true:
                dto.RegularFee45 = 0;
                if (student.RegularFee > 0)
                {
                    dto.ExamFee = student.RegularFee;
                }

                break;
            case false:
                dto.RegularFee45 = dto.ExamFee ?? 0;

                // Update 45 fees 
                /*dto.RegularFee45 = dto.RegularFee45 - dto.CapFee.ToInt() - dto.StatementOfMarksFee.ToInt()
                                   - dto.CertificateOfPassingFee.ToInt();
                dto.CapFee45 = dto.CapFee.ToInt();
                dto.StatementOfMarksFee45 = dto.StatementOfMarksFee.ToInt() + dto.CertificateOfPassingFee.ToInt();*/

                dto.ExamFee = 0;
                dto.CertificateOfPassingFee = 0;
                break;
        }

        switch (feeInfo.BacklogFee)
        {
            case true:
                dto.BackLogFee45 = 0;
                if (student.BacklogFee > 0)
                {
                    // Get Backlog Fee from Student Table
                    var failSubjectsCount = dto.ExamSubjectViewModels
                        .Count(d => d.SubjectType == nameof(SubjectType.BackLog));
                    // Calculate Backlog Fee based on fail subjects count
                    dto.BacklogFee = failSubjectsCount * student.BacklogFee;

                    var coursePartCount = dto.ExamSubjectViewModels.DistinctBy(d => d.CoursePartId).Count();
                    var backLogCoursePartCount = dto.ExamSubjectViewModels
                        .DistinctBy(d => d.CoursePartId)
                        .Count(d => d.SubjectType == nameof(SubjectType.BackLog));
                    var coursePartCapFee = (dto.CapFee ?? 0) / coursePartCount;
                    var coursePartStatementFee = (dto.StatementOfMarksFee ?? 0) / coursePartCount;
                    // Calculate Cap Fee based on course part count
                    dto.CapFee = coursePartCapFee * backLogCoursePartCount;
                    dto.BacklogFee -= dto.CapFee;
                    // Calculate Statement Fee based on course part count
                    dto.StatementOfMarksFee = coursePartStatementFee * backLogCoursePartCount;
                    dto.BacklogFee -= dto.StatementOfMarksFee;
                }

                break;
            case false:
                dto.BackLogFee45 = dto.BacklogFee ?? 0;
                dto.BacklogFee = 0;
                break;
        }

        // If both fees will be paid by student.
        if (dto.BacklogFee <= 0 && dto.ExamFee <= 0)
        {
            dto.CapFee45 = dto.CapFee ?? 0;
            dto.StatementOfMarksFee45 = dto.StatementOfMarksFee ?? 0;
            dto.CapFee = 0;
            dto.StatementOfMarksFee = 0;
        }

        //dto.TotalFee45 = dto.Total ?? 0;
        dto.TotalFee45 = dto.RegularFee45.ToInt() + dto.BackLogFee45.ToInt() +
                         dto.CapFee45.ToInt() + dto.StatementOfMarksFee45.ToInt() +
                         dtoCertificateOfPassingFee;

        dto.Total = dto.ExamFee + dto.BacklogFee + dto.CapFee + dto.StatementOfMarksFee +
            dto.PracticalFee + dto.DissertationFee + dto.OthersFee + dto.LateFee +
            dto.SuperLateFee + dto.EnvironmentalExaminationFee +
            dto.CertificateOfPassingFee ?? 0;

        dto.FeeInWord = _amountInWordsService.GetAmountInWords(dto.Total.ToString());
        dto.BacklogSummary = string.Empty;
    }

    public void UpdateAppTemp(Tbl_APP_TEMP appTemp, Exam exam)
    {
        appTemp.FeeId = exam.FeeId;
        appTemp.Num_RegularFee45 = exam.RegularFee45;
        appTemp.Num_BacklogFee45 = exam.BackLogFee45;
        appTemp.Num_CapFee45 = exam.CapFee45;
        appTemp.Num_StatementFee45 = exam.StatementOfMarksFee45;
        appTemp.Num_TotalFee45 = exam.TotalFee45;
    }

    public IEnumerable<OnlineStudentImportDto> Import(HttpPostedFileBase file)
    {
        try
        {
            _progressDto.ReportMessage("Reading excel file");
            var records = _excelFileService.Read(file.InputStream)
                .ToList();
            if (!records.Any())
                throw new Exception("No entries in excel file to import");

            // Create progress model
            _progressDto.Initialize(file.FileName, 0, records.Count(), 1);
            _progressDto.ReportMessage("Importing records");
            foreach (var record in records)
            {
                var student = FirstOrDefault(p => p.Prn == record.Prn, p => p);
                if (null != student)
                {
                    record.Adapt(student);
                    Update(student);
                    continue;
                }

                // Create Plan
                student = record.Adapt<OnlineStudent>();
                Add(student);
            }

            Save();

            _progressDto.ReportMessage("Imported successfully.");
            return records;
        }
        catch (Exception exception)
        {
            _progressDto.ReportMessage(LogHandler.GetDetailException(exception)?.Message);
            throw;
        }
    }

    public void CancelImport()
    {
        _progressDto.CancelRequested();
    }
    #endregion
}