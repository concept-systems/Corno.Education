using Corno.Globals.Enums;
using Unity;

namespace Corno.Globals;

public static class GlobalVariables
{
    public static string ConnectionString = string.Empty;
    public static string ConnectionStringExamServer = string.Empty;

    public static IUnityContainer Container = null;
}

public class SessionData
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public int CollegeId { get; set; }
    public string CollegeName { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int CenterId { get; set; }
    public string CenterName { get; set; }
    public int InstanceId { get; set; }
    public string InstanceName { get; set; }
    public string MobileNo { get; set; }
    public string Prn { get; set; }
    public string StudentName { get; set; }

    public UserType UserType { get; set; }
}