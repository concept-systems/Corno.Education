namespace Corno.Data.Common;

public class TransactionModel : BaseModel
{
    public TransactionModel()
    {
        FinancialYearId = 1;
    }

    public int? FinancialYearId { get; set; }
}