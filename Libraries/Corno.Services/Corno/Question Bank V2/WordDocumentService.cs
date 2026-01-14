using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Corno.Data.Corno.Question_Bank_V2.Models;
using Corno.Logger;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Corno.Services.Corno.Question_Bank_V2
{
    public class WordDocumentService
    {
        public byte[] GenerateWordDocument(QB_Paper paper, string subjectName, string facultyName, string courseName)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                using (var wordDocument = WordprocessingDocument.Create(memoryStream, 
                           WordprocessingDocumentType.Document))
                {
                    var mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    var body = mainPart.Document.AppendChild(new Body());
                        
                    // Create Header
                    CreateHeader(body, paper, subjectName, facultyName, courseName);
                        
                    // Create Instructions
                    CreateInstructions(body, paper);
                        
                    // Create Sections and Questions
                    var sections = paper.PaperDetails
                        .GroupBy(pd => pd.SectionNo)
                        .OrderBy(g => g.Key)
                        .ToList();
                        
                    foreach (var sectionGroup in sections)
                    {
                        CreateSection(body, sectionGroup.Key, sectionGroup.ToList());
                    }
                        
                    // Create Footer
                    CreateFooter(body, paper);
                }
                    
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                LogHandler.LogError(ex);
                throw new Exception("Failed to generate Word document.", ex);
            }
        }
        
        private void CreateHeader(Body body, QB_Paper paper, string subjectName, string facultyName, string courseName)
        {
            // Title Paragraph
            var titlePara = body.AppendChild(new Paragraph());
            var titleRun = titlePara.AppendChild(new Run());
            titleRun.AppendChild(new Text($"{facultyName} - {courseName}"));
            
            var titleProps = titleRun.RunProperties ?? new RunProperties();
            titleProps.Bold = new Bold();
            titleProps.FontSize = new FontSize { Val = "28" };
            titleRun.RunProperties = titleProps;
            
            titlePara.ParagraphProperties = new ParagraphProperties
            {
                Justification = new Justification { Val = JustificationValues.Center },
                SpacingBetweenLines = new SpacingBetweenLines { After = "200" }
            };
            
            // Subject and Paper Info
            var infoPara = body.AppendChild(new Paragraph());
            infoPara.AppendChild(new Run(new Text($"Subject: {subjectName}")));
            infoPara.AppendChild(new Run(new Break()));
            infoPara.AppendChild(new Run(new Text($"Paper Code: {paper.PaperCode}")));
            infoPara.AppendChild(new Run(new Break()));
            infoPara.AppendChild(new Run(new Text($"Set: {paper.SetNumber}")));
            infoPara.AppendChild(new Run(new Break()));
            infoPara.AppendChild(new Run(new Text($"Max Marks: {paper.MaxMarks}")));
            if (paper.TimeDuration.HasValue)
            {
                infoPara.AppendChild(new Run(new Break()));
                infoPara.AppendChild(new Run(new Text($"Time: {paper.TimeDuration} minutes")));
            }
            
            infoPara.ParagraphProperties = new ParagraphProperties
            {
                Justification = new Justification { Val = JustificationValues.Center },
                SpacingBetweenLines = new SpacingBetweenLines { After = "400" }
            };
            
            // Horizontal Line
            body.AppendChild(new Paragraph(new Run(new Text(new string('-', 50)))));
        }
        
        private void CreateSection(Body body, int sectionNo, System.Collections.Generic.List<QB_PaperDetail> questions)
        {
            // Section Title
            var sectionTitle = body.AppendChild(new Paragraph());
            var sectionRun = sectionTitle.AppendChild(new Run(new Text($"Section {sectionNo}")));
            
            var sectionProps = sectionRun.RunProperties ?? new RunProperties();
            sectionProps.Bold = new Bold();
            sectionProps.FontSize = new FontSize { Val = "20" };
            sectionRun.RunProperties = sectionProps;
            
            sectionTitle.ParagraphProperties = new ParagraphProperties
            {
                SpacingBetweenLines = new SpacingBetweenLines { Before = "400", After = "200" }
            };
            
            // Questions in Section
            var questionNumber = 1;
            foreach (var question in questions.OrderBy(q => q.QuestionNo))
            {
                CreateQuestion(body, questionNumber++, question);
            }
        }
        
        private void CreateQuestion(Body body, int questionNumber, QB_PaperDetail question)
        {
            // Question Number and Text
            var questionPara = body.AppendChild(new Paragraph());
            
            var qNumRun = questionPara.AppendChild(new Run());
            qNumRun.AppendChild(new Text($"Q{questionNumber}. "));
            var qNumProps = qNumRun.RunProperties ?? new RunProperties();
            qNumProps.Bold = new Bold();
            qNumRun.RunProperties = qNumProps;
            
            // Question Text (HTML to Plain Text conversion)
            var questionText = HtmlToPlainText(question.QuestionText);
            questionPara.AppendChild(new Run(new Text(questionText)));
            
            questionPara.ParagraphProperties = new ParagraphProperties
            {
                SpacingBetweenLines = new SpacingBetweenLines { After = "200" },
                Indentation = new Indentation { Left = "360" }
            };
            
            // Marks
            var marksPara = body.AppendChild(new Paragraph());
            marksPara.AppendChild(new Run(new Text($"[Marks: {question.Marks}]")));
            
            var marksProps = marksPara.ParagraphProperties ?? new ParagraphProperties();
            marksProps.Justification = new Justification { Val = JustificationValues.Right };
            marksPara.ParagraphProperties = marksProps;
            
            // Spacing after question
            body.AppendChild(new Paragraph(new Run(new Text(""))));
        }
        
        private void CreateInstructions(Body body, QB_Paper paper)
        {
            var instructionsPara = body.AppendChild(new Paragraph());
            var instructionsRun = instructionsPara.AppendChild(new Run(new Text("Instructions:")));
            
            var instructionsProps = instructionsRun.RunProperties ?? new RunProperties();
            instructionsProps.Bold = new Bold();
            instructionsRun.RunProperties = instructionsProps;
            
            instructionsPara.ParagraphProperties = new ParagraphProperties
            {
                SpacingBetweenLines = new SpacingBetweenLines { Before = "200", After = "200" }
            };
        }
        
        private void CreateFooter(Body body, QB_Paper paper)
        {
            // Page break before footer
            body.AppendChild(new Paragraph(new Run(new Break { Type = BreakValues.Page })));
            
            var footerPara = body.AppendChild(new Paragraph());
            footerPara.AppendChild(new Run(new Text("--- End of Paper ---")));
            
            footerPara.ParagraphProperties = new ParagraphProperties
            {
                Justification = new Justification { Val = JustificationValues.Center },
                SpacingBetweenLines = new SpacingBetweenLines { Before = "400" }
            };
        }
        
        private string HtmlToPlainText(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            
            // Remove HTML tags and decode entities
            var text = Regex.Replace(html, "<.*?>", string.Empty);
            text = HttpUtility.HtmlDecode(text);
            
            return text.Trim();
        }
    }
}
