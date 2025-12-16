using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Common;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class Subject : MasterModel
{
    #region -- Properties --
    public string ShortName { get; set; }
    public int? FacultyId { get; set; }
    public int? RecognitionYear { get; set; }
    public int? CourseId { get; set; }
    public int? CoursePartId { get; set; }
    public bool PaperCategoryApplicable { get; set; }
    public int? BosId { get; set; }
    public int? CommonSubjectId { get; set; }
    public int? ExemptionMarks { get; set; }
    public int? Marks { get; set; }
    public int? PassingMarks { get; set; }
    public int? AvailableSets { get; set; }
    public int? StandardSets { get; set; }
    public int? MinGrade { get; set; }
    public string SubjectType { get; set; }
    public string Narration1 { get; set; }
    public string Narration2 { get; set; }

    public List<SubjectCategoryDetail> SubjectCategoryDetails { get; set; } = new();
    public List<SubjectChapterDetail> SubjectChapterDetails { get; set; } = new();
    public List<SubjectInstructionDetail> SubjectInstructionDetails { get; set; } = new();
    public List<SubjectSectionDetail> SubjectSectionDetails { get; set; } = new();
    #endregion

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not Subject newSubject) return false;

        // ** Categories **
        foreach (var detail in SubjectCategoryDetails)
        {
            var newDetail = newSubject.SubjectCategoryDetails.FirstOrDefault(d =>
                d.Id == detail.Id);
            detail.Copy(newDetail);
        }
        // Add new entries
        var newDetails = newSubject.SubjectCategoryDetails.Where(d => d.Id <= 0).ToList();
        SubjectCategoryDetails.AddRange(newDetails);
        // Remove items from list1 that are not in list2
        SubjectCategoryDetails.RemoveAll(x => newSubject.SubjectCategoryDetails.All(y => y.Id != x.Id));

        // ** Chapters **
        foreach (var detail in SubjectChapterDetails)
        {
            var newDetail = newSubject.SubjectChapterDetails.FirstOrDefault(d =>
                d.Id == detail.Id);
            detail.Copy(newDetail);
        }
        // Add new entries
        var newChapterDetails = newSubject.SubjectChapterDetails.Where(d => d.Id <= 0).ToList();
        SubjectChapterDetails.AddRange(newChapterDetails);
        // Remove items from list1 that are not in list2
        SubjectChapterDetails.RemoveAll(x => newSubject.SubjectChapterDetails.All(y => y.Id != x.Id));

        // ** Instructions **
        foreach (var detail in SubjectInstructionDetails)
        {
            var newDetail = newSubject.SubjectInstructionDetails.FirstOrDefault(d =>
                d.Description.Equals(detail.Description));
            detail.Copy(newDetail);
        }
        // Add new entries
        var newInstructionDetails = newSubject.SubjectInstructionDetails.Where(d => d.Id <= 0).ToList();
        SubjectInstructionDetails.AddRange(newInstructionDetails);
        // Remove items from list1 that are not in list2
        SubjectInstructionDetails.RemoveAll(x => newSubject.SubjectInstructionDetails.All(y => !y.Description.Equals(x.Description)));

        // ** Sections **
        foreach (var detail in SubjectSectionDetails)
        {
            var newDetail = newSubject.SubjectSectionDetails.FirstOrDefault(d =>
                d.Id.Equals(detail.Id));
            detail.Copy(newDetail);
        }
        // Add new entries
        var newSectionDetails = newSubject.SubjectSectionDetails.Where(d => d.Id <= 0).ToList();
        SubjectSectionDetails.AddRange(newSectionDetails);
        // Remove items from list1 that are not in list2
        SubjectSectionDetails.RemoveAll(x => newSubject.SubjectSectionDetails.All(y => !y.Id.Equals(x.Id)));

        return true;
    }
    #endregion
}