using Corno.Globals.Constants;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Corno.Globals;

namespace Corno.Data.Common;

[Serializable]
public class BaseModel
{
    #region -- Constructors --
    public BaseModel()
    {
        Reset();
    }
    #endregion

    #region -- Properties --
    [DisplayName("Company")]
    public int? CompanyId { get; set; }
    [DisplayName("Serial No")]
    public int? SerialNo { get; set; }
    public string Code { get; set; }
    [Key]
    public int? Id { get; set; }
    public string Status { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
    //public virtual ICollection<string> Statuses { get; set; }
    #endregion

    #region -- Public Methods --

    public SessionData GetSessionData()
    {
        return HttpContext.Current.Session[HttpContext.Current.User.Identity.Name] as SessionData;
    }

    public virtual void Reset()
    {
        CompanyId = 1;
        SerialNo = 0;
        Id = 0;
        Status = StatusConstants.Active;

        AddUserData();
    }

    public void AddUserData()
    {
        var sessionData = GetSessionData();
        if (null == sessionData) return;
        CreatedBy = sessionData.UserId;
        CreatedDate = DateTime.Now;
        ModifiedBy = sessionData.UserId;
        ModifiedDate = DateTime.Now;
    }

    public void UpdateUserData()
    {
        var sessionData = GetSessionData();
        if (null == sessionData) return;
        ModifiedBy = sessionData.UserId;
        ModifiedDate = DateTime.Now;
    }

    public virtual bool UpdateDetails(BaseModel newModel)
    {
        return false;
    }

    public virtual void Copy(BaseModel other)
    {
        if (other == null) return;

        CompanyId = other.CompanyId;
        SerialNo = other.SerialNo;
        Code = other.Code;
        //Id = other.Id;
        Status = other.Status;
        CreatedBy = other.CreatedBy;
        CreatedDate = other.CreatedDate;
        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
        DeletedBy = other.DeletedBy;
        DeletedDate = other.DeletedDate;
    }
    #endregion
}