using Corno.Data.Corno.Masters;
using Corno.Services.Corno.Masters.Interfaces;

namespace Corno.Services.Corno.Masters;

public class StudentService : MainMasterService<Student>, IStudentService
{
    #region -- Constructors --
    public StudentService()
    {
        SetIncludes(nameof(Student.StudentAddressDetails));
    }
    #endregion
}