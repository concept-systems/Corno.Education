using Ganss.Excel;

namespace Corno.Data.Import;

public class MarksImportModel
{
    [Column("SrNo")]
    [FormulaResult]
    public int SerialNo { get; set; }
    [Column("Prn")]
    [FormulaResult]
    public string Prn { get; set; }
    [Column("Seat No")]
    [FormulaResult]
    public long? SeatNo { get; set; }
    [Column("Marks")]
    [FormulaResult]
    public string Marks { get; set; }
}