using Corno.Data.Corno.Question_Bank_V2.Models;
using Corno.Services.Corno.Interfaces;
using System.Collections.Generic;

namespace Corno.Services.Corno.Question_Bank_V2.Interfaces
{
    public class PaperGenerationCriteria
    {
        public int UnitId { get; set; }
        public int QuestionTypeId { get; set; }
        public int DifficultyLevelId { get; set; }
        public int TaxonomyLevelId { get; set; }
        public decimal Marks { get; set; }
        public List<int> ExcludedQuestionIds { get; set; } = new List<int>();
    }
    
    public interface IQB_PaperGenerationService
    {
        QB_Paper GeneratePaperAuto(QB_Appointment appointment, int setNumber, string userId);
        QB_Paper GeneratePaperManual(QB_Appointment appointment, int setNumber, 
            Dictionary<int, int> questionSelections, string userId); // StructureDetailId -> QuestionBankId
        List<QB_QuestionBank> GetAvailableQuestions(QB_Appointment appointment, PaperGenerationCriteria criteria);
        void DrawPaper(int paperId, string userId);
        bool CanModifyPaper(int paperId);
        byte[] GenerateWordDocument(QB_Paper paper);
    }
}
