using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Transactions;
using Corno.Data.Admin;
using Corno.Data.Api;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Data.Helpers;
using Corno.Data.ViewModels;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Services.Core.Interfaces;
using Corno.Services.Core.Marks_Entry.Interfaces;
using Corno.Services.Corno;
using MoreLinq;
using Newtonsoft.Json;

namespace Corno.Services.Core.Marks_Entry;

public class MarksApiService : BaseService, IMarksApiService
{
    #region -- Constructors --
    public MarksApiService(ICoreService coreService)
    {
        _coreService = coreService;
    }
    #endregion

    #region -- Data Members --

    private readonly ICoreService _coreService;

    #endregion

    #region -- Private Methods --
    /*private List<MarksEntryDetail> GetDetails(MarksEntry model)
    {
        model ??= new MarksEntry();

        var instanceId = model.InstanceId;// HttpContext.Session[ModelConstants.InstanceId].ToInt();
        List<MarksEntryDetail> marksEntryDetails;
        if (model.PaperId.ToInt() <= 0)
        {
            var category = _coreService.Tbl_SUBJECT_CAT_MSTR_Repository.Get(c => c.Num_FK_COPRT_NO == model.CoursePartId &&
                    c.Num_FK_SUB_CD == model.SubjectId && c.Num_FK_CAT_CD == model.CategoryId)
                .FirstOrDefault();
            marksEntryDetails = (from exam in _coreService.Tbl_STUDENT_SUBJECT_Repository.Get(e =>
                    e.Num_FK_INST_NO == instanceId && e.Num_FK_COPRT_NO == (model.CoursePartId ?? 0) && e.Num_FK_SUB_CD == model.SubjectId &&
                    e.Chr_ST_SUB_STS == "A" && e.Chr_ST_SUB_CAN != "Y")
                                 join categoryMarks in _coreService.Tbl_STUDENT_CAT_MARKS_Repository.Get(c =>
                                         c.Num_FK_INST_NO == instanceId && c.Num_FK_SUB_CD == model.SubjectId && c.Num_FK_CAT_CD == model.CategoryId &&
                                         c.Var_ST_PH_STS == "A")
                                     on exam.Chr_FK_PRN_NO equals categoryMarks.Var_FK_PRN_NO
                                 join yrChange in _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(y => y.Num_FK_INST_NO == model.InstanceId &&
                                         y.Num_FK_COL_CD == model.CollegeId && (y.Num_FK_DistCenter_ID ?? 0) == (model.CentreId ?? 0))
                                     on exam.Chr_FK_PRN_NO equals yrChange.Chr_FK_PRN_NO
                                 select new MarksEntryDetail
                                 {
                                     Prn = exam.Chr_FK_PRN_NO,
                                     SeatNo = yrChange.Num_ST_SEAT_NO,
                                     MaxMarks = category.Num_CAT_MAX_MRK
                                 }).ToList();
        }
        else
        {
            var paper = _coreService.Tbl_SUB_CATPAP_MSTR_Repository.Get(c => c.Num_FK_COPRT_NO == model.CoursePartId &&
                                                                             c.Num_FK_SUB_CD == model.SubjectId && c.Num_FK_CAT_CD == model.CategoryId && c.Num_PK_PAP_CD == model.PaperId).FirstOrDefault();
            marksEntryDetails = (from exam in _coreService.Tbl_STUDENT_SUBJECT_Repository.Get(e =>
                    e.Num_FK_INST_NO == instanceId && e.Num_FK_COPRT_NO == (model.CoursePartId ?? 0) && e.Num_FK_SUB_CD == model.SubjectId &&
                    e.Chr_ST_SUB_STS == "A" && e.Chr_ST_SUB_CAN != "Y")
                                 join paperMarks in _coreService.Tbl_STUDENT_PAP_MARKS_Repository.Get(c =>
                                     c.Num_FK_INST_NO == instanceId && c.Num_FK_SUB_CD == model.SubjectId &&
                                     c.Num_FK_CAT_CD == model.CategoryId && c.Num_FK_PAP_CD == model.PaperId &&
                                     c.Var_ST_PH_STS == "A") on exam.Chr_FK_PRN_NO equals paperMarks.Var_FK_PRN_NO
                                 join yrChange in _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(y => y.Num_FK_INST_NO == model.InstanceId &&
                                         y.Num_FK_COL_CD == model.CollegeId && (y.Num_FK_DistCenter_ID ?? 0) == (model.CentreId ?? 0))
                                     on exam.Chr_FK_PRN_NO equals yrChange.Chr_FK_PRN_NO
                                 select new MarksEntryDetail
                                 {
                                     Prn = exam.Chr_FK_PRN_NO,
                                     SeatNo = yrChange.Num_ST_SEAT_NO,
                                     MaxMarks = paper.Num_PAP_MAX_MRK
                                 }).ToList();
        }

        return marksEntryDetails.DistinctBy(d => d.Prn).ToList();
    }*/
    #endregion

