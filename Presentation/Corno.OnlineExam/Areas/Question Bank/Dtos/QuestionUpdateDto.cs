namespace Corno.OnlineExam.Areas.Question_Bank.Dtos;

public class QuestionUpdateDto
{
    public int PaperId { get; set; }
    public int QuestionSerialNo { get; set; }
    public int DifficultyLevel { get; set; }
    public int? LearningPriorityId { get; set; }
    public int CoNo { get; set; }
    public string Content { get; set; }
}