using System.Collections.Generic;

namespace Corno.Data.Corno;

public class StudentProfile
{
    public StudentProfile()
    {
        StudentProfileDetails = new List<StudentProfileDetail>();
    }

    public string Prn { get; set; }
    public string StudentName { get; set; }
    public string Address { get; set; }
    public string Photo { get; set; }
    public bool MakeDeactive { get; set; }

    public ICollection<StudentProfileDetail> StudentProfileDetails { get; set; }
}