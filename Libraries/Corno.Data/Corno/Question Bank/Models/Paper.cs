using Corno.Data.Common;
using Corno.Data.Corno.Masters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Corno.Data.Corno.Question_Bank.Models;

[Serializable]
public class Paper : QuestionBankModel
{
    #region -- Constructors -- 
    public Paper()
    {
        EnableHeader = true;

        PaperDetails = new List<PaperDetail>();
    }
    #endregion
    public int? InstanceId { get; set; }
    public string PaperType { get; set; }
    public int? MaxMarks { get; set; }
    public int? NoOfSections { get; set; }
    public int? NoOfChapters { get; set; }
    public int? NoOfQuestions { get; set; }
    public byte[] DocumentContent { get; set; }

    [NotMapped]
    public bool EnableHeader { get; set; }
    [NotMapped]
    public int QuestionSerialNo { get; set; }
    [NotMapped]
    public int? DifficultyLevel { get; set; }
    [NotMapped]
    public int? LearningPriorityId { get; set; }
    [NotMapped]
    public int CoNo { get; set; }
    [NotMapped]
    public PaperDetail QuestionInfo { get; set; }


    public List<PaperDetail> PaperDetails { get; set; }

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not Paper newPaper) return false;

        foreach (var paperDetail in PaperDetails)
        {
            var newPaperDetail = newPaper.PaperDetails.FirstOrDefault(d =>
                d.Id == paperDetail.Id);
            paperDetail.Copy(newPaperDetail);
        }

        // Add new entries
        var newPaperDetails = newPaper.PaperDetails.Where(d => d.Id <= 0).ToList();
        PaperDetails.AddRange(newPaperDetails);

        // Remove items from list1 that are not in list2
        PaperDetails.RemoveAll(x => newPaper.PaperDetails.All(y => y.Id != x.Id));

        return true;
    }
    #endregion
}