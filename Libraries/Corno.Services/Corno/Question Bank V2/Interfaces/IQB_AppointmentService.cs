using Corno.Data.Corno.Question_Bank_V2.Models;
using Corno.Services.Corno.Interfaces;
using System.Collections.Generic;

namespace Corno.Services.Corno.Question_Bank_V2.Interfaces
{
    public interface IQB_AppointmentService : IMainService<QB_Appointment>
    {
        void CreateAppointment(QB_Appointment appointment, string userId);
        void AssignRoles(QB_Appointment appointment, List<int> setterUserIds, List<int> checkerUserIds, List<int> moderatorUserIds);
        void GenerateLoginCredentials(QB_Appointment appointment);
        void SendNotifications(QB_Appointment appointment, string notificationType); // Email, SMS, WhatsApp
        List<QB_Appointment> GetAppointmentsForUser(string userId, int instanceId, string roleName);
        string GenerateAppointmentCode(int instanceId);
        void AcceptAppointment(int appointmentDetailId, string userId);
    }
}
