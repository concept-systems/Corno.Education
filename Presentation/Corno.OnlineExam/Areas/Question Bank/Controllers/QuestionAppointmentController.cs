using Corno.Data.Corno.Question_Bank;
using Corno.Data.Corno.Question_Bank.Dtos;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.OnlineExam.Attributes;
using Corno.OnlineExam.Controllers;
using Corno.Services.Corno.Question_Bank.Interfaces;
using Mapster;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Web.Mvc;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Services.Bootstrapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Exception = System.Exception;

namespace Corno.OnlineExam.Areas.Question_Bank.Controllers;

[Authorize]
public class QuestionAppointmentController : CornoController
{
    #region -- Constructors --
    public QuestionAppointmentController(IQuestionAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;

        TypeAdapterConfig<QuestionAppointmentDto, QuestionAppointment>
            .NewConfig()
            .Map(dest => dest.QuestionAppointmentDetails,
                src => src.QuestionAppointmentDetailDtos.Adapt<List<QuestionAppointmentDetail>>())
            .Map(dest => dest.QuestionAppointmentTypeDetails,
            src => src.QuestionAppointmentTypeDetailDtos.Adapt<List<QuestionAppointmentTypeDetail>>());
        TypeAdapterConfig<QuestionAppointment, QuestionAppointmentDto>
            .NewConfig()
            .Map(dest => dest.QuestionAppointmentDetailDtos,
                src => src.QuestionAppointmentDetails.Adapt<List<QuestionAppointmentDetail>>())
            .Map(dest => dest.QuestionAppointmentTypeDetailDtos,
                src => src.QuestionAppointmentTypeDetails.Adapt<List<QuestionAppointmentTypeDetail>>());

        var path = "~/Areas/Question Bank/views/QuestionAppointment/";
        _indexPath = $"{path}Index.cshtml";
        _createPath = $"{path}Create.cshtml";
        _editPath = $"{path}Edit.cshtml";
        //_billPath = $"{path}Bill.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IQuestionAppointmentService _appointmentService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    #endregion

    #region -- Private Methods --


    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Index()
    {
        return View(_indexPath);
    }

    [Authorize]
    public ActionResult Create()
    {
        var sessionData = Session[User.Identity.Name] as SessionData;
        var dto = new QuestionAppointmentDto
        {
            InstanceId = sessionData?.InstanceId,
            CollegeId = sessionData?.CollegeId,
        };

        return View(_createPath, dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [MultipleButton(Name = "action", Argument = "Save")]
    public ActionResult Create(QuestionAppointmentDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);

        try
        {
            // Validate fields
            _appointmentService.ValidateDto(dto);

            // Remove non selected entries
            dto.QuestionAppointmentDetailDtos.RemoveAll(d => d.IsSetter != true && d.IsChecker != true);

            var appointment = _appointmentService.GetExisting(dto);
            if (null != appointment)
            {
                // Update existing appointment
                dto.Adapt(appointment);
                _appointmentService.UpdateAndSave(appointment);
            }
            else
            {
                // Create new appointment
                appointment = dto.Adapt<QuestionAppointment>();
                // Update question type details
                var structureService = Bootstrapper.Get<IStructureService>();
                var structures = structureService.Get(p => p.SubjectId == dto.SubjectId, p => p)
                    .ToList();

                if (!structures.Any())
                    throw new Exception($"No structure found for subjectId: {dto.SubjectId}");
                foreach (var structure in structures)
                {
                    var groups = structure.StructureDetails.GroupBy(p => p.QuestionTypeId);
                    foreach (var group in groups)
                    {
                        // Add question appointment type detail
                        appointment.QuestionAppointmentTypeDetails.Add(
                            new QuestionAppointmentTypeDetail
                            {
                                PaperCategoryId = structure.PaperCategoryId,
                                QuestionTypeId = group.FirstOrDefault()?.QuestionTypeId,
                                QuestionCount = group.Sum(d => d.NofOptions) * appointment.SetsToBeDrawn,
                            });
                    }
                }
                _appointmentService.AddAndSave(appointment);
            }

            // Clear
            dto.SetsToBeDrawn = 0;
            dto.QuestionAppointmentDetailDtos.Clear();

            ModelState.Clear();
            TempData["Success"] = "Saved successfully.";
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, dto);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "GetStaffs")]
    public ActionResult GetStaffs(QuestionAppointmentDto dto)
    {
        if (!ModelState.IsValid || null == dto)
            return View(_createPath, dto);

        try
        {
            if (dto.InstanceId <= 0)
                dto.InstanceId = Session[User.Identity.Name] is SessionData sessionData ? sessionData.InstanceId : 0;

            // Get all Appointment subjects
            _appointmentService.GetStaffs(dto);

            ModelState.Remove(ModelConstants.Id);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, dto);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Send")]
    public ActionResult Send(QuestionAppointmentDto dto, string type)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);

        try
        {
            _appointmentService.SendEmailSms(dto, type);

            TempData["Success"] = "Sent successfully.";
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, dto);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, QuestionAppointmentDetailDto model)
    {
        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    #endregion
}