using Corno.Data.Corno.Question_Bank;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using DevExpress.Drawing.Printing;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using EnumsNET;
using Kendo.Mvc.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Document = DevExpress.XtraRichEdit.API.Native.Document;
using ParagraphAlignment = DevExpress.XtraRichEdit.API.Native.ParagraphAlignment;
using Table = DevExpress.XtraRichEdit.API.Native.Table;

namespace Corno.Services.Corno.Question_Bank;

public class RichEditDocumentService : BaseService, IRichEditDocumentService
{
    #region -- Constructors --

    public RichEditDocumentService(ICourseService courseService,
        ICoursePartService coursePartService, ISubjectService subjectService, IStructureService structureService)
    {
        _courseService = courseService;
        _coursePartService = coursePartService;
        _subjectService = subjectService;
        _structureService = structureService;

        _defaultFont = "Times New Roman";
    }

    #endregion

    #region -- Data Members --

    private readonly ICourseService _courseService;
    private readonly ICoursePartService _coursePartService;
    private readonly ISubjectService _subjectService;
    private readonly IStructureService _structureService;

    private readonly string _defaultFont;
    readonly List<int> _theoryCategories = new() { 2, 48 };
    #endregion

    #region -- Private Methods --

    private void CreateHeader(Paper paper, Document document)
    {
        var course = _courseService.GetById(paper.CourseId ?? 0);
        var coursePart = _coursePartService.GetById(paper.CoursePartId ?? 0);
        var subject = _subjectService.GetById(paper.SubjectId ?? 0);

        document.BeginUpdate();
        document.AppendText($"Total No. of Questions: {paper.NoOfQuestions}\t\t\t\t\t\t\t\t\t Seat No.:\n\n");
        document.AppendText("");
        var range = document.AppendText("BHARATI VIDYAPEETH (DEEMED TO BE UNIVERSITY) PUNE, INDIA\n" +
                                        $"{course?.Name?.ToUpper()}\n" +
                                        $"{coursePart.Name}\n " +
                                        $"SUBJECT : ({subject.Code}) {subject.Name?.ToUpper()}");
        if ((paper.BranchId ?? 0) > 0)
        {
            var branchService = Bootstrapper.Bootstrapper.Get<IBranchService>();
            var branch = branchService.GetById(paper.BranchId ?? 0);
            document.AppendText($"\nBranch : {branch.Name}");
        }
        document.InsertText(document.Range.End, "\n");

        // Make text bold and center
        MakeTextBoldAndCenter(document, range);

        var maxMarks = subject.SubjectCategoryDetails
            .Where(d => _theoryCategories.Contains(d.CategoryId))
            .Sum(d => d.MaxMarks);
        document.AppendText("Day: \t\t\t\t\t\t\t\t\t\t\t\t Time:\n");
        document.AppendText($"Date: \t\t\t\t\t\t\t\t\t\t\t\t Max Marks: {maxMarks}");
        //document.AppendText($"Date: \t\t\t\t\t\t\t\t\t\t\t\t Max Marks:  {paper.MaxMarks}");

        // Add horizontal line
        //AddHorizontalLine(document);

        //var blankRange = document.InsertText(document.Range.End, "\n");

        // Add Instructions
        AddInstructions(document, paper);

        document.InsertText(document.Range.End, " ");

        // Add horizontal line
        //AddHorizontalLine(document);

        document.InsertText(document.Range.End, "\n");
    }

