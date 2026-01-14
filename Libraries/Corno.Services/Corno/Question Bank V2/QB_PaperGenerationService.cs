using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Data.Corno.Question_Bank_V2.Models;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using Corno.Services.Corno.Question_Bank_V2.Interfaces;

namespace Corno.Services.Corno.Question_Bank_V2
{
    public class QB_PaperGenerationService : IQB_PaperGenerationService
    {
        private readonly IQB_QuestionBankService _questionBankService;
        private readonly IStructureService _structureService;
        private readonly IMainService<QB_Paper> _paperService;
        private readonly IMainService<QB_PaperDetail> _paperDetailService;
        
        public QB_PaperGenerationService(
            IQB_QuestionBankService questionBankService,
            IStructureService structureService,
            IMainService<QB_Paper> paperService,
            IMainService<QB_PaperDetail> paperDetailService)
        {
            _questionBankService = questionBankService;
            _structureService = structureService;
            _paperService = paperService;
            _paperDetailService = paperDetailService;
        }
        
        public QB_Paper GeneratePaperAuto(QB_Appointment appointment, int setNumber, string userId)
        {
            // Get structure
            var structure = _structureService.GetById(appointment.StructureId);
            if (structure == null)
                throw new Exception("Structure not found.");
            
            // Get approved questions
            var approvedQuestions = _questionBankService.GetApprovedQuestions(
                appointment.InstanceId ?? 0, 
                appointment.SubjectId ?? 0);
            
            if (approvedQuestions.Count == 0)
                throw new Exception("No approved questions available for paper generation.");
            
            // Create paper
            var paper = new QB_Paper
            {
                AppointmentId = appointment.Id,
                StructureId = appointment.StructureId ?? 0,
                InstanceId = appointment.InstanceId,
                FacultyId = appointment.FacultyId,
                CourseId = appointment.CourseId,
                CoursePartId = appointment.CoursePartId,
                BranchId = appointment.BranchId,
                SubjectId = appointment.SubjectId,
                PaperCategoryId = appointment.PaperCategoryId,
                PaperType = "Auto",
                SetNumber = setNumber,
                MaxMarks = structure.MaxMarks ?? 0,
                NoOfSections = structure.NoOfSections,
                TimeDuration = null, // Can be calculated
                GeneratedBy = userId,
                GeneratedDate = DateTime.Now,
                ModeratorUserId = userId,
                Status = "Generated",
                PaperDetails = new List<QB_PaperDetail>()
            };
            
            // Generate paper code
            paper.PaperCode = GeneratePaperCode(appointment.InstanceId ?? 0, setNumber);
            
            var usedQuestionIds = new List<int>();
            var difficultyDistribution = new Dictionary<int, int>();
            var taxonomyDistribution = new Dictionary<int, int>();
            
            // Process each structure detail
            foreach (var structureDetail in structure.StructureDetails.OrderBy(s => s.SectionNo).ThenBy(s => s.QuestionNo))
            {
                var chapterNos = structureDetail.ChapterNos?.Split(',');
                var nofOptions = structureDetail.NofOptions ?? 1;
                
                for (int optionIndex = 0; optionIndex < nofOptions; optionIndex++)
                {
                    int unitId = 0;
                    if (chapterNos != null && optionIndex < chapterNos.Length)
                    {
                        int.TryParse(chapterNos[optionIndex], out unitId);
                    }
                    
                    var criteria = new PaperGenerationCriteria
                    {
                        UnitId = unitId,
                        QuestionTypeId = structureDetail.QuestionTypeId ?? 0,
                        DifficultyLevelId = GetOptimalDifficulty(difficultyDistribution, structure),
                        TaxonomyLevelId = GetOptimalTaxonomy(taxonomyDistribution, structure),
                        Marks = (decimal)(structureDetail.Marks ?? 0),
                        ExcludedQuestionIds = usedQuestionIds
                    };
                    
                    var selectedQuestion = SelectBestQuestion(criteria, approvedQuestions, usedQuestionIds);
                    
                    if (selectedQuestion == null)
                        throw new Exception($"No suitable question found for Structure Detail {structureDetail.SerialNo}, Option {optionIndex + 1}");
                    
                    // Create paper detail
                    var paperDetail = new QB_PaperDetail
                    {
                        PaperId = paper.Id,
                        QuestionBankId = selectedQuestion.Id,
                        StructureDetailId = structureDetail.Id,
                        SectionNo = structureDetail.SectionNo ?? 1,
                        QuestionNo = structureDetail.QuestionNo ?? 1,
                        QuestionTypeId = structureDetail.QuestionTypeId ?? 0,
                        UnitId = unitId,
                        DifficultyLevelId = selectedQuestion.DifficultyLevelId ?? 0,
                        TaxonomyLevelId = selectedQuestion.TaxonomyLevelId ?? 0,
                        Marks = (decimal)(structureDetail.Marks ?? 0),
                        QuestionText = selectedQuestion.GetQuestionTextPlain(),
                        ModelAnswer = selectedQuestion.GetModelAnswerPlain(),
                        SelectionMethod = "Auto",
                        SelectionCriteria = System.Web.Helpers.Json.Encode(criteria)
                    };
                    
                    paper.PaperDetails.Add(paperDetail);
                    usedQuestionIds.Add(selectedQuestion.Id ?? 0);
                    
                    // Update distributions
                    var diffLevel = selectedQuestion.DifficultyLevelId ?? 0;
                    difficultyDistribution[diffLevel] = GetOrDefault(difficultyDistribution, diffLevel) + 1;
                    
                    var taxLevel = selectedQuestion.TaxonomyLevelId ?? 0;
                    taxonomyDistribution[taxLevel] = GetOrDefault(taxonomyDistribution, taxLevel) + 1;
                }
            }
            
            // Save paper
            _paperService.AddAndSave(paper);
            
            // Save paper details
            foreach (var detail in paper.PaperDetails)
            {
                detail.PaperId = paper.Id;
                _paperDetailService.AddAndSave(detail);
            }
            
            return paper;
        }
        
