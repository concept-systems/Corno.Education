using Corno.Data.Common;
using Corno.Data.Corno.Question_Bank;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;

namespace Corno.Data.Corno.Paper_Setting.Models;

[Serializable]
public class Appointment : UniversityBaseModel
{
    #region -- Constructors -- 
    public Appointment()
    {
        //EnableHeader = true;

        AppointmentDetails = new List<AppointmentDetail>();
        AppointmentBillDetails = new List<AppointmentBillDetail>();

        AppointmentBillDetail = new AppointmentBillDetail();
    }
    #endregion

    [NotMapped]
    public new int? CentreId { get; set; }
    [NotMapped]
    public new int? CourseTypeId { get; set; }

    public int? CategoryId { get; set; }
    public int? SubjectId { get; set; }


    [NotMapped]
    public int? StaffId { get; set; }
    [NotMapped]
    public AppointmentBillDetail AppointmentBillDetail { get; set; }
    
    public List<AppointmentDetail> AppointmentDetails { get; set; }
    public List<AppointmentBillDetail> AppointmentBillDetails { get; set; }

    #region -- Private Methods --

    private void UpdateAppointmentDetails(Appointment newAppointment)
    {
        foreach (var detail in AppointmentDetails)
        {
            var newDetail = newAppointment.AppointmentDetails.FirstOrDefault(d =>
                d.StaffId == detail.StaffId);
            if (null != newDetail)
                detail.Copy(newDetail);
        }

        // Add new entries
        var newAppointmentDetails = newAppointment.AppointmentDetails.Where(d => d.Id <= 0).ToList();
        AppointmentDetails.AddRange(newAppointmentDetails);

        // Delete existing entries
        AppointmentDetails.RemoveAll(x => newAppointment.AppointmentDetails.All(y =>
            y.Id != x.Id));
    }

    // Update bill details
    private void UpdateBillDetails(Appointment newAppointment)
    {
        foreach (var detail in AppointmentBillDetails)
        {
            var newDetail = newAppointment.AppointmentBillDetails.FirstOrDefault(d =>
                d.StaffId == detail.StaffId);
            if (null != newDetail)
                detail.Copy(newDetail);
        }

        // Add new entries
        var newBillDetails = newAppointment.AppointmentBillDetails.Where(d => d.Id <= 0).ToList();
        AppointmentBillDetails.AddRange(newBillDetails);

        // Delete existing entries
        AppointmentBillDetails.RemoveAll(x => newAppointment.AppointmentBillDetails.All(y =>
            y.Id != x.Id));
    }
    #endregion

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not Appointment newAppointment) return false;

        // Update appointment details
        UpdateAppointmentDetails(newAppointment);

        // Update bill details
        UpdateBillDetails(newAppointment);

        return true;
    }

    /*private Expression<Func<Appointment, bool>> GetPredicate(this Appointment appointment, List<int> subjectIds = default)
    {
        Expression<Func<Appointment, bool>> predicate = p =>
            p.InstanceId == appointment.InstanceId;// && p.AppointmentDetails.Any(d => d.IsPaperSetter);
        if ((appointment.FacultyId ?? 0) > 0)
            predicate = predicate.And(s => s.FacultyId == appointment.FacultyId);
        if ((appointment.CollegeId ?? 0) > 0)
            predicate = predicate.And(s => s.CollegeId == appointment.CollegeId);
        if ((appointment.CourseId ?? 0) > 0)
            predicate = predicate.And(s => s.CourseId == appointment.CourseId);
        if ((appointment.CoursePartId ?? 0) > 0)
            predicate = predicate.And(s => s.CoursePartId == appointment.CoursePartId);
        if ((appointment.BranchId ?? 0) > 0)
            predicate = predicate.And(s => s.BranchId == appointment.BranchId);

        if (subjectIds is { Count: > 0 })
            predicate = predicate.And(s => subjectIds.Contains(s.SubjectId ?? 0));
        else if ((appointment.SubjectId ?? 0) > 0)
            predicate = predicate.And(s => s.SubjectId == appointment.SubjectId);

        if ((appointment.CategoryId ?? 0) > 0)
            predicate = predicate.And(s => s.CategoryId == appointment.CategoryId);

        return predicate;
    }*/
    #endregion
}