    private void CreateSections(Paper paper, Document document)
    {
        var tableSections = document.Tables.Create(document.Range.End, 1, 3);
        tableSections.MarginRight = 0f;
        tableSections.MarginRight = 0f;

        var structure = _structureService.GetExisting(paper);
        var sectionNos = structure.StructureDetails.Select(d => d.SectionNo ?? 0)
            .Distinct()
            .ToList();
        //for (var sectionIndex = 0; sectionIndex < structure.NoOfSections; sectionIndex++)
        foreach (var sectionNo in sectionNos)
        {
            /*var sectionNo = sectionIndex + 1;

            // Add Section row
            if (structure.NoOfSections > 1)
                AddSectionRow(document, tableSections, sectionNo);*/
            var isSectionMcQ = structure.StructureDetails
                .All(d => d.SectionNo == sectionNo && d.QuestionTypeId == (int)QuestionType.Mcq);
            if (isSectionMcQ)
            {
                AddSectionRow(document, tableSections, sectionNo);
                AddMcqQuestionRow(document, tableSections, structure.StructureDetails.FirstOrDefault(),
                    "MCQs");
            }
            else if (sectionNos.Count > 1)
            {
                AddSectionRow(document, tableSections, sectionNo);
            }

            var questionGroups = structure.StructureDetails
                .Where(d => d.SectionNo == sectionNo)
                .GroupBy(d => d.QuestionNo);
            foreach (var questionGroup in questionGroups)
            {
                /*var paperDetails = paper.PaperDetails.Where(d =>
                        d.QuestionNo == structureDetail.SerialNo).ToList();*/
                var paperDetails = paper.PaperDetails.Where(d =>
                    d.SectionNo == sectionNo &&
                    d.QuestionNo == questionGroup.Key)
                    .OrderBy(d => d.SerialNo).ToList();

                var structureDetail = questionGroup.First();

                var questionType = ((QuestionType)(structureDetail.QuestionTypeId ?? 0)).GetName().SplitPascalCase();
                switch (structureDetail.AttemptCount)
                {
                    case 0:
                    case 1:
                        switch ((QuestionType)(structureDetail.QuestionTypeId ?? 0))
                        {
                            case QuestionType.ShortAnswer:
                            case QuestionType.ShortNotes:
                                // Add Question text
                                string[] words =
                                    { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN" };
                                var attemptText = $"Attempt ANY {words[structureDetail.AttemptCount ?? 0]} of the following: ({questionType})";
                                AddQuestionRow(document, tableSections, structureDetail, attemptText, paperDetails.Count);
                                AddAttemptRow(document, tableSections, paperDetails);
                                break;
                            case QuestionType.Mcq:
                                //foreach (var htmlText in paperDetails.Select(paperDetail => ConvertByteArrayToHtml(paperDetail.DocumentContent)))
                                foreach (var paperDetail in paperDetails)
                                {
                                    var htmlText = ConvertByteArrayToHtml(paperDetail.DocumentContent);
                                    AddMcqRow(document, tableSections, paperDetail, htmlText);
                                }
                                break;
                            default:
                                for (var index = 0; index < paperDetails.Count; index++)
                                {
                                    var paperDetail = paperDetails[index];
                                    var htmlText = ConvertByteArrayToHtml(paperDetail.DocumentContent);
                                    AddQuestionRow(document, tableSections, structureDetail, htmlText);
                                    if (index < paperDetails.Count - 1)
                                        AddOrRow(document, tableSections);
                                }
                                break;
                        }
                        /*if (structureDetail.QuestionTypeId.ToInt() == (int)QuestionType.LongAnswer)
                        {
                            for (var index = 0; index < paperDetails.Count; index++)
                            {
                                var paperDetail = paperDetails[index];
                                var htmlText = ConvertByteArrayToHtml(paperDetail.DocumentContent);
                                AddQuestionRow(document, tableSections, structureDetail, htmlText);
                                if (index < paperDetails.Count - 1)
                                    AddOrRow(document, tableSections);
                            }
                        }
                        else
                        {
                            // Add Question text
                            string[] words =
                                { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN" };
                            var attemptText = $"Attempt ANY {words[structureDetail.AttemptCount ?? 0]} of the following: ({questionType})";
                            AddQuestionRow(document, tableSections, structureDetail, attemptText, paperDetails.Count);
                            AddAttemptRow(document, tableSections, paperDetails);
                        }*/
                        break;
                    default:
                        {
                            // Add Question text
                            string[] words =
                            {
                                "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN"
                            };
                            var attemptText =
                                $"Attempt ANY {words[structureDetail.AttemptCount ?? 0]} of the following: ({questionType})";
                            AddQuestionRow(document, tableSections, structureDetail, attemptText, paperDetails.Count);
                            AddAttemptRow(document, tableSections, paperDetails);
                        }
                        break;
                }
                /*switch (structureDetail.AttemptCount)
                {
                    case 0:
                    case 1:
                        for (var index = 0; index < paperDetails.Count; index++)
                        {
                            var paperDetail = paperDetails[index];
                            var htmlText = ConvertByteArrayToHtml(paperDetail.DocumentContent);
                            AddQuestionRow(document, tableSections, structureDetail, htmlText);
                            if (index < paperDetails.Count - 1)
                                AddOrRow(document, tableSections);
                        }

                        break;
                    default:
                        // Add Question text
                        string[] words =
                            { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN" };
                        var attemptText = $"Attempt ANY {words[structureDetail.AttemptCount ?? 0]} of the following:";
                        AddQuestionRow(document, tableSections, structureDetail, attemptText, paperDetails.Count);
                        AddAttemptRow(document, tableSections, paperDetails);
                        /*for (var index = 0; index < paperDetails.Count; index++)
                        {
                            var description = $"{(char)('a' + index)}) {paperDetails[index].Description}";
                            AddAttemptRow(document, table, description);
                        }#1#
                        break;
                }*/

                AddBlankRow(tableSections);
            }

            // Add blank row
            //AddBlankRow(table);
        }

        // Add End ***** row
        AddEndRow(document, tableSections);

        // Make table border less
        SetTableBorder(tableSections, TableBorderLineStyle.Nil);
        if (tableSections.Rows.Count > 1)
            tableSections.Rows.RemoveAt(0);
    }

    private static void AddHorizontalLine(Document document)
    {
        var tableHorizontalLine = document.Tables.Create(document.Range.End, 1, 1, AutoFitBehaviorType.AutoFitToWindow);

        tableHorizontalLine.BeginUpdate();

        //tableHorizontalLine.TableLayout = TableLayoutType.Autofit;

        tableHorizontalLine.Borders.Left.LineStyle = TableBorderLineStyle.Nil;
        tableHorizontalLine.Borders.Top.LineStyle = TableBorderLineStyle.Nil;
        tableHorizontalLine.Borders.Right.LineStyle = TableBorderLineStyle.Nil;
        tableHorizontalLine.Borders.Bottom.LineStyle = TableBorderLineStyle.Single;

        tableHorizontalLine.EndUpdate();
    }

    private void AddInstructions(Document document, Paper paper)
    {
        /*// Notes below text
        document.AppendText("NB:");*/

        var subject = _subjectService.GetById(paper.SubjectId ?? 0);

        var tableInstructions = document.Tables.Create(document.Range.End, 1, 2, AutoFitBehaviorType.AutoFitToWindow);

        tableInstructions.BeginUpdate();

        // Note Below 
        var row = AddBlankRow(tableInstructions);
        document.InsertText(row.Cells[0].Range.Start, "NB :");

        var index = 1;
        foreach (var instruction in subject?.SubjectInstructionDetails
                     .Where(p => p.PaperCategoryId == paper.PaperCategoryId)
                     .OrderBy(p => p.SerialNo).ToList()!)
        {
            row = AddBlankRow(tableInstructions);
            //SetInstructionRowCellWidths(row);

            document.InsertText(row.Cells[0].Range.Start, $"{index++}.");
            document.InsertText(row.Cells[1].Range.Start, instruction.Description);
        }

        // Make table border less
        //SetTableBorder(tableInstructions, TableBorderLineStyle.Nil);
        if (tableInstructions.Rows.Count > 1)
            tableInstructions.Rows.RemoveAt(0);

        // Iterate through the rows and cells to set the borders
        foreach (var tableRow in tableInstructions.Rows)
        {
            foreach (var cell in tableRow.Cells)
            {
                if (cell.Index == 0) // For first cell
                {
                    cell.PreferredWidthType = WidthType.Fixed;
                    cell.PreferredWidth = 2F;
                }
                else
                {
                    cell.PreferredWidthType = WidthType.Auto;
                }

                // Clear all borders
                cell.Borders.Left.LineStyle = TableBorderLineStyle.None;
                cell.Borders.Right.LineStyle = TableBorderLineStyle.None;
                cell.Borders.Top.LineStyle = TableBorderLineStyle.None;
                cell.Borders.Bottom.LineStyle = TableBorderLineStyle.None;

                // Set top border for the first row
                if (tableRow.Index == 0)
                {
                    cell.Borders.Top.LineStyle = TableBorderLineStyle.Single;
                }

                // Set bottom border for the last row
                if (tableRow.Index == tableInstructions.Rows.Count - 1)
                {
                    cell.Borders.Bottom.LineStyle = TableBorderLineStyle.Single;
                }
            }

            // Merge all cells of first row.
            if (tableRow.Index == 0)
                tableInstructions.MergeCells(tableRow.Cells[0], tableRow.Cells[1]);
        }
        /*tableInstructions.TableLayout = TableLayoutType.Autofit;
        //tableInstructions.PreferredWidthType = WidthType.None;
        tableInstructions.ForEachCell((cell, rowIndex, cellIndex) =>
        {
            cell.PreferredWidthType = WidthType.Auto;
        });*/

        tableInstructions.EndUpdate();
    }

    private static TableRow AddBlankRow(Table table)
    {
        return table.Rows.Append();
    }

    private void AddSectionRow(Document document, Table table, int sectionNo)
    {
        string[] romanNumerals = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };

        var row = AddBlankRow(table);
        SetQuestionRowCellWidths(row);

        MakeTextBoldAndCenter(document, row.Cells[1].Range);

        document.InsertText(row.Cells[1].Range.Start, $"SECTION - {romanNumerals[sectionNo - 1]}");

        // Add blank row
        AddBlankRow(table);
    }

