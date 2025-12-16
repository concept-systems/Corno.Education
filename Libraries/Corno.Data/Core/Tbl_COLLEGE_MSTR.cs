using System;
using System.Collections.Generic;

namespace Corno.Data.Core;

public partial class Tbl_COLLEGE_MSTR
{
    public short Num_PK_COLLEGE_CD { get; set; }
    public short Num_FK_DIST_CD { get; set; }
    public string Var_CL_COLLEGE_NM1 { get; set; }
    public string Var_CL_COLLEGE_NM2 { get; set; }
    public string Var_CL_SHRT_NM { get; set; }
    public string Var_CL_COLLEGE_ADD1 { get; set; }
    public string Var_CL_COLLEGE_ADD2 { get; set; }
    public string Var_CL_COLLEGE_ADD3 { get; set; }
    public string Var_CL_CITY_NM { get; set; }
    public string Var_CL_PIN_CD { get; set; }
    public string Var_CL_PH1 { get; set; }
    public string Var_CL_PH2 { get; set; }
    public string Var_CL_PH3 { get; set; }
    public string Var_CL_FAX { get; set; }
    public string Var_CL_E_MAIL { get; set; }
    public string Var_CL_WEB { get; set; }
    public short? Num_CL_EXM_CAPACITY { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public short? Solapur_data_Org_College_CD { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string Chr_DISTANCE_EDU { get; set; }
    public string Num_BankACNo { get; set; }
    public string Chr_BankBranch_Code { get; set; }

    public Tbl_COLLEGE_MSTR()
    {
        this.ConvocationFees = new List<ConvocationFee>();
    }
    public virtual ICollection<ConvocationFee> ConvocationFees { get; set; }
}