using Corno.Data.Corno.Question_Bank.Models;
using Corno.Globals.Enums;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Kendo.Mvc.Extensions;
using Telerik.Windows.Documents.Flow.FormatProviders.Docx;
using Telerik.Windows.Documents.Flow.FormatProviders.Html;
using Telerik.Windows.Documents.Flow.FormatProviders.Pdf;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.Windows.Documents.Flow.Model.Editing;
using Telerik.Windows.Documents.Flow.Model.Lists;
using Telerik.Windows.Documents.Flow.Model.Styles;
using Telerik.Windows.Documents.Primitives;
using Section = Telerik.Windows.Documents.Flow.Model.Section;

namespace Corno.Services.Corno.Question_Bank;

public class TelerikDocumentService : BaseService, ITelerikDocumentService
{
    #region -- Constructors --

    public TelerikDocumentService(ICourseService courseService, ICoursePartService coursePartService,
        ISubjectService subjectService, IStructureService structureService)
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
    private void BuildHeader(Section section, Paper paper)
    {
        var course = _courseService.GetById(paper.CourseId ?? 0);
        var coursePart = _coursePartService.GetById(paper.CoursePartId ?? 0);
        var subject = _subjectService.GetById(paper.SubjectId ?? 0);

        // Compute Max Marks (same logic you had)
        var maxMarks = subject?.SubjectCategoryDetails?
            .Where(d => _theoryCategories.Contains(d.CategoryId))
            .Sum(d => (int?)d.MaxMarks) ?? 0;

        // Create/get default header for this section
        var header = section.Headers.Default ?? section.Headers.Add();
        section.HeaderTopMargin = 36; // optional: 0.375" top padding

        // We’ll use small tables to align left/right fields neatly.
        // 1) First row: "Total No. of Questions" (left) and "Seat No." (right)
        var topTable = header.Blocks.AddTable();
        topTable.TableCellPadding = new Padding(0);
        topTable.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);