    private void AddQuestionRow(Document document, Table table,
        StructureDetail structureDetail, string htmlText, int questionCount = 1)
    {
        table.BeginUpdate();

        // Set fixed width for the first column
        // Add Question
        var row = AddBlankRow(table);
        SetQuestionRowCellWidths(row);

        // Insert first cell
        document.InsertText(row.Cells[0].Range.Start, $"Q. {structureDetail.QuestionNo}");

        // Insert second cell
        var questionRange = document.InsertHtmlText(row.Cells[1].Range.Start, htmlText);
        // Align the text to the left
        var pp = document.BeginUpdateParagraphs(questionRange);
        pp.Alignment = ParagraphAlignment.Left;
        pp.Style.FontName = _defaultFont;
        pp.Style.FontSize = 12F;
        document.EndUpdateParagraphs(pp);

        // Insert third cell
        document.InsertText(row.Cells[2].Range.Start,
            questionCount > 1 ? $"({structureDetail.Marks}x{structureDetail.AttemptCount})" : $"({structureDetail.Marks})");

        table.EndUpdate();
    }

    private void AddMcqQuestionRow(Document document, Table table,
        StructureDetail structureDetail, string htmlText)
    {
        table.BeginUpdate();

        // Set fixed width for the first column
        // Add Question
        var row = AddBlankRow(table);
        SetQuestionRowCellWidths(row);

        // Insert first cell
        document.InsertText(row.Cells[0].Range.Start, $"Q. {structureDetail.QuestionNo}");

        // Insert second cell
        var questionRange = document.InsertHtmlText(row.Cells[1].Range.Start, htmlText);
        // Align the text to the left
        var pp = document.BeginUpdateParagraphs(questionRange);
        pp.Alignment = ParagraphAlignment.Left;
        pp.Style.FontName = _defaultFont;
        pp.Style.FontSize = 12F;
        document.EndUpdateParagraphs(pp);

        table.EndUpdate();
    }

