using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Data.Helpers;
using Corno.Data.ViewModels;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;

namespace Corno.Services.Corno.Question_Bank;

public class StructureService : MainService<Structure>, IStructureService
{
    #region -- Constructors --
    public StructureService(ICornoService cornoService, ISubjectService subjectService)
    {
        _cornoService = cornoService;
        _subjectService = subjectService;

        SetIncludes(nameof(Structure.StructureDetails));
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _cornoService;
    private readonly ISubjectService _subjectService;

    readonly List<int> _theoryCategories = new() { 2, 48 };

    #endregion

    #region -- Private Methods --
    private void ValidateHeaderFields(Structure structure)
    {
        if (structure.FacultyId.ToInt() <= 0)
            throw new Exception("Invalid Faculty.");
        if (structure.CourseId.ToInt() <= 0)
            throw new Exception("Invalid Course.");
        if (structure.CoursePartId.ToInt() <= 0)
            throw new Exception("Invalid Course Part.");
        if (structure.SubjectId.ToInt() <= 0)
            throw new Exception("Invalid Subject.");
        if (structure.PaperCategoryId.ToInt() <= 0)
            throw new Exception("Invalid Paper Category.");
    }

    private void ValidateTotalMarks(Structure structure)
    {
        var totalMarks = 0;
        var subject = _subjectService.GetById(structure.SubjectId);
        var subjectCategoryDetail = subject.SubjectCategoryDetails.FirstOrDefault(d =>
            _theoryCategories.Contains(d.CategoryId) && d.PaperCategoryId == structure.PaperCategoryId);
        var groups = structure.StructureDetails.GroupBy(x => x.SectionNo);
        foreach (var group in groups)
        {
            var first = group.First();
            var subjectSectionDetail = subject.SubjectSectionDetails.FirstOrDefault(d =>
                d.SectionNo == first.SectionNo && d.PaperCategoryId == structure.PaperCategoryId);
            if (null == subjectSectionDetail) continue;
            totalMarks += group
                .OrderByDescending(x =>
                    x.Marks.ToInt() * x.AttemptCount.ToInt()) // Order rows in descending order of Marks * AttemptCount
                .Take(subjectSectionDetail.AttemptCount) // Take AttemptCount rows for sections
                .Sum(x => x.Marks.ToInt() * x.AttemptCount.ToInt()); // Sum the selected rows fo
        }
        if (!totalMarks.Equals(subjectCategoryDetail?.MaxMarks))
            throw new Exception($"Total Sum of questions marks ({totalMarks}) does not match with max marks ({subjectCategoryDetail?.MaxMarks}).");
    }

    private void ValidateFields(Structure structure, bool validateDetails = true)
    {
        // Validate header fields
        ValidateHeaderFields(structure);

        if (structure.NoOfSections.ToInt() <= 0)
            throw new Exception("Invalid No. Of Sections.");
        if (structure.NoOfChapters.ToInt() <= 0)
            throw new Exception("Invalid No. Of Chapters.");
        if (structure.NoOfQuestions.ToInt() <= 0)
            throw new Exception("Invalid No. Of Questions.");
        if (structure.MaxMarks.ToInt() <= 0)
            throw new Exception("Invalid Max Marks.");

        if (!validateDetails) return;

        if (structure.StructureDetails.Count <= 0)
            throw new Exception("No rows in structure.");
        if (structure.StructureDetails.Count > structure.NoOfQuestions)
            throw new Exception("No. of question are greater than max count");

        // Validate total marks
        ValidateTotalMarks(structure);

        /*var totalMarks = structure.StructureDetails.Sum(d => (d.Marks ?? 0) * (d.AttemptCount ?? 0));
        if (!totalMarks.Equals(structure.MaxMarks))
            throw new Exception($"Total Sum of questions marks ({totalMarks}) does not match with max marks ({structure.MaxMarks}).");*/
        foreach (var detail in structure.StructureDetails)
        {
            if (string.IsNullOrEmpty(detail.ChapterNos))
                throw new Exception($"Chapter Nos. are blank for question {detail.SerialNo}");
            var chapterNos = detail.ChapterNos.Split(',');
            if (!chapterNos.Length.Equals(detail.NofOptions))
                throw new Exception($"No. of options for Question No {detail.SerialNo} doesn't match with chapter nos. count");
            //if ((detail.SectionNo ?? 0) > (detail.NofOptions ?? 0))
            //    throw new Exception($"Invalid section no for question {detail.SerialNo}");
            if ((detail.AttemptCount ?? 0) > (detail.NofOptions ?? 0))
                throw new Exception($"Invalid Attempt any for question {detail.SerialNo}");
            if ((detail.Marks ?? 0) <= 0)
                throw new Exception($"Invalid marks for question {detail.SerialNo}");
        }
    }

    private Structure GetExisting(Structure structure)
    {
        var existing = GetExisting(structure.FacultyId ?? 0, structure.CourseId ?? 0, structure.CoursePartId ?? 0,
            structure.BranchId ?? 0, structure.SubjectId ?? 0, structure.PaperCategoryId ?? 0);
        return existing;
    }

    #endregion

    #region -- Public Methods --
    public Structure GetExisting(QuestionBankModel model)
    {
        var existing = GetExisting(model.FacultyId ?? 0, model.CourseId ?? 0, model.CoursePartId ?? 0,
            model.BranchId ?? 0, model.SubjectId ?? 0, model.PaperCategoryId ?? 0);
        return existing;
    }

    public Structure GetExisting(int facultyId, int courseId, int coursePartId, int branchId, int subjectId,
        int paperCategoryId)
    {
        var existing = FirstOrDefault(s => (s.FacultyId ?? 0) == facultyId &&
                                           (s.CourseId ?? 0) == courseId &&
                                           (s.CoursePartId ?? 0) == coursePartId &&
                                           (s.BranchId ?? 0) == branchId &&
                                           (s.SubjectId ?? 0) == subjectId &&
                                           (s.PaperCategoryId ?? 0) == paperCategoryId, s => s);
        return existing;
    }

    public Structure GetQuestions(Structure structure)
    {
        // Validate fields
        ValidateHeaderFields(structure);

        var existing = GetExisting(structure);
        if (null != existing)
            return existing;

        var subject = _subjectService.GetById(structure.SubjectId ?? 0);
        if (null == subject)
            throw new Exception($"Invalid subject ({structure.SubjectId})");

        
        var subjectCategoryDetail = subject.SubjectCategoryDetails.FirstOrDefault(d => 
                _theoryCategories.Contains(d.CategoryId) && d.PaperCategoryId == structure.PaperCategoryId);

        structure.NoOfChapters = subject.SubjectChapterDetails.Count;
        structure.NoOfSections = subject.SubjectSectionDetails
            .Where(d => d.PaperCategoryId == structure.PaperCategoryId)
            .Count(d => d.SectionNo >= 0);
        structure.NoOfQuestions = subject.SubjectSectionDetails
            .Where(d => d.PaperCategoryId == structure.PaperCategoryId)
            .Sum(d => d.NoOfQuestions);
        structure.MaxMarks = subjectCategoryDetail?.MaxMarks;

        structure.StructureDetails.Clear();
        // Add blank objects to the list using LINQ
        Enumerable.Range(1, structure.NoOfQuestions ?? 0).ToList().ForEach(index =>
            structure.StructureDetails.Add(new StructureDetail { SerialNo = index }));
        /*for (var index = 0; index < structure.NoOfQuestions; index++)
            structure.StructureDetails.Add(new StructureDetail());*/

        return structure;
    }

    public List<MasterViewModel> GetQuestionNos(int subjectId, int chapterId)
    {
        if (subjectId <= 0)
            throw new Exception("Invalid Subject code");
        if (chapterId <= 0)
            throw new Exception("Invalid chapter");

        var structure = _cornoService.StructureRepository.Get(s => s.SubjectId == subjectId).FirstOrDefault();
        if (null == structure)
            throw new Exception($"Structure is not created for subject {subjectId}");

        var questionNos = structure.StructureDetails
            .Where(d => d.ChapterNos.Contains(chapterId.ToString()))
            .Select(d => new MasterViewModel { Id = d.SerialNo ?? 0, SerialNo = d.Marks.ToInt(), Name = d.Marks.ToString() })
            .ToList();

        return questionNos;
    }

    public void Save(Structure structure)
    {
        // Validate Fields
        ValidateFields(structure);

        var existing = GetExisting(structure);
        if (null != existing)
        {
            if (structure.PaperCategoryId > 0 && !structure.PaperCategoryId.Equals(existing.PaperCategoryId))
                existing.PaperCategoryId = structure.PaperCategoryId;
            existing.StructureDetails.ForEach(d =>
            {
                var newDetail = structure.StructureDetails.FirstOrDefault(x => x.SerialNo == d.SerialNo);
                d.QuestionNo = newDetail?.QuestionNo;
                d.SectionNo = newDetail?.SectionNo;
                d.ChapterNos = newDetail?.ChapterNos;
                d.NofOptions = newDetail?.NofOptions;
                d.QuestionTypeId = newDetail?.QuestionTypeId;
                d.QuestionText = newDetail?.QuestionText;
                d.AttemptCount = newDetail?.AttemptCount;
                d.Marks = newDetail?.Marks;
            });

            _cornoService.StructureRepository.Update(existing);
        }
        else
            _cornoService.StructureRepository.Add(structure);

        _cornoService.Save();
    }
    #endregion
}