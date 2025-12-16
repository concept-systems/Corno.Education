using Corno.Data.Common;
using Mapster;
using System;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class StudentAddressDetail : BaseModel
{
    #region -- Constructors -- 
    public StudentAddressDetail()
    {
    }
    #endregion

    #region -- Properties --
    public  int? StudentId { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string Pin { get; set; }
    public string Phone { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }

    [AdaptIgnore]
    protected virtual Student Student { get; set; }
    #endregion

    #region -- Public Methods --
    public void Copy(StudentAddressDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        Address = other.Address;
        City = other.City;
        State = other.State;
        Country = other.Country;
        Pin = other.Pin;
        Phone = other.Phone;
        Mobile = other.Mobile;
        Email = other.Email;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}