    #region -- Private Methods --
    private void ValidateFieldsForApi(MarksApiViewModel model)
    {
        if (model.InstanceId.ToInt() <= 0)
            throw new Exception("Invalid Instance.");
        if (model.CourseTypeId.ToInt() <= 0)
            throw new Exception("Invalid Course Type.");
        if (model.CourseId.ToInt() <= 0)
            throw new Exception("Invalid Course.");
        if (model.CoursePartId.ToInt() <= 0)
            throw new Exception("Invalid Course Part.");
        if (model.SubjectId.ToInt() <= 0)
            throw new Exception("Invalid Subject.");
        if (model.PageSize.ToInt() <= 0)
            throw new Exception("Invalid Page Size.");
        if (model.CategoryId.ToInt() <= 0)
            throw new Exception("Invalid Category.");

        var instance = _coreService.Tbl_SYS_INST_Repository.FirstOrDefault(p =>
            p.Num_PK_INST_SRNO == model.InstanceId, p => p);
        if (null == instance)
            throw new Exception($"Instance {model.InstanceId} not found.");
        
        var course = _coreService.Tbl_COURSE_MSTR_Repository.Get(c => c.Num_PK_CO_CD == model.CourseId).FirstOrDefault();
        if (null == course)
            throw new Exception($"Course {model.CourseId} not found.");
    }

