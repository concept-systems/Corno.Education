using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank.Models;

[Serializable]
public class QuestionAppointment : BaseModel
{
    #region -- Constructors --

    #endregion

    public int? InstanceId { get; set; }
    public int? FacultyId { get; set; }
    public int? CollegeId { get; set; }
    public int? CentreId { get; set; }
    public int? CourseId { get; set; }
    public int? CoursePartId { get; set; }
    public int? CourseTypeId { get; set; }
    public int? BranchId { get; set; }

    public int? CategoryId { get; set; }
    public int? SubjectId { get; set; }

    public int? SetsToBeDrawn { get; set; } = 0;

    public virtual List<QuestionAppointmentDetail> QuestionAppointmentDetails { get; set; } = new();
    public virtual List<QuestionAppointmentTypeDetail> QuestionAppointmentTypeDetails { get; set; } = new();

    #region -- Private Methods --

    private void UpdateAppointmentDetails(QuestionAppointment newAppointment)
    {
        foreach (var detail in QuestionAppointmentDetails)
        {
            var newDetail = newAppointment.QuestionAppointmentDetails.FirstOrDefault(d =>
                d.Id == detail.Id);
            detail.Copy(newDetail);
        }

        // Add new entries
        var newAppointmentDetails = newAppointment.QuestionAppointmentDetails.Where(d => d.Id <= 0).ToList();
        QuestionAppointmentDetails.AddRange(newAppointmentDetails);

        // Delete existing entries
        QuestionAppointmentDetails.RemoveAll(x => newAppointment.QuestionAppointmentDetails.All(y => 
            y.Id != x.Id));
    }

    private void UpdateAppointmentTypeDetails(QuestionAppointment newAppointment)
    {
        foreach (var detail in QuestionAppointmentTypeDetails)
        {
            var newDetail = newAppointment.QuestionAppointmentTypeDetails.FirstOrDefault(d =>
                d.Id == detail.Id);
            detail.Copy(newDetail);
        }

        // Add new entries
        var newAppointmentTypeDetails = newAppointment.QuestionAppointmentTypeDetails.Where(d => d.Id <= 0).ToList();
        QuestionAppointmentTypeDetails.AddRange(newAppointmentTypeDetails);

        // Delete existing entries
        QuestionAppointmentTypeDetails.RemoveAll(x => newAppointment.QuestionAppointmentTypeDetails.All(y =>
            y.Id != x.Id));
    }
    #endregion

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not QuestionAppointment newAppointment) return false;

        // Update appointment details
        UpdateAppointmentDetails(newAppointment);

        // Update appointment type detail
        UpdateAppointmentTypeDetails(newAppointment);

        return true;
    }
    #endregion
}