        public QB_Paper GeneratePaperManual(QB_Appointment appointment, int setNumber, 
            Dictionary<int, int> questionSelections, string userId)
        {
            var structure = _structureService.GetById(appointment.StructureId);
            if (structure == null)
                throw new Exception("Structure not found.");
            
            var paper = new QB_Paper
            {
                AppointmentId = appointment.Id,
                StructureId = appointment.StructureId ?? 0,
                InstanceId = appointment.InstanceId,
                FacultyId = appointment.FacultyId,
                CourseId = appointment.CourseId,
                CoursePartId = appointment.CoursePartId,
                BranchId = appointment.BranchId,
                SubjectId = appointment.SubjectId,
                PaperCategoryId = appointment.PaperCategoryId,
                PaperType = "Manual",
                SetNumber = setNumber,
                MaxMarks = structure.MaxMarks ?? 0,
                NoOfSections = structure.NoOfSections,
                GeneratedBy = userId,
                GeneratedDate = DateTime.Now,
                ModeratorUserId = userId,
                Status = "Generated",
                PaperDetails = new List<QB_PaperDetail>()
            };
            
            paper.PaperCode = GeneratePaperCode(appointment.InstanceId ?? 0, setNumber);
            
            // Process manual selections
            foreach (var structureDetail in structure.StructureDetails)
            {
                if (questionSelections.ContainsKey(structureDetail.Id ?? 0))
                {
                    var questionId = questionSelections[structureDetail.Id ?? 0];
                    var question = _questionBankService.GetById(questionId);
                    
                    if (question == null)
                        throw new Exception($"Question {questionId} not found.");
                    
                    var paperDetail = new QB_PaperDetail
                    {
                        PaperId = paper.Id,
                        QuestionBankId = question.Id,
                        StructureDetailId = structureDetail.Id,
                        SectionNo = structureDetail.SectionNo ?? 1,
                        QuestionNo = structureDetail.QuestionNo ?? 1,
                        QuestionTypeId = structureDetail.QuestionTypeId ?? 0,
                        UnitId = question.UnitId,
                        DifficultyLevelId = question.DifficultyLevelId ?? 0,
                        TaxonomyLevelId = question.TaxonomyLevelId ?? 0,
                        Marks = (decimal)(structureDetail.Marks ?? 0),
                        QuestionText = question.GetQuestionTextPlain(),
                        ModelAnswer = question.GetModelAnswerPlain(),
                        SelectionMethod = "Manual"
                    };
                    
                    paper.PaperDetails.Add(paperDetail);
                }
            }
            
            _paperService.AddAndSave(paper);
            
            foreach (var detail in paper.PaperDetails)
            {
                detail.PaperId = paper.Id;
                _paperDetailService.AddAndSave(detail);
            }
            
            return paper;
        }
        
        public List<QB_QuestionBank> GetAvailableQuestions(QB_Appointment appointment, PaperGenerationCriteria criteria)
        {
            var questions = _questionBankService.GetApprovedQuestions(
                appointment.InstanceId ?? 0,
                appointment.SubjectId ?? 0);
            
            return questions
                .Where(q => q.UnitId == criteria.UnitId)
                .Where(q => q.QuestionTypeId == criteria.QuestionTypeId)
                .Where(q => q.Marks == criteria.Marks)
                .Where(q => !criteria.ExcludedQuestionIds.Contains(q.Id ?? 0))
                .OrderByDescending(q => q.QualityScore ?? 0)
                .ThenBy(q => q.UsageCount)
                .ToList();
        }
        
        public void DrawPaper(int paperId, string userId)
        {
            var paper = _paperService.GetById(paperId);
            if (paper == null)
                throw new Exception("Paper not found.");
            
            if (paper.Status == "Drawn")
                throw new Exception("Paper has already been drawn and cannot be modified.");
            
            // Validate paper is complete
            if (paper.PaperDetails == null || paper.PaperDetails.Count == 0)
                throw new Exception("Paper is incomplete. Please add questions before drawing.");
            
            paper.Status = "Drawn";
            paper.DrawnDate = DateTime.Now;
            paper.DrawnBy = userId;
            
            // Generate Word document
            paper.WordDocumentContent = GenerateWordDocument(paper);
            paper.WordDocumentFileName = $"{paper.PaperCode}_Set{paper.SetNumber}.docx";
            paper.WordDocumentGeneratedDate = DateTime.Now;
            
            _paperService.UpdateAndSave(paper);
        }
        
