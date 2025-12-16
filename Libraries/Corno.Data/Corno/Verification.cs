using System;
using System.Collections.Generic;
using Corno.Data.Common;
using Corno.Data.Masters;

namespace Corno.Data.Transactions
{
    public partial class VerificationCommon : BaseModel
    {
        
        public int? PRNNo { get; set; }
        public string Name { get; set; }
        public int? BranchID { get; set; }
        public int? CourseID { get; set; }
        public int? CoursePartID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public int? CountryID { get; set; }
        public int? CityID { get; set; }
        public int? TehsilID { get; set; }
        public int? DistrictID { get; set; }
        public int? StateID { get; set; }
        public int? Pincode { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public int? SubjectID { get; set; }
        public string SubjectName { get; set; }
        public Double Fee { get; set; }
        public int? ApplicationNo { get; set; }
        
       
    }
     public partial class Verification : VerificationCommon
    {
        public Verification()
        {

        }

        //public virtual Branch Branch { get; set; }
        public virtual City City { get; set; }
        public virtual Country Country { get; set; }
        //public virtual Course Course { get; set; }
        //public virtual CoursePart CoursePart { get; set; }
        public virtual District District { get; set; }
        public virtual State State { get; set; }
        //public virtual Subject Subject { get; set; }
        public virtual Tehsil Tehsil { get; set; }
    }
    public partial class VerificationViewModel : VerificationCommon
    {
        public VerificationViewModel()
        {

        }
        //public virtual ICollection<Branch> Branchs { get; set; }
        public virtual ICollection<City> Citys { get; set; }
        public virtual ICollection<Country> Countrys { get; set; }
        //public virtual ICollection<Course> Courses { get; set; }
        //public virtual ICollection<CoursePart> CourseParts { get; set; }
        public virtual ICollection<District> Districts { get; set; }
        public virtual ICollection<State> States { get; set; }
        public virtual ICollection<Tehsil> Tehsils { get; set; }
        //public virtual ICollection<Subject> Subjects { get; set; }

    }



}
