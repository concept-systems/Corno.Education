using Corno.Data.Common;
using Corno.Data.Corno.Masters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Corno.Data.Corno.Question_Bank.Models;

[Serializable]
public class Structure : QuestionBankModel
{
    #region -- Constructors -- 
    public Structure()
    {
        EnableHeader = true;

        StructureDetails = new List<StructureDetail>();
    }
    #endregion

    /*[Required(ErrorMessage = "Max marks is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than zero")]*/
    public int? MaxMarks { get; set; }
    /*[Required(ErrorMessage = "No. of sections is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than zero")]*/
    public int? NoOfSections { get; set; }
    //[Required(ErrorMessage = "No. of chapters is required")]
    //[Range(1, int.MaxValue, ErrorMessage = "Value must be greater than zero")]
    public int? NoOfChapters { get; set; }
    //[Required(ErrorMessage = "No. of questions is required")]
    //[Range(1, int.MaxValue, ErrorMessage = "Value must be greater than zero")]
    public int? NoOfQuestions { get; set; }

    [NotMapped]
    public bool EnableHeader { get; set; }


    public List<StructureDetail> StructureDetails { get; set; }

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not Structure newStructure) return false;

        var toDelete = new List<StructureDetail>();
        foreach (var structureDetail in StructureDetails)
        {
            var newStructureDetail = newStructure.StructureDetails.FirstOrDefault(d =>
                d.Id == structureDetail.Id);
            structureDetail.Copy(newStructureDetail);
        }

        // Add new entries
        var newStructureDetails = newStructure.StructureDetails.Where(d => d.Id <= 0).ToList();
        StructureDetails.AddRange(newStructureDetails);

        // Remove items from list1 that are not in list2
        StructureDetails.RemoveAll(x => newStructure.StructureDetails.All(y => y.Id != x.Id));

        return true;
    }

    public override void Copy(BaseModel other)
    {
        if (other is not Structure otherStructure) return;

        base.Copy(other);

        PaperCategoryId = otherStructure.PaperCategoryId;
        MaxMarks = otherStructure.MaxMarks;
        NoOfSections = otherStructure.NoOfSections;
        NoOfChapters = otherStructure.NoOfChapters;
        NoOfQuestions = otherStructure.NoOfQuestions;

        foreach (var structureDetail in otherStructure.StructureDetails)
        {
            var newStructureDetail = new StructureDetail();
            newStructureDetail.Copy(structureDetail);

            StructureDetails.Add(newStructureDetail);
        }
    }
    #endregion
}