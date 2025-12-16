using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using Corno.Data.Common;

namespace Corno.Data.Corno;

[Serializable]
public class MarksEntry : UniversityBaseModel
{
    public MarksEntry()
    {
        bEnable = true;
        MarksEntryDetails = new List<MarksEntryDetail>();
    }

    public int? SubjectId { get; set; }
    public int? CategoryId { get; set; }
    public int? PaperId { get; set; }
    public bool bEnable { get; set; }

    [NotMapped]
    public string Otp { get; set; }
    [NotMapped]
    public string Error { get; set; }

    public HttpPostedFileBase UploadedFile { get; set; }



    public List<MarksEntryDetail> MarksEntryDetails { get; set; }
}