    private void AddMcqRow(Document document, Table table,
        PaperDetail paperDetail, string htmlText, int questionCount = 1)
    {
        table.BeginUpdate();

        // Set fixed width for the first column
        // Add Question
        var row = AddBlankRow(table);
        SetQuestionRowCellWidths(row);

        // Insert first cell
        document.InsertText(row.Cells[0].Range.Start, $"{paperDetail.SerialNo})");

        // Insert second cell
        var questionRange = document.InsertHtmlText(row.Cells[1].Range.Start, htmlText);
        // Align the text to the left
        var pp = document.BeginUpdateParagraphs(questionRange);
        pp.Alignment = ParagraphAlignment.Left;
        pp.Style.FontName = _defaultFont;
        pp.Style.FontSize = 12F;
        document.EndUpdateParagraphs(pp);

        // Insert third cell
        document.InsertText(row.Cells[2].Range.Start, $"({paperDetail.Marks})");

        table.EndUpdate();
    }

    private void AddOrRow(Document document, Table table)
    {
        UpdateMiddleCellOnly(document, table, "OR");
    }

    private void AddEndRow(Document document, Table table)
    {
        UpdateMiddleCellOnly(document, table, "* * * * * *");
    }

    private void UpdateMiddleCellOnly(Document document, Table table, string text)
    {
        // Add Row
        var row = AddBlankRow(table);
        SetQuestionRowCellWidths(row);

        /*row.Cells[1].Style.Bold = true;
        row.Cells[1].Style.Italic = false;
        row.Cells[1].Style.Alignment = ParagraphAlignment.Center;*/

        MakeTextBoldAndCenter(document, row.Cells[1].Range);

        document.InsertText(row.Cells[1].Range.Start, text);
    }