        public bool CanModifyPaper(int paperId)
        {
            var paper = _paperService.GetById(paperId);
            return paper != null && paper.Status != "Drawn";
        }
        
        public byte[] GenerateWordDocument(QB_Paper paper)
        {
            // Word document generation using OpenXML
            // This is a placeholder - implement full Word generation
            try
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    // Use DocumentFormat.OpenXml to create Word document
                    // Implementation details in WordDocumentService
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                LogHandler.LogError(ex);
                return null;
            }
        }
        
        private QB_QuestionBank SelectBestQuestion(PaperGenerationCriteria criteria, 
            List<QB_QuestionBank> availableQuestions, List<int> usedQuestionIds)
        {
            var filtered = availableQuestions
                .Where(q => q.UnitId == criteria.UnitId)
                .Where(q => q.QuestionTypeId == criteria.QuestionTypeId)
                .Where(q => q.Marks == criteria.Marks)
                .Where(q => !usedQuestionIds.Contains(q.Id ?? 0))
                .ToList();
            
            if (filtered.Count == 0)
                return null;
            
            // Score and select best question
            var scored = filtered.Select(q => new
            {
                Question = q,
                Score = CalculateScore(q, criteria)
            }).OrderByDescending(s => s.Score).FirstOrDefault();
            
            return scored?.Question;
        }
        
        private decimal CalculateScore(QB_QuestionBank question, PaperGenerationCriteria criteria)
        {
            decimal score = 100.0m;
            
            // Difficulty match (30% weight)
            if (question.DifficultyLevelId == criteria.DifficultyLevelId)
                score += 30;
            else
            {
                var diff = Math.Abs((question.DifficultyLevelId ?? 0) - criteria.DifficultyLevelId);
                score -= diff * 10;
            }
            
            // Taxonomy match (25% weight)
            if (question.TaxonomyLevelId == criteria.TaxonomyLevelId)
                score += 25;
            else
            {
                var diff = Math.Abs((question.TaxonomyLevelId ?? 0) - criteria.TaxonomyLevelId);
                score -= diff * 5;
            }
            
            // Usage count (lower is better - 20% weight)
            score -= question.UsageCount * 2;
            
            // Quality score (15% weight)
            if (question.QualityScore.HasValue)
                score += (decimal)(question.QualityScore.Value * 3);
            
            // Recent usage (10% weight)
            if (question.LastUsedDate.HasValue)
            {
                var daysSinceUse = (DateTime.Now - question.LastUsedDate.Value).Days;
                score += Math.Min(daysSinceUse / 10.0m, 10);
            }
            else
                score += 10; // Never used - bonus
            
            return Math.Max(0, score);
        }
        
        private int GetOptimalDifficulty(Dictionary<int, int> currentDistribution, Structure structure)
        {
            var totalQuestions = structure.StructureDetails.Sum(s => s.NofOptions ?? 1);
            var targetEasy = (int)(totalQuestions * 0.20);
            var targetMedium = (int)(totalQuestions * 0.40);
            var targetHard = (int)(totalQuestions * 0.30);
            var targetVeryHard = (int)(totalQuestions * 0.10);
            
            var currentEasy = GetOrDefault(currentDistribution, 1);
            var currentMedium = GetOrDefault(currentDistribution, 2);
            var currentHard = GetOrDefault(currentDistribution, 3);
            var currentVeryHard = GetOrDefault(currentDistribution, 4);
            
            if (currentEasy < targetEasy) return 1;
            if (currentMedium < targetMedium) return 2;
            if (currentHard < targetHard) return 3;
            if (currentVeryHard < targetVeryHard) return 4;
            
            return 2; // Default to Medium
        }
        
        private int GetOptimalTaxonomy(Dictionary<int, int> currentDistribution, Structure structure)
        {
            var totalQuestions = structure.StructureDetails.Sum(s => s.NofOptions ?? 1);
            var targetPerLevel = totalQuestions / 6;
            
            for (int level = 1; level <= 6; level++)
            {
                if (GetOrDefault(currentDistribution, level) < targetPerLevel)
                    return level;
            }
            
            return 3; // Default to Apply
        }

        private static int GetOrDefault(Dictionary<int, int> dictionary, int key, int defaultValue = 0)
        {
            int value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }
        
        private string GeneratePaperCode(int instanceId, int setNumber)
        {
            var year = DateTime.Now.Year;
            var count = _paperService.GetQuery().Count(p => p.InstanceId == instanceId && 
                                                           p.PaperCode.StartsWith($"PAP-{year}"));
            
            return $"PAP-{year}-{(count + 1):D5}-SET{setNumber}";
        }
    }
}