    /*private void ValidateFields(MarksEntry model, string submitType)
    {
        if (model.CollegeId.ToInt() <= 0)
            throw new Exception("Invalid College.");
        if (model.CourseTypeId.ToInt() <= 0)
            throw new Exception("Invalid Course Type.");
        if (model.CourseId.ToInt() <= 0)
            throw new Exception("Invalid Course.");
        if (model.CoursePartId.ToInt() <= 0)
            throw new Exception("Invalid Course Part.");
        if (model.SubjectId.ToInt() <= 0)
            throw new Exception("Invalid Subject.");
        if (model.CategoryId.ToInt() <= 0)
            throw new Exception("Invalid Category.");
        var papers = _coreService.Tbl_SUB_CATPAP_MSTR_Repository.Get(c =>
            c.Num_FK_COPRT_NO == model.CoursePartId && c.Num_FK_SUB_CD == model.SubjectId && c.Num_FK_CAT_CD == model.CategoryId).ToList();
        if (papers.Count > 0 && model.PaperId.ToInt() <= 0)
            throw new Exception("Invalid Paper.");

        switch (submitType)
        {
            case ModelConstants.Search:
                break;
            default:
                // Validations
                if (model.MarksEntryDetails.Count <= 0)
                    throw new Exception("At least one seat no should have marks filled.");
                var extraMarkEntry = model.MarksEntryDetails.FirstOrDefault(d => d.Marks?.ToDouble() > d.MaxMarks && d.Marks != "999");
                if (null != extraMarkEntry)
                    throw new Exception($"Seat No {extraMarkEntry.SeatNo} marks {extraMarkEntry.Marks} have exceeded max marks {extraMarkEntry.MaxMarks}.");
                var blankMarkEntry = model.MarksEntryDetails.FirstOrDefault(d => string.IsNullOrEmpty(d.Marks));
                if (null != blankMarkEntry)
                    throw new Exception($"Seat No {blankMarkEntry.SeatNo} marks cannot be blank.");
                break;
        }
    }

    private void UpdateUserDetails(MarksEntry marksEntry, AspNetUser user)
    {
        //var userId = user.Identity.GetUserId();
        marksEntry.ModifiedBy = user.Id;
        marksEntry.ModifiedDate = DateTime.Now;

        marksEntry.MarksEntryDetails.ForEach(t => t.ModifiedBy = user.Id);
        marksEntry.MarksEntryDetails.ForEach(t => t.ModifiedDate = DateTime.Now);
    }

    private void AddInExamServer(MarksEntry model, AspNetUser user)
    {
        if (null == model.CoursePartId)
            throw new Exception("Invalid course part.");

        var instanceId = model.InstanceId; // HttpContext.Session[ModelConstants.InstanceId].ToInt();
        foreach (var detail in from detail in model.MarksEntryDetails
                               let found = _coreService.Tbl_MARKS_TMP_Repository.Get(t => t.Num_FK_INST_NO == model.InstanceId &&
                         t.Num_FK_COL_CD == (model.CollegeId ?? 0) &&
                         t.Chr_FK_COPRT_NO == model.CoursePartId.ToString() &&
                         t.Chr_FK_SUB_CD == model.SubjectId.ToString() &&
                         t.Chr_CODE_SEAT_NO == detail.SeatNo &&
                         t.Chr_FK_CAT_CD == (model.CategoryId ?? 0).ToString() &&
                         t.Chr_FK_PAP_CD == (model.PaperId ?? 0).ToString())
                     .FirstOrDefault()
                               where null == found
                               select detail)
        {
            const string absentMarks = "999";
            var status = "P";
            if (detail.Marks == absentMarks)
            {
                status = "A";
                detail.Marks = "0";
            }
            var marksTemp = new Tbl_MARKS_TMP
            {
                Num_FK_INST_NO = (short)instanceId,
                Num_FK_COL_CD = (short)(model.CollegeId ?? 0),
                Num_FK_DISTCOL_CD = (short)(model.CentreId ?? 0),
                Chr_FK_COPRT_NO = model.CoursePartId.ToString(),
                Num_FORM_NO = 1,
                Num_FORM_SRNO = 1,
                Chr_FK_BRANCH_CD = model.BranchId,
                Chr_FK_SUB_CD = model.SubjectId.ToString(),
                Chr_FK_CAT_CD = model.CategoryId.ToString(),
                Chr_FK_PAP_CD = (model.PaperId ?? 0).ToString(),
                Chr_FK_SEC_CD = "0",

                Chr_CODE_SEAT_NO = detail.SeatNo ?? 0,
                Chr_MARKS = detail.Marks,
                Chr_MARKS2 = null,

                Chr_EXAMINER_CD = "1",
                Chr_HD_EXAMINER_CD = "1",

                Chr_STATUS_FLG = status,
                Num_ENTRY_NO = 1,
                Num_EVAL_NO = 1,
                Var_BUNDLE_NO = model.SubjectId.ToString(),
                Var_CHKLIST_FLG = "A",
                Num_Application_No = 1,

                Var_USR_NM = user.UserName,
                Dtm_DTE_CR = DateTime.Now,
                Dtm_DTE_UP = DateTime.Now

            };
            _coreService.Tbl_MARKS_TMP_Repository.Add(marksTemp);
        }

        _coreService.Save();
    }*/