    // Remove only the first <p>...</p> pair, keep its inner HTML
    static string MergeFirstParagraphIntoHost(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return html;

        html = html.TrimStart();

        // Match the very first <p ...>...</p> (non-greedy)
        var m = Regex.Match(html, @"^\s*<p\b[^>]*>(.*?)</p\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (!m.Success) return html;

        // Replace the whole match with its inner content (group 1)
        return html.Substring(0, m.Index) + m.Groups[1].Value + html.Substring(m.Index + m.Length);
    }

    private void AddAttemptRow(Document document, Table table, List<PaperDetail> paperDetails)
    {
        var row = AddBlankRow(table);
        SetQuestionRowCellWidths(row);

        var tableAttempt = document.Tables.Create(row.Cells[1].Range.Start, 1, 2);
        //tableAttempt.TableLayout = TableLayoutType.Fixed;

        tableAttempt.BeginUpdate();

        var index = 0;
        foreach (var detail in paperDetails)
        {
            var attemptRow = AddBlankRow(tableAttempt);
            //SetAttemptRowCellWidths(row);
            var letter = (char)('a' + index);
            document.InsertHtmlText(attemptRow.Cells[0].Range.Start, $"{letter})");

            // Insert HTML content
            /*var htmlContent = ConvertByteArrayToHtml(detail?.DocumentContent);
            document.InsertHtmlText(attemptRow.Cells[1].Range.Start, htmlContent);*/
            if (!string.IsNullOrEmpty(detail.Description))
            {
                var html = MergeFirstParagraphIntoHost(detail.Description);
                document.InsertHtmlText(attemptRow.Cells[1].Range.Start, html);
            }
            else
            {
                var htmlContent = ConvertByteArrayToHtml(detail?.DocumentContent);
                document.InsertHtmlText(attemptRow.Cells[1].Range.Start, htmlContent);
            }

            index++;
        }

        // Make table border less
        SetTableBorder(tableAttempt, TableBorderLineStyle.Nil);
        if (tableAttempt.Rows.Count > 1)
            tableAttempt.Rows.RemoveAt(0);

        tableAttempt.EndUpdate();
    }

    private void MakeTextBoldAndCenter(Document document, DocumentRange range)
    {
        // Make the text bold and center-aligned
        var cp = document.BeginUpdateCharacters(range);
        cp.Bold = true;
        cp.Italic = false;
        cp.Underline = UnderlineType.None;
        document.EndUpdateCharacters(cp);

        var pp = document.BeginUpdateParagraphs(range);
        pp.Alignment = ParagraphAlignment.Center;
        pp.Style.FontName = _defaultFont;
        pp.Style.FontSize = 12F;
        document.EndUpdateParagraphs(pp);
    }

    private static void SetTableBorder(Table table, TableBorderLineStyle borderLineStyle)
    {
        table.Borders.Top.LineStyle = borderLineStyle;
        table.Borders.Bottom.LineStyle = borderLineStyle;
        table.Borders.Left.LineStyle = borderLineStyle;
        table.Borders.Right.LineStyle = borderLineStyle;

        foreach (var attemptRow in table.Rows)
        {
            foreach (var cell in attemptRow.Cells)
                SetTableCellBorder(cell, borderLineStyle);
        }
    }

    private static void SetInstructionRowCellWidths(TableRow row)
    {
        row.Cells[0].PreferredWidthType = WidthType.Auto;
        row.Cells[1].PreferredWidthType = WidthType.Auto;
        //SetTableCellWidth(row.Cells[0], WidthType.Fixed, TableCellVerticalAlignment.Center,true, 0.05F);
        //SetTableCellWidth(row.Cells[1], WidthType.Auto, TableCellVerticalAlignment.Center, true, 5F);
        /*SetTableCellWidth(row.Cells[0], 0.5f);
        SetTableCellWidth(row.Cells[1], 4.2F);*/
    }

    private static void SetQuestionRowCellWidths(TableRow row)
    {
        SetTableCellWidth(row.Cells[0], 0.69F);
        SetTableCellWidth(row.Cells[1], 6.4F);
        SetTableCellWidth(row.Cells[2], 0.6F);
    }

    private static void SetAttemptRowCellWidths(TableRow row)
    {
        // Set width in middle question row. Width : 5.4inch
        SetTableCellWidth(row.Cells[0], 0.4F);
        SetTableCellWidth(row.Cells[1], 6F);
    }

    private static void SetTableCellWidth(TableCell cell, WidthType widthType,
        TableCellVerticalAlignment verticalAlignment, bool wordWrap,
        float widthInInches)
    {
        cell.PreferredWidthType = widthType;
        cell.VerticalAlignment = verticalAlignment;
        cell.PreferredWidth = Units.InchesToDocumentsF(widthInInches); // 1 inch
        cell.WordWrap = wordWrap;
    }

    private static void SetTableCellWidth(TableCell cell, float widthInInches)
    {
        cell.PreferredWidthType = WidthType.Fixed;
        cell.VerticalAlignment = TableCellVerticalAlignment.Top;
        cell.PreferredWidth = Units.InchesToDocumentsF(widthInInches); // 1 inch
        cell.WordWrap = false;
    }

    private static void SetTableCellBorder(TableCell cell, TableBorderLineStyle borderLineStyle)
    {
        cell.Borders.Left.LineStyle = borderLineStyle;
        cell.Borders.Right.LineStyle = borderLineStyle;
        cell.Borders.Top.LineStyle = borderLineStyle;
        cell.Borders.Bottom.LineStyle = borderLineStyle;
    }

    private void SetDefaultFont(RichEditDocumentServer richEditDocumentServer)
    {
        // Assuming you have an instance of RichEditDocumentServer named richEditDocumentServer
        richEditDocumentServer.BeginUpdate();

        // Set the default font for the entire document
        richEditDocumentServer.Document.DefaultCharacterProperties.FontName = _defaultFont;
        richEditDocumentServer.Document.DefaultCharacterProperties.FontSize = 12; // Set the desired font size

        // Apply the font to all text ranges
        var documentRange = richEditDocumentServer.Document.Range;
        var characterProperties = richEditDocumentServer.Document.BeginUpdateCharacters(documentRange);
        characterProperties.FontName = _defaultFont;
        characterProperties.FontSize = 12;
        richEditDocumentServer.Document.EndUpdateCharacters(characterProperties);

        // Apply the font to the "Normal" paragraph style
        var normalStyle = richEditDocumentServer.Document.ParagraphStyles["Normal"];
        normalStyle.FontName = _defaultFont;
        normalStyle.FontSize = 12;

        var firstSection = richEditDocumentServer.Document.Sections[0];
        firstSection.Page.Landscape = false;
        var paperKind = new DXPaperKind();
        firstSection.Page.PaperKind = DXPaperKind.Letter;

        // Set margins to utilize header/footer space for body
        // 100f = 1 cm
        firstSection.Margins.Top = 100f;
        firstSection.Margins.Bottom = 5f;
        firstSection.Margins.Left = 150f;       // 150f = 1.5 cm 
        firstSection.Margins.Right = 50f;
        firstSection.Margins.HeaderOffset = 0f;
        firstSection.Margins.FooterOffset = 50f;

        richEditDocumentServer.EndUpdate();
    }

    private void CreateFooter(Paper paper, Document document)
    {
        var firstSection = document.Sections[0];

        // Begin updating the footer
        var footer = firstSection.BeginUpdateFooter();

        /*// Clear any existing content
        footer.Range.Clear();*/

        // Create a paragraph and set its alignment to right
        var paragraph = footer.Paragraphs.Append();
        footer.Paragraphs[paragraph.Index].Alignment = ParagraphAlignment.Right;

        // Insert page number and total page count fields
        var pageNumberRange = footer.CreateRange(paragraph.Range.Start, 0);
        footer.Fields.Create(pageNumberRange.End, " PAGE");
        footer.InsertText(pageNumberRange.End, " of ");
        footer.Fields.Create(pageNumberRange.End, "NUMPAGES ");
        footer.InsertText(pageNumberRange.End, "\t\t"); // Insert a tab before the page number

        // End updating the footer
        firstSection.EndUpdateFooter(footer);
    }

    #endregion

    #region -- Public Methods --
    public RichEditDocumentServer CreateWordFile(Paper paper)
    {
        // Load data into RichEditControl
        var richEditDocumentServer = new RichEditDocumentServer();
        var document = richEditDocumentServer.Document;

        // Change document font
        SetDefaultFont(richEditDocumentServer);

        // Create Header
        CreateHeader(paper, document);

        // Create Sections
        CreateSections(paper, document);

        // Create Header
        CreateFooter(paper, document);

        return richEditDocumentServer;
    }

    public string ConvertByteArrayToHtml(byte[] bytes)
    {
        if (bytes == null) return default;

        using var richEditDocumentServer = new RichEditDocumentServer();
        using var stream = new MemoryStream(bytes);

        if (!richEditDocumentServer.LoadDocument(stream, DocumentFormat.Rtf))
            return default;

        var document = richEditDocumentServer.Document;
        var range = document.Range;

        document.BeginUpdate();
        var paragraphProperties = document.BeginUpdateParagraphs(range);
        paragraphProperties.Alignment = ParagraphAlignment.Justify;
        document.EndUpdateParagraphs(paragraphProperties);
        document.EndUpdate();

        return richEditDocumentServer.HtmlText;

    }

    /*public string ConvertByteArrayToHtml(byte[] bytes)
    {
        if (null == bytes) return default;

        using var richEditDocumentServer = new RichEditDocumentServer();
        // Load the RTF byte array into the server
        using var stream = new MemoryStream(bytes);
        return richEditDocumentServer.LoadDocument(stream, DocumentFormat.Rtf) ?
            richEditDocumentServer.HtmlText : default;
    }*/

    public byte[] ConvertHtmlToByteArray(string htmlContent)
    {
        var document = new RichEditDocumentServer();
        document.HtmlText = htmlContent;
        using var stream = new MemoryStream();
        document.SaveDocument(stream, DocumentFormat.Html);
        return stream.ToArray();
    }

    public string ConvertByteArrayToPlainText(byte[] bytes)
    {
        if (null == bytes) return default;

        using var richEditDocumentServer = new RichEditDocumentServer();
        // Load the RTF byte array into the server
        using var stream = new MemoryStream(bytes);
        return richEditDocumentServer.LoadDocument(stream, DocumentFormat.Rtf) ?
            richEditDocumentServer.Text : default;
    }
    #endregion
}