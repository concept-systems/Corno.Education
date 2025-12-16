using System.IO;
using System.Linq;
using System.Web;
using System.Windows;
using System.Windows.Media;
using Corno.Data.Corno.Question_Bank;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using HtmlAgilityPack;
using Telerik.Windows.Documents.Flow.FormatProviders.Html;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.Windows.Documents.Flow.Model.Editing;
using Telerik.Windows.Documents.Flow.Model.Shapes;
using Telerik.Windows.Documents.Flow.Model.Styles;
using Telerik.Windows.Documents.Spreadsheet.Model;
using BorderStyle = Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle;
using ImageSource = Telerik.Windows.Documents.Media.ImageSource;
using Unit = Telerik.Windows.Documents.Media.Unit;

namespace Corno.Services.Corno.Question_Bank;

public class DocumentService : BaseService, IDocumentService
{
    #region -- Constructors --
    public DocumentService(ICornoService cornoService, ICourseService courseService,
        ICoursePartService coursePartService, ISubjectService subjectService, IStructureService structureService)
    {
        _cornoService = cornoService;
        _courseService = courseService;
        _coursePartService = coursePartService;
        _subjectService = subjectService;
        _structureService = structureService;
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _cornoService;
    private readonly ICourseService _courseService;
    private readonly ICoursePartService _coursePartService;
    private readonly ISubjectService _subjectService;
    private readonly IStructureService _structureService;

    #endregion

    #region -- Private Methods --
    private void CreateHeader(RadFlowDocumentEditor editor, Paper paper)
    {
        var course = _courseService.GetById(paper.CourseId ?? 0);
        var coursePart = _coursePartService.GetById(paper.CoursePartId ?? 0);
        var subject = _subjectService.GetById(paper.SubjectId ?? 0);

        // Add new section
        editor.Document.Sections.AddSection();

        var retrievedSection = editor.Document.Sections.First();

        var paragraph1 = retrievedSection.Blocks.AddParagraph();
        paragraph1.TextAlignment = Alignment.Left;
        // Total No. of Questions 
        paragraph1.Inlines.AddRun($"Total No. of Questions: {paper.NoOfQuestions}\t\t\t\t\t\t\t Seat No.:");

        var paragraph2 = retrievedSection.Blocks.AddParagraph();
        paragraph2.TextAlignment = Alignment.Center;
        // Course
        var line1 = new Run(editor.Document) { FontWeight = FontWeights.Bold, Text = course?.Name?.ToUpper() + "\n" };
        paragraph2.Inlines.Add(line1);
        paragraph2.Inlines.Add(new Break(editor.Document));
        // Course Part : Instance
        var line2 = new Run(editor.Document) { FontWeight = FontWeights.Bold, Text = $"{coursePart?.Name?.ToUpper()} : #InstanceName#" };
        paragraph2.Inlines.Add(line2);
        paragraph2.Inlines.Add(new Break(editor.Document));
        // Subject
        var line3 = new Run(editor.Document) { FontWeight = FontWeights.Bold, Text = $"SUBJECT : {subject?.Name?.ToUpper()}" };
        paragraph2.Inlines.Add(line3);
        //paragraph.Inlines.Add(new Break(editor.Document));
        // Branch
        if ((paper.BranchId ?? 0) > 0)
        {
            var line4 = new Run(editor.Document) { FontWeight = FontWeights.Bold, Text = $"BRANCH : " };
            paragraph2.Inlines.Add(line4);
            paragraph2.Inlines.Add(new Break(editor.Document));
        }

        var paragraph3 = retrievedSection.Blocks.AddParagraph();
        paragraph3.TextAlignment = Alignment.Left;
        // Day     Time
        paragraph3.Inlines.AddRun("Day: \t\t\t\t\t\t\t\t\t\t Time:");
        paragraph3.Inlines.Add(new Break(editor.Document));
        // Date     Max Marks
        paragraph3.Inlines.AddRun($"Date: \t\t\t\t\t\t\t\t\t\t Max Marks:  {paper.MaxMarks}");
        //paragraph2.Inlines.Add(new Break(editor.Document));

        // Horizontal line
        //paragraph2.Borders = new ParagraphBorders(null, null, null, new Border(BorderStyle.Single));

        var border = new Border(1, BorderStyle.Thick, ThemableColor.FromColor(Colors.Black));

        /*var table = retrievedSection.Blocks.AddTable();
        table.Borders = new TableBorders(border);
        table.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);*/

        var paragraph4 = retrievedSection.Blocks.AddParagraph();
        paragraph4.TextAlignment = Alignment.Left;
        // Note Below
        // Horizontal line
        paragraph4.Borders = new ParagraphBorders(null, border, null, border);
        paragraph4.Inlines.AddRun("Notes Below:").FontWeight = FontWeights.Bold;
        paragraph4.Inlines.Add(new Break(editor.Document));
        var index = 0;
        foreach (var instruction in subject?.SubjectInstructionDetails.OrderBy(p => p.SerialNo).ToList()!)
        //for (var index = 0; index < subject?.SubjectInstructionDetails.Count; index++)
        {
            //var instruction = subject.SubjectInstructionDetails[index];
            paragraph4.Inlines.AddRun($"{index + 1}. {instruction?.Description}");
            if (!index.Equals(subject?.SubjectInstructionDetails.Count - 1))
                paragraph4.Inlines.Add(new Break(editor.Document));
            index++;
        }
        paragraph4.Inlines.Remove(new Break(editor.Document));

        //// Create a paragraph with a bottom border
        //var paragraphHorizantalLine = retrievedSection.Blocks.AddParagraph();
        //paragraphHorizantalLine.Borders.Between = new Border(1, BorderStyle.Single, ThemableColor.FromColor(Colors.Black));
    }

    private void CreateFooter(RadFlowDocumentEditor editor, Paper paper)
    {
        var retrievedSection = editor.Document.Sections.First();

        var paragraph1 = retrievedSection.Blocks.AddParagraph();
        paragraph1.TextAlignment = Alignment.Center;
        // **** 
        paragraph1.Inlines.AddRun("* * * * *");

        /*// Add a default header to the section
        var defaultFooter = retrievedSection.Footers.Add();
        var paragraph1 = defaultFooter.Blocks.AddParagraph();
        paragraph1.TextAlignment = Alignment.Center;
        */

        //// Insert the PAGE field for the current page number
        //var pageField = new Field(editor.Document, FieldType.Page);
        //paragraph1.Inlines.Add(pageField.CreateFieldCodeStart());
        //paragraph1.Inlines.AddRun("PAGE");
        //paragraph1.Inlines.Add(pageField.CreateFieldCodeEnd());

    }
    /*private void CreateHeader(RadFlowDocumentEditor editor, Paper paper)
    {
        var course = _courseService.GetById(paper.CourseId ?? 0);
        var coursePart = _coursePartService.GetById(paper.CoursePartId ?? 0);
        var subject = _subjectService.GetById(paper.SubjectId ?? 0);

        // Add a new section to the document
        var section = editor.InsertSection();

        // Add a default header to the section
        var defaultHeader = section.Headers.Add();

        /#1#/ Creating Header with an image
        var defaultHeader = editor.Document.Sections.First().Headers.Add();#1#

        var paragraph = defaultHeader.Blocks.AddParagraph();
        paragraph.TextAlignment = Alignment.Center;

        // Course
        var line1 = new Run(editor.Document) { FontWeight = FontWeights.Bold, Text = course?.Name?.ToUpper() + "\n" };
        paragraph.Inlines.Add(line1);
        paragraph.Inlines.Add(new Break(editor.Document));
        // Course Part : Instance
        var line2 = new Run(editor.Document) { FontWeight = FontWeights.Bold, Text = $"{coursePart?.Name?.ToUpper()} : #InstanceName#" };
        paragraph.Inlines.Add(line2);
        paragraph.Inlines.Add(new Break(editor.Document));
        // Subject
        var line3 = new Run(editor.Document) { FontWeight = FontWeights.Bold, Text = $"SUBJECT : {subject?.Name?.ToUpper()}" };
        paragraph.Inlines.Add(line3);
        //paragraph.Inlines.Add(new Break(editor.Document));
        // Branch
        if ((paper.BranchId ?? 0) > 0)
        {
            var line4 = new Run(editor.Document) { FontWeight = FontWeights.Bold, Text = $"BRANCH : " };
            paragraph.Inlines.Add(line4);
            paragraph.Inlines.Add(new Break(editor.Document));
        }

        var paragraph2 = defaultHeader.Blocks.AddParagraph();
        paragraph2.TextAlignment = Alignment.Left;
        // Date     Time
        paragraph2.Inlines.AddRun("Day: \t\t\t\t\t\t\t\t\t\t Time:");
        paragraph2.Inlines.Add(new Break(editor.Document));
        // Date     Time
        paragraph2.Inlines.AddRun($"Date: \t\t\t\t\t\t\t\t\t\t Max Marks:  {paper.MaxMarks}");
        //paragraph2.Inlines.Add(new Break(editor.Document));

        // Horizontal line
        //paragraph2.Borders = new ParagraphBorders(null, null, null, new Border(BorderStyle.Single));

        /*var table = defaultHeader.Blocks.AddTable();
        table.Borders = new TableBorders(null, new Border(BorderStyle.Thick), null, null);
        table.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);#1#

        var paragraph3 = defaultHeader.Blocks.AddParagraph();
        paragraph3.TextAlignment = Alignment.Left;
        // Date     Time
        paragraph3.Inlines.AddRun("Notes Below:").FontWeight = FontWeights.Bold;
        paragraph3.Inlines.Add(new Break(editor.Document));
        /*foreach (var instruction in subject?.SubjectInstructionDetails.ToList()!)#1#
        for (var index = 0; index < subject?.SubjectInstructionDetails.Count; index++)
        {
            var instruction = subject.SubjectInstructionDetails[index];
            paragraph3.Inlines.AddRun($"{index + 1}. {instruction?.Description}");
            if (!index.Equals(subject?.SubjectInstructionDetails.Count - 1))
                paragraph3.Inlines.Add(new Break(editor.Document));
        }
        paragraph3.Inlines.Remove(new Break(editor.Document));
        // Horizontal line
        paragraph3.Borders = new ParagraphBorders(null, new Border(BorderStyle.Single), null, new Border(BorderStyle.Single));
    }*/

    private byte[] GetImageData(string imageUrl)
    {
        try
        {
            // Ensure the URL is a local path
            var localPath = HttpContext.Current.Server.MapPath(imageUrl); // Convert URL to local server path
            localPath = localPath.Replace("%20", " ");
            return System.IO.File.Exists(localPath) ? System.IO.File.ReadAllBytes(localPath) : null;
        }
        catch
        {
            // Handle exceptions as necessary
            return null;
        }
    }

    private void InsertHtmlContentIntoCell(Paragraph paragraph, HtmlNode rootNode)
    {
        // Iterate through the HTML nodes and insert them into the cell
        foreach (var node in rootNode.ChildNodes)
        {
            if (node.HasChildNodes)
            {
                InsertHtmlContentIntoCell(paragraph, node);
                paragraph.Inlines.Add(new Break(paragraph.Document));
                continue;
            }

            switch (node.NodeType)
            {
                case HtmlNodeType.Text:
                    var run = paragraph.Inlines.AddRun(node.InnerText);
                    if (node.XPath.ToLower().Contains("strong"))
                        run.FontWeight = FontWeights.Bold;

                    break;
                case HtmlNodeType.Element:
                    {
                        if (node.Name == "img")
                        {
                            // Handle image insertion accordingly
                            // Example:
                            var imageUrl = node.GetAttributeValue("src", "");
                            var imageData = GetImageData(imageUrl);
                            if (imageData == null) continue;

                            using var imageStream = new MemoryStream(imageData);
                            var imageInline = new ImageInline(paragraph.Document)
                            {
                                Image = { ImageSource = new Telerik.Windows.Documents.Media.ImageSource(imageStream, "jpg") }
                            };
                            paragraph.Inlines.Add(new Break(paragraph.Document)); // Add a line break before the image if needed
                            paragraph.Inlines.Add(imageInline);
                        }

                        break;
                    }
                    /*default:
                        //paragraph.Inlines.AddRun(node.InnerText);
                        break;*/
            }
        }
    }

    public BlockBase CloneBlock(BlockBase block, RadFlowDocument document)
    {
        switch (block)
        {
            case Paragraph paragraph:
            {
                var newParagraph = new Paragraph(document);
                    /*{
                        ListId = paragraph.ListId,
                        ListLevel = paragraph.ListLevel
                    };*/
                    foreach (var inline in paragraph.Inlines)
                    {
                        newParagraph.Inlines.Add(CloneInline(inline, document));
                    }
                    return newParagraph;
                }
            case Table table1:
                {
                    var table = table1;
                    var newTable = new Table(document);
                    foreach (var row in table.Rows)
                    {
                        var newRow = new TableRow(document);
                        foreach (var cell in row.Cells)
                        {
                            var newCell = new TableCell(document);
                            foreach (var cellBlock in cell.Blocks)
                            {
                                newCell.Blocks.Add(CloneBlock(cellBlock, document));
                            }
                            newRow.Cells.Add(newCell);
                        }
                        newTable.Rows.Add(newRow);
                    }
                    return newTable;
                }
            default:
                // Add other block types as needed
                return null;
        }
    }

    public InlineBase CloneInline(InlineBase inline, RadFlowDocument document)
    {
        //if (inline is not Run run) return null;
        switch (inline)
        {
            case Run run:
                return new Run(document) { Text = run.Text };
            case ImageInline imageInline:
                /*var newImageInline = new ImageInline(document)
                {
                    Image =
                    {
                        ImageSource = imageInline.Image.ImageSource,
                        //Size = imageInline.Image.Size
                    }
                };
                return newImageInline;*/
                var newImageInline = new ImageInline(document);
                var imageSource = new ImageSource(imageInline.Image.ImageSource.Data, "jpg");
                newImageInline.Image.ImageSource = imageSource;
                return newImageInline;
            // Handle other inline types as needed
            default:
                return null;
        }

        /*var newRun = new Run(document)
        {
            Text = run.Text
        };
        return newRun;*/
        // Add other inline types as needed
    }

    private string CorrectHtmlContent(string htmlContent)
    {
        //LogHandler.LogInfo($"{htmlContent} \n");

        // Replace <br/> with <p></p>
        htmlContent = htmlContent.Replace("<br/>", "<p></p>");
        htmlContent = htmlContent.Replace("<br />", "<p></p>");

        // Replace &amp; &gt; &lt;
        htmlContent = htmlContent.Replace("&amp;", "&");
        htmlContent = htmlContent.Replace("&gt;", ">");
        htmlContent = htmlContent.Replace("&lt;", "<");

        return htmlContent;
    }

    public RadFlowDocument GetHtmlContent(string htmlContent)
    {
        htmlContent = CorrectHtmlContent(htmlContent);

        var htmlProvider = new HtmlFormatProvider();
        htmlProvider.ImportSettings.LoadImageFromUri += (sender, e) =>
        {
            // Load the image data from the URI
            using var webClient = new System.Net.WebClient();
            var path = System.Web.Hosting.HostingEnvironment.MapPath(e.Uri.ToString()) ?? string.Empty;
            path = path.Replace("%20", " ");
            var imageData = webClient.DownloadData(path);
            e.SetImageInfo(imageData, "jpg"); // Adjust the format as needed
        };
        var newContent = htmlProvider.Import(htmlContent);

        return newContent;
    }

    public void AddContentToTableCell(TableCell cell, string htmlContent)
    {
        var newContent = GetHtmlContent(htmlContent);

        foreach (var block in newContent.Sections.First().Blocks)
        {
            cell.Blocks.Add(CloneBlock(block, cell.Document));
        }
    }

    private void InsertHtmlContentIntoCell(TableCell cell, string editorHtmlContent)
    {
        // Load the HTML content from the RadEditor into HtmlAgilityPack
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(editorHtmlContent);

        var paragraph = new Paragraph(cell.Document);
        InsertHtmlContentIntoCell(paragraph, htmlDoc.DocumentNode);
        cell.Blocks.Add(paragraph);

        /*// Iterate through the HTML nodes and insert them into the cell
        foreach (var node in htmlDoc.DocumentNode.ChildNodes)
        {
            /*if(node.HasChildNodes)
                InsertHtmlContentIntoCell(cell, editorHtmlContent);#1#

            // Convert the HTML node to RadFlowDocument blocks and add them to the cell
            //ConvertHtmlNodeToBlocks(cell.Blocks, node, cell.Document);
            if (node.Name == "img")
            {
                // Handle image insertion accordingly
                // Example:
                 var imageUrl = node.GetAttributeValue("src", "");
                 var i = 0;
                 // byte[] imageData = GetImageData(imageUrl);
                 // if (imageData != null)
                 // {
                 //     Paragraph paragraph = new Paragraph();
                 //     paragraph.Inlines.AddImage(imageData);
                 //     blocks.Add(paragraph);
                 // }
            }
            else
            {
                cell.Blocks.AddParagraph().Inlines.AddRun(node.InnerText);
            }
        }*/
    }

    /*private void ConvertHtmlNodeToBlocks(BlockCollection blocks, HtmlNode node, RadFlowDocument document)
    {
        // Your logic to convert HTML nodes to RadFlowDocument blocks
        // For example:
        if (node.Name == "p")
        {
            // Create a new paragraph block and add it to the blocks collection
            var paragraph = new Paragraph(document);
            paragraph.Inlines.AddRun(node.InnerText); // Or handle inner HTML accordingly
            blocks.Add(paragraph);
        }
        else if (node.Name == "img")
        {
            // Handle image insertion accordingly
            // Example:
            // string imageUrl = node.GetAttributeValue("src", "");
            // byte[] imageData = GetImageData(imageUrl);
            // if (imageData != null)
            // {
            //     Paragraph paragraph = new Paragraph();
            //     paragraph.Inlines.AddImage(imageData);
            //     blocks.Add(paragraph);
            // }
        }
        // Add more conditions to handle other HTML elements as needed
    }*/

    /*private InlineBase CloneInline(InlineBase originalInline, RadFlowDocument document)
    {
        if (originalInline is Run)
        {
            Run originalRun = (Run)originalInline;
            Run newRun = new Run(document);

            newRun.Text = originalRun.Text
            // Copy properties from originalRun to newRun
            newRun.FontFamily = originalRun.FontFamily;
            newRun.FontSize = originalRun.FontSize;
            // Copy other properties as needed

            return newRun;
        }
        else if (originalInline is ImageInline)
        {
            ImageInline originalImageInline = (ImageInline)originalInline;
            ImageInline newImageInline = new ImageInline(document);
            newImageInline.Image = originalImageInline.Image;

            // Copy properties from originalImageInline to newImageInline
            // Adjust properties as needed

            return newImageInline;
        }
        // Handle other inline types as needed
        return null;
    }*/

    /*public void AddToTableCell(string htmlContent, TableCell cell)
    {
        // Create an instance of HtmlFormatProvider
        var provider = new HtmlFormatProvider();

        // Import the HTML content to a RadFlowDocument
        var document = provider.Import(htmlContent);

        // Clear the existing content in the cell
        cell.Blocks.Clear();

        // Create a new Paragraph in the cell
        //var paragraph = cell.Blocks.AddParagraph();

        // Append the imported document to the cell
        foreach (var section in document.Sections)
        {
            foreach (var block in section.Blocks)
            {
                if (block is not Paragraph paragraph1) continue;

                Block clone = block.Clone();
                cell.Blocks.Add(clone);

                /*foreach (var inline in ((Paragraph)block).Inlines)
                {
                    if (inline is Run run)
                    {
                        Run newRun = new Run(cell.Document) { Text = run.Text };
                        paragraph.Inlines.Add(newRun);
                    }
                    // Handle other types of Inline elements as needed
                }#1#
            }
        }
    }*/

    private void AddSectionRow(Table table, int sectionNo)
    {
        string[] romanNumerals = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };

        // Add section
        var sectionRow = table.Rows.AddTableRow();
        var cellSection = sectionRow.Cells.AddTableCell();
        cellSection.ColumnSpan = 3;
        cellSection.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);