    /*private ApiResponse ReadApiEklavya(MarksApiViewModel marksApiViewModel)
    {
        using var client = new HttpClient();
        const string authToken = "qc4aw-lb9su-vrt2j-op1xz";

        client.BaseAddress = new Uri(@"http://osmapi.eklavvya.in/OSM/GetBVDUResult");
        client.DefaultRequestHeaders.Add("AuthenticationKey", authToken);

        /*var requestData = new
        {
            CourseCode = "637",
            AcademicSession = "2023-24",
            SubjectCode = "16858-KAYACHIKITSA (Paper1)"
        };#1#

        var requestData = new
        {
            CourseCode = marksApiViewModel.CourseId.ToString(),
            AcademicSession = marksApiViewModel.InstanceName,
            SubjectCode = marksApiViewModel.SubjectName
        };

        var json = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            //var response = await client.PostAsync("/endpoint", content);
            var response = client.PostAsync(client.BaseAddress, content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;

                // Deserialize the JSON response
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                /#1#/ Process the deserialized response
                Console.WriteLine($@"Response code: {apiResponse.ResponseCode}");
                Console.WriteLine($@"Message: {apiResponse.Message}");
                Console.WriteLine($@"Number of records: {apiResponse.NumberOfRecords}");

                foreach (var item in apiResponse.Data)
                {
                    Console.WriteLine($@"Roll No: {item.RollNo}");
                    Console.WriteLine($@"Part 1 Marks: {item.Part1Marks}");
                    Console.WriteLine($@"Part 2 Marks: {item.Part2Marks}");
                    Console.WriteLine($@"Total Score: {item.TotalScore}");
                }#1#
                return apiResponse;
            }
            else
            {
                throw new Exception("Request failed with status code: " + response.StatusCode);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }*/
    #endregion

    #region -- Public Methods --


    /*public void GetFormSeatNos(MarksEntry model)
    {
        // Validate Fields
        //ValidateFields(model);

        var marksEntryDetails = GetDetails(model);

        var marksTemps = _coreService.Tbl_MARKS_TMP_Repository.Get(t => t.Num_FK_INST_NO == model.InstanceId &&
                                                                        t.Num_FK_COL_CD == (model.CollegeId ?? 0) && t.Chr_FK_COPRT_NO ==
                                                                        model.CoursePartId.ToString() && t.Chr_FK_SUB_CD == model.SubjectId.ToString() &&
                                                                        t.Chr_FK_CAT_CD == (model.CategoryId ?? 0).ToString() && t.Chr_FK_PAP_CD == (model.PaperId ?? 0).ToString())
            .Select(m => m.Chr_CODE_SEAT_NO).ToList();

        marksEntryDetails = marksEntryDetails.Where(m => !marksTemps.Contains(m.SeatNo ?? 0)).ToList();

        if (marksEntryDetails.Count <= 0)
            throw new Exception("No students found for marks entry.");

        model.MarksEntryDetails.AddRange(marksEntryDetails);
    }*/

