using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Common;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class Branch : MasterModel
{
    #region -- Constructors -- 
    public Branch()
    {
        BranchSubjectDetails = new List<BranchSubjectDetail>();
    }
    #endregion

    #region  -- Properties --

    public string ShortName { get; set; }
    public int? SequenceNo { get; set; }

    public int? CourseId { get; set; }
    public string NameBl { get; set; }

    public List<BranchSubjectDetail> BranchSubjectDetails { get; set; }
    #endregion

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not Branch newBranch) return false;

        foreach (var branchDetail in BranchSubjectDetails)
        {
            var newBranchDetail = newBranch.BranchSubjectDetails.FirstOrDefault(d =>
                d.Id == branchDetail.Id);
            branchDetail.Copy(newBranchDetail);
        }

        // Add new entries
        var newBranchDetails = newBranch.BranchSubjectDetails.Where(d => d.Id <= 0).ToList();
        BranchSubjectDetails.AddRange(newBranchDetails);

        // Remove items from list1 that are not in list2
        BranchSubjectDetails.RemoveAll(x => newBranch.BranchSubjectDetails.All(y => y.Id != x.Id));

        return true;
    }
    #endregion
}