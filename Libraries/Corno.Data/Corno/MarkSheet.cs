using Corno.Data.Common;

namespace Corno.Data.Corno;

public class MarkSheetCommon : BaseModel
{
    public string PrnNo { get; set; }
    public string Otp { get; set; }
    public string FileName { get; set; }
}

public class MarkSheet : MarkSheetCommon
{
}