        var row1 = topTable.Rows.AddTableRow();
        var c1 = row1.Cells.AddTableCell();
        var c2 = row1.Cells.AddTableCell();
        c1.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 65);
        c2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 35);

        var pLeft = c1.Blocks.AddParagraph();
        pLeft.Inlines.AddRun($"Total No. of Questions: {paper.NoOfQuestions}");

        var pRight = c2.Blocks.AddParagraph();
        pRight.TextAlignment = Alignment.Right;
        pRight.Inlines.AddRun("Seat No.: __________________");

        // Spacer
        header.Blocks.AddParagraph();

        // 2) Main centered title block (bold, centered)
        AddCenteredBold(header, "BHARATI VIDYAPEETH (DEEMED TO BE UNIVERSITY) PUNE, INDIA");
        if (!string.IsNullOrWhiteSpace(course?.Name))
            AddCenteredBold(header, course.Name.ToUpperInvariant());
        if (!string.IsNullOrWhiteSpace(coursePart?.Name))
            AddCenteredBold(header, coursePart.Name);

        if (!string.IsNullOrWhiteSpace(subject?.Name))
        {
            var subj = $"SUBJECT : ({subject.Code}) {subject.Name.ToUpperInvariant()}";
            AddCenteredBold(header, subj);
        }

        if ((paper.BranchId ?? 0) > 0)
        {
            var branchService = Bootstrapper.Bootstrapper.Get<IBranchService>();
            var branch = branchService.GetById(paper.BranchId ?? 0);
            if (!string.IsNullOrWhiteSpace(branch?.Name))
                AddCenteredBold(header, $"Branch : {branch.Name}");
        }

        // Spacer
        header.Blocks.AddParagraph();

        // 3) Day/Time and Date/Max Marks (left/right)
        var bottomTable = header.Blocks.AddTable();
        bottomTable.TableCellPadding = new Padding(0);
        bottomTable.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);

        var row2 = bottomTable.Rows.AddTableRow();
        var l2 = row2.Cells.AddTableCell();
        var r2 = row2.Cells.AddTableCell();
        l2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 50);
        r2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 50);

        // Left: Day / Date
        l2.Blocks.AddParagraph().Inlines.AddRun("Day:");
        l2.Blocks.AddParagraph().Inlines.AddRun("Date:");

        // Right: Time / Max Marks (right-aligned)
        var timeP = r2.Blocks.AddParagraph();
        timeP.TextAlignment = Alignment.Right;
        timeP.Inlines.AddRun("Time:");

        var marksP = r2.Blocks.AddParagraph();
        marksP.TextAlignment = Alignment.Right;
        marksP.Inlines.AddRun($"Max Marks: {maxMarks}");

        AddInstructionsHeader_Best(section.Document, section, paper);
    }

    public void BuildBody(RadFlowDocument document, Paper paper)
    {
        var editor = new RadFlowDocumentEditor(document);
        var table = editor.InsertTable(1, 3);
        // Make the table span the page width (optional but nice)
        table.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);

        var structure = _structureService.GetExisting(paper);
        var sectionNos = structure.StructureDetails.Select(d => d.SectionNo ?? 0)
            .Distinct()
            .ToList();

        foreach (var sectionNo in sectionNos.OrderBy(n => n))
        {
            var isSectionMcQ = structure.StructureDetails
                .All(d => d.SectionNo == sectionNo && d.QuestionTypeId == (int)QuestionType.Mcq);
            if (isSectionMcQ)
            {
                AddSectionRow(editor, table, sectionNo);
                /*AddMcqQuestionRow(document, tableSections, structure.StructureDetails.FirstOrDefault(),
                    "MCQs");*/
            }
            else if (sectionNos.Count > 1)
            {
                AddSectionRow(editor, table, sectionNo);
            }

            var questionGroups = structure.StructureDetails
                .Where(d => d.SectionNo == sectionNo)
                .GroupBy(d => d.QuestionNo);
            foreach (var questionGroup in questionGroups)
            {
                var paperDetails = paper.PaperDetails.Where(d =>
                    d.SectionNo == sectionNo &&
                    d.QuestionNo == questionGroup.Key).ToList();

                var structureDetail = questionGroup.First();

                var questionType = ((QuestionType)(structureDetail.QuestionTypeId ?? 0)).GetName().SplitPascalCase();
                AddQuestionRow(editor, table, structureDetail, paperDetails.FirstOrDefault()?.Description);
            }
        }
    }

    private void AddSectionRow(RadFlowDocumentEditor editor, Table table, int sectionNo)
    {
        string[] romanNumerals = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };

        var row = table.Rows.AddTableRow();
        var middleCell = row.Cells[1];
        // RIGHT cell: marks, right-aligned
        var marksP = middleCell.Blocks.AddParagraph();
        marksP.TextAlignment = Alignment.Right;
        marksP.Inlines.AddRun($"SECTION - {romanNumerals[sectionNo - 1]}");
    }

    private void AddQuestionRow(RadFlowDocumentEditor editor, Table table,
        StructureDetail structureDetail, string htmlText, int questionCount = 1)
    {
        // Set fixed width for the first column
        // Add Question
        var row = table.Rows.AddTableRow();
        row.Cells.AddTableCell();
        row.Cells.AddTableCell();
        row.Cells.AddTableCell();

        var leftCell = row.Cells[0];
        var middleCell = row.Cells[1];
        var rightCell = row.Cells[2];

        // LEFT cell: "Q{n}. " + question HTML
        var leftCellParagraph = leftCell.Blocks.AddParagraph();
        leftCellParagraph.Spacing.SpacingBefore = 6;
        leftCellParagraph.Spacing.SpacingAfter = 2;

        var label = leftCellParagraph.Inlines.AddRun($"Q{structureDetail.QuestionNo}. ");
        label.FontWeight = FontWeights.Bold;

        // Make sure the caret is inside the paragraph in the left cell
        editor.MoveToParagraphEnd(leftCellParagraph);

        // Your existing pipeline that inserts the question HTML
        // (MathType, images, etc. will flow inside this left cell)
        InsertHtmlAtCaret(editor, htmlText, continueInSameParagraph: true);
        //middleCell.Blocks.AddParagraph().Inlines.AddRun(htmlText);

        // RIGHT cell: marks, right-aligned
        var marksP = rightCell.Blocks.AddParagraph();
        marksP.TextAlignment = Alignment.Right;
        marksP.Inlines.AddRun(questionCount > 1 ? $"({structureDetail.Marks}x{structureDetail.AttemptCount})" : $"({structureDetail.Marks})");

        // Optional visual gap after each question row
        editor.MoveToTableEnd(table);
        var gap = editor.InsertParagraph();
        gap.Spacing.SpacingAfter = 4;
    }

    /*public static void BuildBody(RadFlowDocument document, Paper paper,
        bool sectionNameAsAlpha = true) // A, B, C… or numeric
    {
        var section = document.Sections.FirstOrDefault() ?? document.Sections.AddSection();
        section.PageMargins = new Padding(72); // 1" margins

        var editor = new RadFlowDocumentEditor(document);

        foreach (var grp in paper.PaperDetails
                     .OrderBy(d => d.SectionNo)
                     .ThenBy(d => d.QuestionNo)
                     .GroupBy(d => d.SectionNo))
        {
            // --- SECTION HEADING ---
            var sectionTitle = sectionNameAsAlpha
                ? $"SECTION {ToAlpha(grp.Key ?? 0)}"
                : $"SECTION {grp.Key}";

            AddCenteredBoldHeading(editor, sectionTitle);

            // (Optional) small spacing after heading
            var spacer = editor.InsertParagraph();
            spacer.Spacing.SpacingAfter = 4;

            // --- QUESTIONS UNDER THIS SECTION ---

            foreach (var q in grp)
            {
                // 2-col table: [Q + question HTML] | [marks]
                var table = editor.InsertTable(1, 2);
                // Make the table span the page width (optional but nice)
                table.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);

                var row = table.Rows[0];
                var leftCell = row.Cells[0];
                var rightCell = row.Cells[1];

                // LEFT cell: "Q{n}. " + question HTML
                var p = leftCell.Blocks.AddParagraph();
                p.Spacing.SpacingBefore = 6;
                p.Spacing.SpacingAfter = 2;

                var label = p.Inlines.AddRun($"Q{q.QuestionNo}. ");
                label.FontWeight = FontWeights.Bold;

                // Make sure the caret is inside the paragraph in the left cell
                editor.MoveToParagraphEnd(p);

                // Your existing pipeline that inserts the question HTML
                // (MathType, images, etc. will flow inside this left cell)
                InsertHtmlAtCaret(editor, q.Description, continueInSameParagraph: true);

                // RIGHT cell: marks, right-aligned
                var marksP = rightCell.Blocks.AddParagraph();
                marksP.TextAlignment = Alignment.Right;
                marksP.Inlines.AddRun($"{q.Marks} (Marks)");

                // Optional visual gap after each question row
                editor.MoveToTableEnd(table);
                var gap = editor.InsertParagraph();
                gap.Spacing.SpacingAfter = 4;
            }

            /*foreach (var q in grp)
            {
                // "Q" label line (e.g., "Q1.") + question HTML right after it
                var p = editor.InsertParagraph();
                p.Spacing.SpacingBefore = 6;
                p.Spacing.SpacingAfter = 2;

                var label = p.Inlines.AddRun($"Q{q.QuestionNo}. ");
                label.FontWeight = FontWeights.Bold;

                // Insert the question body from HTML directly after the label
                InsertHtmlAtCaret(editor, q.Description, continueInSameParagraph: true);

                // add a trailing paragraph for spacing between questions
                var gap = editor.InsertParagraph();
                gap.Spacing.SpacingAfter = 4;
            }#1#
        }
    }*/

    private static void BuildFooter(Section section)
    {
        var footer = section.Footers.Add(); // default footer
        var p = footer.Blocks.AddParagraph();
        p.TextAlignment = Alignment.Center;
        p.Inlines.AddRun("*******"); // as requested
    }

    

    // Heading helper: centered & bold
    private static void AddCenteredBoldHeading(RadFlowDocumentEditor editor, string text)
    {
        var p = editor.InsertParagraph();
        p.TextAlignment = Alignment.Center;
        p.Spacing.SpacingBefore = 10;
        p.Spacing.SpacingAfter = 6;

        var r = p.Inlines.AddRun(text);
        r.FontWeight = FontWeights.Bold;
        r.FontSize = 14; // tweak as needed
    }

    // HTML import + merge at caret using Telerik providers
    private static void InsertHtmlAtCaret(RadFlowDocumentEditor editor, string html,
                                          bool continueInSameParagraph)
    {
        var htmlProvider = new HtmlFormatProvider();
        var fragment = htmlProvider.Import(html); // HTML → RadFlowDocument [2](https://docs.telerik.com/devtools/document-processing/libraries/radwordsprocessing/model/paragraph)

        var options = new InsertDocumentOptions
        {
            ConflictingStylesResolutionMode = ConflictingStylesResolutionMode.UseTargetStyle,
            InsertLastParagraphMarker = !continueInSameParagraph
        };

        editor.InsertDocument(fragment, options); // merge fragment at caret [3](https://stackoverflow.com/questions/11232113/how-do-i-use-a-custom-css-stylesheet-for-telerik)
    }

    private static string ToAlpha(int n)
    {
        // 1 -> A, 2 -> B, ... supports AA if needed
        string s = "";
        int x = n;
        while (x > 0)
        {
            x--; s = (char)('A' + (x % 26)) + s; x /= 26;
        }
        return s;
    }

    // Call this AFTER you build the title part of the header:
    void AddInstructionsInHeader(Section section, Paper paper)
    {
        // Ensure a header exists
        var header = section.Headers.Default ?? section.Headers.Add();

        // ---- Horizontal line BEFORE instructions ----
        AddHorizontalRule(header); // <— renders a thin line across the page

        // Small spacing
        header.Blocks.AddParagraph();

        // NB: line
        var nb = header.Blocks.AddParagraph();
        nb.Inlines.AddRun("NB :").FontWeight = FontWeights.Bold;

        // Get instructions for this subject/category
        var subject = _subjectService.GetById(paper.SubjectId ?? 0);
        var items = subject?.SubjectInstructionDetails?
                        .Where(d => d.PaperCategoryId == paper.PaperCategoryId)
                        .OrderBy(d => d.SerialNo)
                        .ToList() ?? new();

        // Two-column, borderless table (first col narrow for 1., 2., ...)
        var table = header.Blocks.AddTable();
        table.TableCellPadding = new Padding(0);
        table.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);
        // Remove table borders
        table.Borders = new TableBorders(new Border(BorderStyle.None));  // none on all sides
                                                                         // (Tables can be inserted in headers; Table API: blocks in Section/Header support tables.) [4](https://docs.telerik.com/devtools/document-processing/libraries/radwordsprocessing/model/table)

        int i = 1;
        foreach (var instr in items)
        {
            var row = table.Rows.AddTableRow();
            var cNo = row.Cells.AddTableCell();
            var cTxt = row.Cells.AddTableCell();

            // Widths: ~18 DIP (≈ 0.19") for numbering; text fills remaining
            cNo.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, 18);
            cTxt.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);

            var pNo = cNo.Blocks.AddParagraph();
            pNo.Inlines.AddRun($"{i++}.");

            var pTxt = cTxt.Blocks.AddParagraph();
            pTxt.Spacing.LineSpacingType = HeightType.Auto; // default line height rule
            pTxt.Inlines.AddRun(instr.Description);
        }

        // Small spacing
        header.Blocks.AddParagraph();

        // ---- Horizontal line AFTER instructions ----
        AddHorizontalRule(header); // <— closing line

        // Optionally add a tiny spacer paragraph to ensure separation from the body
        var spacer = header.Blocks.AddParagraph();
        spacer.Spacing.SpacingAfter = 4;
    }

    // Renders a full-width “HR” using the documented table-top-border workaround.
    static void AddHorizontalRule(Header header, double thickness = 1.0)
    {
        var hr = header.Blocks.AddTable();
        hr.TableCellPadding = new Padding(0);
        hr.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);

        // A single empty row/cell is enough
        var r = hr.Rows.AddTableRow();
        r.Cells.AddTableCell();

        // Top border only => draws a horizontal line
        // (RadWordsProcessing has no native <hr>, Telerik suggests a top-border table.) [1](https://www.telerik.com/forums/horizontal-line-in-radwordsprocessing)
        var topLine = new Border(BorderStyle.Single);   // defaults fine; adjust style if you want
        hr.Borders = new TableBorders(null, null, null, null);
    }

    void AddInstructionsHeader_Best(RadFlowDocument document, Section section,
        Paper paper)
    {
        var header = section.Headers.Default ?? section.Headers.Add();

        // 1) Horizontal line above (recommended workaround by Telerik)
        AddHorizontalRuleTable(header); // [3](https://docs.telerik.com/devtools/wpf/api/telerik.windows.documents.layout.padding)

        // Small spacer
        header.Blocks.AddParagraph();

        // 2) NB: label
        var nb = header.Blocks.AddParagraph();
        nb.Inlines.AddRun("NB :").FontWeight = FontWeights.Bold;

        // 3) Numbered list for instructions
        var subject = _subjectService.GetById(paper.SubjectId ?? 0);
        var items = subject?.SubjectInstructionDetails?
            .Where(d => d.PaperCategoryId == paper.PaperCategoryId)
            .OrderBy(d => d.SerialNo)
            .ToList() ?? new();

        // Create a standard numbered list (1., 2., 3., ...)
        var list = document.Lists.Add(ListTemplateType.NumberedDefault);  // real list, not a table [1](https://docs.telerik.com/devtools/document-processing/libraries/radwordsprocessing/concepts/lists)

        foreach (var instr in items)
        {
            var p = header.Blocks.AddParagraph();
            p.ListId = list.Id;      // apply numbering
            p.ListLevel = 0;
            p.Spacing.SpacingBefore = 0;
            p.Spacing.SpacingAfter = 0;
            p.Inlines.AddRun(instr.Description);
        }

        // Spacer
        header.Blocks.AddParagraph();

        // 4) Horizontal line below
        AddHorizontalRuleTable(header); // [3](https://docs.telerik.com/devtools/wpf/api/telerik.windows.documents.layout.padding)
    }

    // HR using Telerik’s recommended technique: a 1-row table with a top border.
    static void AddHorizontalRuleTable(Header header, double thicknessDip = 1.0)
    {
        var hr = header.Blocks.AddTable();
        hr.TableCellPadding = new Padding(0);
        hr.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);

        // a single cell row is enough
        var row = hr.Rows.AddTableRow();
        row.Cells.AddTableCell();

        // top border only -> a clean line across the width
        var top = new Border(BorderStyle.Single);  // simple, consistent line
                                                   // (Thickness can be adjusted with style if needed; the table approach is what Telerik suggests for 'horizontal line'.) [3](https://docs.telerik.com/devtools/wpf/api/telerik.windows.documents.layout.padding)
        hr.Borders = new TableBorders(null, null, null, null);
    }



    // Helper: add a centered bold paragraph line into the header
    static void AddCenteredBold(Header header, string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        var p = header.Blocks.AddParagraph();
        p.TextAlignment = Alignment.Center;
        p.Spacing.SpacingBefore = 2;
        p.Spacing.SpacingAfter = 2;
        var r = p.Inlines.AddRun(text);
        r.FontWeight = FontWeights.Bold;
    }


    private static void InsertQuestionHeading(RadFlowDocumentEditor editor, int qNumber)
    {
        var p = editor.InsertParagraph();
        p.Spacing.SpacingBefore = 8;
        p.Spacing.SpacingAfter = 4;
        var run = p.Inlines.AddRun($"Q{qNumber}. ");
        run.FontWeight = FontWeights.Bold;
    }

    private void InsertOrSeparator(RadFlowDocumentEditor editor)
    {
        var p = editor.InsertParagraph();
        p.TextAlignment = Alignment.Center;
        var r = p.Inlines.AddRun("OR");
        r.FontWeight = FontWeights.Bold;
    }

    private void InsertHtml(RadFlowDocumentEditor editor, string html, bool continueInSameParagraph = false)
    {
        var htmlProvider = new HtmlFormatProvider();
        // Import HTML to a temporary RadFlowDocument
        var fragmentDoc = htmlProvider.Import(html); // imports HTML into RadFlowDocument [5](https://docs.telerik.com/devtools/document-processing/libraries/radwordsprocessing/formats-and-conversion/html/htmlformatprovider)
        // Insert the fragment at the current editor caret position
        var opts = new InsertDocumentOptions
        {
            ConflictingStylesResolutionMode = ConflictingStylesResolutionMode.UseTargetStyle,
            InsertLastParagraphMarker = !continueInSameParagraph
        };
        editor.InsertDocument(fragmentDoc, opts); // merge imported content at caret [6](https://docs.telerik.com/devtools/document-processing/libraries/radwordsprocessing/editing/insert-documents)
    }

    public byte[] ExportDocx(RadFlowDocument doc)
    {
        var provider = new DocxFormatProvider();
        return provider.Export(doc); // export to DOCX [2](https://docs.telerik.com/devtools/document-processing/libraries/radwordsprocessing/formats-and-conversion/word-file-formats/docx/docxformatprovider)
    }

    public byte[] ExportPdf(RadFlowDocument doc)
    {
        var provider = new PdfFormatProvider();
        return provider.Export(doc); // export to PDF from RadFlowDocument [3](https://docs.telerik.com/devtools/document-processing/libraries/radwordsprocessing/formats-and-conversion/pdf/pdfformatprovider)
    }
    #endregion

    #region -- Public Methods --
    public RadFlowDocument Build(Paper paper)
    {
        var document = new RadFlowDocument();
        var section = document.Sections.AddSection();
        section.PageMargins = new Padding(72, 72, 72, 72); // 1" margins

        BuildHeader(section, paper);

        BuildBody(document, paper);

        BuildFooter(section);

        /*var editor = new RadFlowDocumentEditor(doc);

        // Title block
        var title = editor.InsertParagraph();
        title.TextAlignment = Alignment.Center;
        title.Inlines.AddRun(subject.SubjectName).FontWeight = FontWeights.Bold;
        editor.InsertParagraph(); // spacing

        // Optional meta line under title
        var meta = editor.InsertParagraph();
        meta.TextAlignment = Alignment.Center;
        meta.Inlines.AddRun(
            $"Branch: {subject.BranchName}   |   Max Marks: {subject.MaxMarks}   |   Date: {subject.ExamDate:dd-MMM-yyyy}   |   Time: {subject.ExamTimeText}"
        ).FontSize = 11;

        editor.InsertParagraph(); // spacing

        // Number regular (single) questions
        int qNumber = 1;

        // 1) Render "OR" sets
        foreach (var orSet in questions.Where(q => q.Kind == QuestionKind.OrAlternative && q.OrGroupId.HasValue)
                     .GroupBy(q => q.OrGroupId).OrderBy(g => g.Min(x => x.SequenceNo)))
        {
            InsertQuestionHeading(editor, qNumber++);
            int i = 0;
            foreach (var alt in orSet.OrderBy(x => x.SequenceNo))
            {
                if (i > 0) InsertOrSeparator(editor);
                InsertHtml(editor, alt.DescriptionHtml);
                i++;
            }
            editor.InsertParagraph(); // spacing after the block
        }

        // 2) Render "Attempt any N" groups
        foreach (var anyOf in questions.Where(q => q.Kind == QuestionKind.AnyOfGroup && q.GroupId.HasValue)
                     .GroupBy(q => new { q.GroupId, q.AttemptCount })
                     .OrderBy(g => g.Min(x => x.SequenceNo)))
        {
            InsertQuestionHeading(editor, qNumber++);
            var lead = editor.InsertParagraph();
            lead.Inlines.AddRun($"Attempt any {anyOf.Key.AttemptCount} of the following:").FontWeight = FontWeights.Bold;

            int optionIndex = 1;
            foreach (var q in anyOf.OrderBy(x => x.SequenceNo))
            {
                var opt = editor.InsertParagraph();
                opt.Indentation.LeftIndent = 24; // indent subitems
                opt.Inlines.AddRun($"{optionIndex}) ").FontWeight = FontWeights.Bold;
                InsertHtml(editor, q.DescriptionHtml, continueInSameParagraph: true);
                optionIndex++;
            }
            editor.InsertParagraph();
        }

        // 3) Render remaining singles
        foreach (var q in questions.Where(q => q.Kind == QuestionKind.Single
                                               && !q.GroupId.HasValue && !q.OrGroupId.HasValue)
                     .OrderBy(x => x.SequenceNo))
        {
            InsertQuestionHeading(editor, qNumber++);
            InsertHtml(editor, q.DescriptionHtml);
            editor.InsertParagraph();
        }*/

        return document;
    }
    #endregion


}