    public MarksApiDto ImportFromMarksApi(MarksApiViewModel marksApiViewModel)
    {
        var result = new MarksApiDto();
        
        try
        {
            // Validate fields
            ValidateFieldsForApi(marksApiViewModel);

            // Get API key from AppSettings
            var apiKey = ConfigurationManager.AppSettings["MarksApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Marks API Key is not configured in AppSettings.");

            // Get CourseNo from CourseId
            var course = _coreService.Tbl_COURSE_MSTR_Repository.Get(c => c.Num_PK_CO_CD == marksApiViewModel.CourseId).FirstOrDefault();
            if (null == course)
                throw new Exception($"Course {marksApiViewModel.CourseId} not found.");

            var courseNo = course.Num_PK_CO_CD;

            // Fetch data from API
            var apiResponse = FetchMarksFromApi(courseNo, marksApiViewModel.PageSize ?? 120, apiKey, 1);
            
            result.TotalRecordsFetched = apiResponse.Data?.Count ?? 0;
            LogHandler.LogInfo($"Marks API: Fetched {result.TotalRecordsFetched} records from API for CourseNo: {courseNo}");

            if (apiResponse.Data == null || apiResponse.Data.Count == 0)
            {
                result.Message = "No data found in API response.";
                return result;
            }

            // Convert and save to Tbl_MARKS_TMP
            var instanceId = marksApiViewModel.InstanceId ?? 0;
            var insertedCount = 0;
            var updatedCount = 0;

            //apiResponse.Data = apiResponse.Data.Take(5).ToList();
            foreach (var apiItem in apiResponse.Data)
            {
                try
                {
                    // Validate data
                    if (!apiItem.SeatNo.HasValue || apiItem.SeatNo.Value <= 0)
                    {
                        LogHandler.LogInfo($"Marks API: Skipping record with invalid SeatNo: {apiItem.SeatNo}");
                        continue;
                    }

                    if (string.IsNullOrEmpty(apiItem.Marks.ToString()))
                    {
                        LogHandler.LogInfo($"Marks API: Skipping record with empty marks for SeatNo: {apiItem.SeatNo}");
                        continue;
                    }

                    // Check if record already exists
                    var existingRecord = _coreService.Tbl_MARKS_TMP_Repository.Get(t =>
                        t.Num_FK_INST_NO == instanceId &&
                        t.Num_FK_COL_CD == (marksApiViewModel.CollegeId ?? 0) &&
                        t.Chr_FK_COPRT_NO == marksApiViewModel.CoursePartId.ToString() &&
                        t.Chr_FK_SUB_CD == marksApiViewModel.SubjectId.ToString() &&
                        t.Chr_CODE_SEAT_NO == apiItem.SeatNo.Value)
                        .FirstOrDefault();

                    var status = "P";
                    var marks = apiItem.Marks.Split('.').FirstOrDefault();
                    LogHandler.LogInfo($"Marks, Orig : {apiItem.Marks}, Converted : {marks}");
                    /*var marksStr = marks.ToString();
                    if (marksStr == "999" || marksStr == "ABS" || marksStr.ToUpper() == "ABSENT")
                    {
                        status = "A";
                        marks = "0";
                    }*/

                    // Marks should be less than 3 digits;
                    if (existingRecord != null)
                    {
                        // Update existing record
                        existingRecord.Chr_MARKS = marks;
                        existingRecord.Chr_STATUS_FLG = status;
                        existingRecord.Dtm_DTE_UP = DateTime.Now;
                        _coreService.Tbl_MARKS_TMP_Repository.Update(existingRecord);
                        updatedCount++;
                    }
                    else
                    {
                        // Insert new record
                        var marksTemp = new Tbl_MARKS_TMP
                        {
                            Num_FK_INST_NO = (short)instanceId,
                            Num_FK_COL_CD = (short)(marksApiViewModel.CollegeId ?? 0),
                            
                            Num_FK_DISTCOL_CD = (short)apiItem.CenterId.ToUShort(),
                            Chr_FK_COPRT_NO = marksApiViewModel.CoursePartId.ToString(),
                            
                            Chr_FK_SUB_CD = apiItem.SubjectCode,
                            Chr_FK_CAT_CD = marksApiViewModel.CategoryId?.ToString(),
                            Chr_FK_PAP_CD = null != marksApiViewModel.PaperId ? marksApiViewModel.PaperId?.ToString() : "0",
                            Chr_FK_SEC_CD = "0",

                            Num_FORM_NO = 1,
                            Num_FORM_SRNO = 1,

                            Chr_CODE_SEAT_NO = apiItem.SeatNo.Value,

                            Chr_MARKS = marks,
                            Chr_MARKS2 = null,
                            Chr_EXAMINER_CD = "1",
                            Chr_HD_EXAMINER_CD = "1",
                            Chr_STATUS_FLG = status,
                            Num_ENTRY_NO = 1,
                            Num_EVAL_NO = 1,
                            Var_BUNDLE_NO = marksApiViewModel.SubjectId.ToString(),
                            Var_CHKLIST_FLG = "A",
                            Num_Application_No = 1,
                            Var_USR_NM = "API",
                            Dtm_DTE_CR = DateTime.Now,
                            Dtm_DTE_UP = DateTime.Now
                        };
                        _coreService.Tbl_MARKS_TMP_Repository.Add(marksTemp);
                        insertedCount++;
                    }

                    // Add to result for display
                    result.ImportedMarks.Add(new MarksApiDetailDto
                    {
                        SeatNo = apiItem.SeatNo.Value,
                        Marks = marks,
                        Status = status,
                        Prn = apiItem.Prn
                    });
                }
                catch (Exception ex)
                {
                    LogHandler.LogError(new Exception($"Error processing record with SeatNo: {apiItem.SeatNo}", ex));
                }
            }

            // Save all changes
            _coreService.Save();

            result.TotalRecordsInserted = insertedCount;
            result.TotalRecordsUpdated = updatedCount;
            result.Message = $"Successfully imported {insertedCount} new records and updated {updatedCount} existing records.";

            LogHandler.LogInfo($"Marks API: Inserted {insertedCount} records, Updated {updatedCount} records in Tbl_MARKS_TMP");

            return result;
        }
        catch (Exception ex)
        {
            LogHandler.LogError(ex);
            result.Message = $"Error importing marks: {ex.Message}";
            throw;
        }
    }

