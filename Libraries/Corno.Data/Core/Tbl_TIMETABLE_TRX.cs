using System;

namespace Corno.Data.Core;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Tbl_TIMETABLE_TRX
{
    [Key, Column(Order = 0)]
    public short? Num_FK_COPRT_NO { get; set; }

    [Key, Column(Order = 1)]
    public short? Num_FK_Course_No { get; set; }

    [Key, Column(Order = 2)]
    public int? Num_FK_PH_CD { get; set; }

    [Key, Column(Order = 3)]
    public short? Num_FK_CAT_CD { get; set; }

    [Key, Column(Order = 4)]
    public int? Num_FK_PP_CD { get; set; }

    [Key, Column(Order = 5)]
    public short? NUM_FK_SUB_DIV_CD { get; set; }

    [Key, Column(Order = 6)]
    public short? Num_FK_INST_NO { get; set; }

    [Key, Column(Order = 7)]
    public DateTime? Dtm_DTE_CR { get; set; }

    public DateTime? Dtm_TBM_FROM_TIME { get; set; }
    public DateTime? Dtm_TBM_TO_TIME { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string VAR_START_TIME { get; set; }
    public string VAR_TO_TIME { get; set; }
}

/*public class Tbl_TIMETABLE_TRX
{
    public short? Num_FK_COPRT_NO { get; set; }
    public short? Num_FK_Course_No { get; set; }
    public int? Num_FK_PH_CD { get; set; }
    public short? Num_FK_CAT_CD { get; set; }
    public int? Num_FK_PP_CD { get; set; }
    public short? NUM_FK_SUB_DIV_CD { get; set; }
    public DateTime? Dtm_TBM_FROM_TIME { get; set; }
    public DateTime? Dtm_TBM_TO_TIME { get; set; }
    public short? Num_FK_INST_NO { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string VAR_START_TIME { get; set; }
    public string VAR_TO_TIME { get; set; }
}*/