        var paragraphSection = cellSection.Blocks.AddParagraph();
        paragraphSection.TextAlignment = Alignment.Center;
        paragraphSection.Inlines.AddRun($"SECTION - {romanNumerals[sectionNo - 1]}").FontWeight = FontWeights.Bold;
    }

    private void AddQuestionRow(Table table, StructureDetail structureDetail, string description)
    {
        // Add Question
        var row = table.Rows.AddTableRow();

        var cellQuestionNo = row.Cells.AddTableCell();
        cellQuestionNo.Blocks.AddParagraph().Inlines.AddRun($"Q. {structureDetail.SerialNo}");
        cellQuestionNo.PreferredWidth = new TableWidthUnit(50);

        var cellDescription = row.Cells.AddTableCell();
        cellDescription.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Auto, 1);
        //cellDescription.Blocks.AddParagraph().Inlines.AddRun(description);
        //AddToTableCell(description, cellDescription);

        // Parse the HTML content and insert it into the target cell
        //InsertHtmlContentIntoCell(cellDescription, description);
        AddContentToTableCell(cellDescription, description);

        var cellMarks = row.Cells.AddTableCell();
        cellMarks.PreferredWidth = new TableWidthUnit(40);
        var paragraphSection = cellMarks.Blocks.AddParagraph();
        paragraphSection.TextAlignment = Alignment.Right;
        //var attemptCount = (structureDetail.AttemptCount ?? 0) <= 0 ? 1 : structureDetail.AttemptCount;
        paragraphSection.Inlines.AddRun($"({structureDetail.Marks})");

        /*cellMarks.Blocks.AddParagraph().Inlines.AddRun($"{(structureDetail.Marks)}");
        cellMarks.PreferredWidth = new TableWidthUnit(5);*/
    }

    private void AddAttemptRow(Table table, string description)
    {
        // Add Question
        var row = table.Rows.AddTableRow();

        var cellQuestionNo = row.Cells.AddTableCell();
        cellQuestionNo.Blocks.AddParagraph().Inlines.AddRun($"");
        cellQuestionNo.PreferredWidth = new TableWidthUnit(40);

        var cellDescription = row.Cells.AddTableCell();
        cellDescription.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Auto, 1);
        //cellDescription.Blocks.AddParagraph().Inlines.AddRun(HttpUtility.HtmlEncode(description));
        AddContentToTableCell(cellDescription, description);
        /*var newContent = GetHtmlContent(description);
        foreach (var block in newContent.Sections.First().Blocks)
        {
            cellDescription.Blocks.Add(CloneBlock(block, cellDescription.Document));
        }*/

        var cellMarks = row.Cells.AddTableCell();
        cellMarks.Blocks.AddParagraph().Inlines.AddRun($"");
        cellMarks.PreferredWidth = new TableWidthUnit(5);
    }

    private static void AddOrRow(Table table)
    {
        // Add section
        var sectionRow = table.Rows.AddTableRow();
        var cellSection = sectionRow.Cells.AddTableCell();
        cellSection.ColumnSpan = 3;
        cellSection.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);

        var paragraphSection = cellSection.Blocks.AddParagraph();
        paragraphSection.TextAlignment = Alignment.Center;
        paragraphSection.Inlines.AddRun("OR").FontWeight = FontWeights.Bold;
    }

    private static void AddBlankRow(Table table)
    {
        // Add section
        var sectionRow = table.Rows.AddTableRow();
        var cellSection = sectionRow.Cells.AddTableCell();
        cellSection.ColumnSpan = 3;
        cellSection.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);

        var paragraphSection = cellSection.Blocks.AddParagraph();
        paragraphSection.TextAlignment = Alignment.Center;
        paragraphSection.Inlines.AddRun("").FontWeight = FontWeights.Bold;
    }

    private void CreateSections(RadFlowDocumentEditor editor, Paper paper)
    {
        var structure = _structureService.GetExisting(paper);

        // Create a table with 3 columns
        var retrievedSection = editor.Document.Sections.First();

        //var table = editor.InsertTable();
        var table = retrievedSection.Blocks.AddTable();
        table.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);

        for (var sectionIndex = 0; sectionIndex < structure.NoOfSections; sectionIndex++)
        {
            var sectionNo = sectionIndex + 1;
            // Add Section row
            AddSectionRow(table, sectionNo);

            foreach (var structureDetail in structure.StructureDetails.Where(d => d.SectionNo == sectionNo))
            {
                var paperDetails = paper.PaperDetails.Where(d => d.QuestionNo == structureDetail.SerialNo)
                    .ToList();

                switch (structureDetail.AttemptCount)
                {
                    case 0:
                    case 1:
                        for (var index = 0; index < paperDetails.Count; index++)
                        {
                            var paperDetail = paperDetails[index];
                            AddQuestionRow(table, structureDetail, paperDetail.Description);
                            if (index < paperDetails.Count - 1)
                                AddOrRow(table);
                        }
                        break;
                    default:
                        // Add Question text
                        string[] words = { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN" };
                        var attemptText = $"Attempt ANY {words[structureDetail.AttemptCount ?? 0]} of the following:";
                        AddQuestionRow(table, structureDetail, attemptText);
                        for (var index = 0; index < paperDetails.Count; index++)
                        {
                            var description = $"{(char)('a' + index)}) {paperDetails[index].Description}";
                            AddAttemptRow(table, description);
                        }
                        break;
                }
            }

            // Add blank row
            AddBlankRow(table);
        }
    }

    #endregion

    #region -- Public Methods --
    public RadFlowDocument CreateWordFile(Paper paper)
    {
        // Create a new RadFlowDocument
        var document = new RadFlowDocument();
        var editor = new RadFlowDocumentEditor(document);
        editor.ParagraphFormatting.TextAlignment.LocalValue = Alignment.Justified;

        var defaultStyle = editor.Document.StyleRepository.GetStyle("Normal");
        var fontFamily = ThemableFontFamily.FromFontFamily(new FontFamily("Times New Roman"));
        defaultStyle.CharacterProperties.FontFamily.LocalValue = fontFamily;
        defaultStyle.CharacterProperties.FontSize.LocalValue = Unit.PointToDip(12);

        /*// Create Section
        var section = new Section(document);
        document.Sections.Add(section);*/

        // Create document header
        CreateHeader(editor, paper);

        // Create Sections
        CreateSections(editor, paper);

        // Create document footer
        CreateFooter(editor, paper);

        return document;
    }
    #endregion
}