    private MarksApiResponse FetchMarksFromApi(int courseNo, int pageSize, string apiKey, int page = 1)
    {
        using var client = new HttpClient();
        
        var baseUrl = "http://27.107.189.142:1594/api/v1/marks";
        var url = $"{baseUrl}?courseNo={courseNo}&page={page}&pageSize={pageSize}&apikey={Uri.EscapeDataString(apiKey)}";

        try
        {
            var response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                LogHandler.LogInfo($"Marks API Response: {responseContent}");

                // Deserialize the JSON response
                var apiResponse = JsonConvert.DeserializeObject<MarksApiResponse>(responseContent);
                return apiResponse ?? new MarksApiResponse { Data = new List<MarksApiDataItem>() };
            }
            else
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                LogHandler.LogError(new Exception($"Marks API request failed with status code: {response.StatusCode}. Response: {errorContent}"));
                throw new Exception($"Request failed with status code: {response.StatusCode}. {errorContent}");
            }
        }
        catch (Exception e)
        {
            LogHandler.LogError(e);
            throw new Exception($"Error fetching marks from API: {e.Message}", e);
        }
    }

    /*public void Save(MarksEntry model, string submitType, AspNetUser user)
    {
        // Validate fields in model
        ValidateFields(model, submitType);

        // Check whether extra marks entries are added.
        var newModel = new MarksEntry
        {
            InstanceId = model.InstanceId,
            CollegeId = model.CollegeId,
            CentreId = model.CentreId,
            CourseTypeId = model.CourseTypeId,
            CourseId = model.CourseId,
            CoursePartId = model.CoursePartId,
            SubjectId = model.SubjectId,
            CategoryId = model.CategoryId,
            PaperId = model.PaperId
        };
        newModel.MarksEntryDetails.Clear();
        GetFormSeatNos(newModel);
        var seatNos = newModel.MarksEntryDetails.Select(d => d.SeatNo).Distinct();
        // Remove from model
        model.MarksEntryDetails.RemoveAll(d => !seatNos.Contains(d.SeatNo));

        //using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout });
        /*try
        {#1#
        UpdateUserDetails(model, user);

        // Add Student To Exam Server
        AddInExamServer(model, user);

        //scope.Complete();
        /*}
        catch
        {
            scope.Dispose();
            throw;
        }#1#
    }

    public void GetReportSeatNos(MarksEntry model)
    {
        var marksEntryDetails = GetDetails(model);

        var marksTemps = _coreService.Tbl_MARKS_TMP_Repository.Get(t => t.Num_FK_INST_NO == model.InstanceId &&
                                                                        t.Num_FK_COL_CD == (model.CollegeId ?? 0) && t.Chr_FK_COPRT_NO ==
                                                                        model.CoursePartId.ToString() && t.Chr_FK_SUB_CD == model.SubjectId.ToString() &&
                                                                        t.Chr_FK_CAT_CD == (model.CategoryId ?? 0).ToString() &&
                                                                        t.Chr_FK_PAP_CD == (model.PaperId ?? 0).ToString() &&
                                                                        t.Var_CHKLIST_FLG == "A" && t.Num_Application_No == 1)
            .Select(m => new { m.Chr_CODE_SEAT_NO, m.Chr_MARKS, m.Chr_STATUS_FLG }).ToList();

        foreach (var marksEntryDetail in marksEntryDetails)
        {
            var tempDetail = marksTemps.FirstOrDefault(t => t.Chr_CODE_SEAT_NO == marksEntryDetail.SeatNo);
            if (null == tempDetail) continue;

            marksEntryDetail.Status = tempDetail.Chr_STATUS_FLG;
            marksEntryDetail.Marks = tempDetail.Chr_MARKS;
        }

        model.MarksEntryDetails.AddRange(marksEntryDetails);
    }*/
}
    #endregion