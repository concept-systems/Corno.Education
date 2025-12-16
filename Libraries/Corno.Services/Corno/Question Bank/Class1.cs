// Services/Question_Bank/RichEditDocumentService.cs

using Corno.Data.Corno.Masters;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Globals.Enums;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using DevExpress.XtraSpellChecker.Native;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Kendo.Mvc.Extensions;
using Telerik.Windows.Documents.Flow.FormatProviders.Html;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.Windows.Documents.Flow.Model.Editing;
using Telerik.Windows.Documents.Flow.Model.Editors;
using Telerik.Windows.Documents.Flow.Model.Fields;
using Telerik.Windows.Documents.Flow.Model.Styles;
using Telerik.Windows.Documents.Media;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace Corno.Services.Corno.Question_Bank
{
    public class TelerikDocumentService : BaseService, IRichEditDocumentService
    {
        private readonly ICourseService _courseService;
        private readonly ICoursePartService _coursePartService;
        private readonly ISubjectService _subjectService;
        private readonly IStructureService _structureService;
        private readonly string _defaultFont = "Times New Roman";
        private readonly List<int> _theoryCategories = new() { 2, 48 };

        public TelerikDocumentService(
            ICourseService courseService,
            ICoursePartService coursePartService,
            ISubjectService subjectService,
            IStructureService structureService)
        {
            _courseService = courseService;
            _coursePartService = coursePartService;
            _subjectService = subjectService;
            _structureService = structureService;
        }

        public RadFlowDocument CreateWordFile(Paper paper)
        {
            var document = new RadFlowDocument();
            ApplyDocumentDefaults(document);          // page, margins, font  [3](https://feedback.telerik.com/document-processing/1356350-wordsprocessing-normal-is-not-assigned-as-a-paragraph-style-when-such-is-not-explicitly-applied-to-the-element)[8](https://docs.telerik.com/devtools/document-processing/libraries/radwordsprocessing/formats-and-conversion/html/htmlformatprovider)
            CreateHeader(paper, document);
            CreateSections(paper, document);
            CreateFooter(document);                   // PAGE of NUMPAGES     [6](https://docs.telerik.com/devtools/document-processing/libraries/radwordsprocessing/concepts/fields/numbering-fields-provider)
            return document;
        }

        // ===== Helpers =====

        private static double Inches(double i) => UnitHelper.InchToDip(i); // 1/96"  [4](https://www.telerik.com/forums/insert-document-inside-a-run-of-another-document)

        private void ApplyDocumentDefaults(RadFlowDocument document)
        {
            var section = document.Sections.AddSection();
            section.PageSize = PaperTypeConverter.ToSize(PaperTypes.Letter); // use A4 if needed  [3](https://feedback.telerik.com/document-processing/1356350-wordsprocessing-normal-is-not-assigned-as-a-paragraph-style-when-such-is-not-explicitly-applied-to-the-element)
            section.PageMargins = new Padding(
                UnitHelper.CmToDip(1.0),   // Top
                UnitHelper.CmToDip(1.5),   // Left
                UnitHelper.CmToDip(0.05),  // Bottom
                UnitHelper.CmToDip(0.5));  // Right
            section.HeaderTopMargin = UnitHelper.CmToDip(0);
            section.FooterBottomMargin = UnitHelper.CmToDip(0.5);

            var editor = new RadFlowDocumentEditor(document);
            editor.CharacterFormatting.FontFamily.LocalValue = new ThemableFontFamily(_defaultFont);
            editor.CharacterFormatting.FontSize.LocalValue = 12d;          // default font  [8](https://docs.telerik.com/devtools/document-processing/libraries/radwordsprocessing/formats-and-conversion/html/htmlformatprovider)
        }

        private void CreateHeader(Paper paper, RadFlowDocument document)
        {
            var course = _courseService.GetById(paper.CourseId ?? 0);
            var coursePart = _coursePartService.GetById(paper.CoursePartId ?? 0);
            var subject = _subjectService.GetById(paper.SubjectId ?? 0);
            var editor = new RadFlowDocumentEditor(document);

            var p1 = editor.InsertParagraph();
            MakeRun(p1, $"Total No. of Questions: {paper.NoOfQuestions}\t\t\t\t\t\t\t Seat No:\n\n", false, Alignment.Left);

            var p2 = editor.InsertParagraph();
            var title =
                "BHARATI VIDYAPEETH (DEEMED TO BE UNIVERSITY) PUNE, INDIA\n" +
                $"{course?.Name?.ToUpper()}\n" +
                $"{coursePart?.Name}\n" +
                $"SUBJECT : ({subject?.Code}) {subject?.Name?.ToUpper()}";
            MakeRun(p2, title, true, Alignment.Center);

            if ((paper.BranchId ?? 0) > 0)
            {
                var branchService = Bootstrapper.Bootstrapper.Get<IBranchService>();
                var branch = branchService.GetById(paper.BranchId ?? 0);
                var pb = editor.InsertParagraph();
                MakeRun(pb, $"Branch : {branch?.Name}", false, Alignment.Center);
            }

            var p3 = editor.InsertParagraph();
            MakeRun(p3, "Day:\t\t\t\t\t\t\t\t Time:\n", false, Alignment.Left);

            var maxMarks = subject.SubjectCategoryDetails
                                  .Where(d => _theoryCategories.Contains(d.CategoryId))
                                  .Sum(d => d.MaxMarks);
            var p4 = editor.InsertParagraph();
            MakeRun(p4, $"Date:\t\t\t\t\t\t\t\t Max Marks: {maxMarks}", false, Alignment.Left);

            AddInstructions(document, paper);
            editor.InsertParagraph();
        }

        private void CreateSections(Paper paper, RadFlowDocument document)
        {
            var section = document.Sections.First();
            var tableSections = section.Blocks.AddTable();

            var structure = _structureService.GetExisting(paper);
            var sectionNos = structure.StructureDetails
                                      .Select(d => d.SectionNo ?? 0)
                                      .Distinct()
                                      .ToList();

            foreach (var secNo in sectionNos)
            {
                var isMcq = structure.StructureDetails
                                     .Where(d => d.SectionNo == secNo)
                                     .All(d => d.QuestionTypeId == (int)QuestionType.Mcq);

                if (isMcq)
                {
                    AddSectionRow(document, tableSections, secNo);
                    AddMcqQuestionRow(document, tableSections, structure.StructureDetails.First(), "MCQs");
                }
                else if (sectionNos.Count > 1)
                {
                    AddSectionRow(document, tableSections, secNo);
                }

                var groups = structure.StructureDetails
                                      .Where(d => d.SectionNo == secNo)
                                      .GroupBy(d => d.QuestionNo);

                foreach (var g in groups)
                {
                    var paperDetails = paper.PaperDetails
                        .Where(d => d.SectionNo == secNo && d.QuestionNo == g.Key)
                        .OrderBy(d => d.SerialNo)
                        .ToList();

                    var sd = g.First();
                    var qType = ((QuestionType)(sd.QuestionTypeId ?? 0)).GetName().SplitPascalCase();

                    switch (sd.AttemptCount)
                    {
                        case 0:
                        case 1:
                            switch ((QuestionType)(sd.QuestionTypeId ?? 0))
                            {
                                case QuestionType.ShortAnswer:
                                case QuestionType.ShortNotes:
                                    {
                                        string[] words = { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN" };
                                        var attemptText = $"Attempt ANY {words[sd.AttemptCount ?? 0]} of the following: ({qType})";
                                        AddQuestionRow(document, tableSections, sd, attemptText, paperDetails.Count);
                                        AddAttemptRow(document, tableSections, paperDetails);
                                        break;
                                    }
                                case QuestionType.Mcq:
                                    {
                                        foreach (var pd in paperDetails)
                                        {
                                            var html = FirstNonEmpty(pd.HtmlContent, pd.Description);
                                            AddMcqRow(document, tableSections, pd, html);
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        for (int i = 0; i < paperDetails.Count; i++)
                                        {
                                            var pd = paperDetails[i];
                                            var html = FirstNonEmpty(pd.HtmlContent, pd.Description);
                                            AddQuestionRow(document, tableSections, sd, html);
                                            if (i < paperDetails.Count - 1) AddOrRow(document, tableSections);
                                        }
                                        break;
                                    }
                            }
                            break;

                        default:
                            {
                                string[] words = { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN" };
                                var attemptText = $"Attempt ANY {words[sd.AttemptCount ?? 0]} of the following: ({qType})";
                                AddQuestionRow(document, tableSections, sd, attemptText, paperDetails.Count);
                                AddAttemptRow(document, tableSections, paperDetails);
                                break;
                            }
                    }

                    AddBlankRow(tableSections);
                }
            }

            AddEndRow(document, tableSections);
            SetTableBorder(tableSections, BorderStyle.None);
            if (tableSections.Rows.Count > 1) tableSections.Rows.RemoveAt(0);
        }

        private static string FirstNonEmpty(params string[] inputs)
            => inputs?.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s)) ?? string.Empty;

        private static TableRow AddBlankRow(Table t) => t.Rows.AddTableRow();

        private void AddSectionRow(RadFlowDocument doc, Table t, int sectionNo)
        {
            string[] romans = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };
            var row = AddBlankRow(t);
            Ensure3Cells(row);
            SetQuestionRowCellWidths(row);

            var p = row.Cells[1].Blocks.AddParagraph();
            MakeRun(p, $"SECTION - {romans[sectionNo - 1]}", true, Alignment.Center);
            AddBlankRow(t);
        }

        private void AddQuestionRow(RadFlowDocument doc, Table t, StructureDetail sd, string htmlOrText, int questionCount = 1)
        {
            var row = AddBlankRow(t);
            Ensure3Cells(row);
            SetQuestionRowCellWidths(row);

            row.Cells[0].Blocks.AddParagraph().Inlines.AddRun($"Q. {sd.QuestionNo}");
            InsertHtmlIntoCell(row.Cells[1], htmlOrText);

            var marks = questionCount > 1 ? $"({sd.Marks}x{sd.AttemptCount})" : $"({sd.Marks})";
            row.Cells[2].Blocks.AddParagraph().Inlines.AddRun(marks);
        }

        private void AddMcqQuestionRow(RadFlowDocument doc, Table t, StructureDetail sd, string title)
        {
            var row = AddBlankRow(t);
            Ensure3Cells(row);
            SetQuestionRowCellWidths(row);

            row.Cells[0].Blocks.AddParagraph().Inlines.AddRun($"Q. {sd.QuestionNo}");
            var p = row.Cells[1].Blocks.AddParagraph();
            MakeRun(p, title, false, Alignment.Left);
        }

        private void AddMcqRow(RadFlowDocument doc, Table t, PaperDetail pd, string html, int questionCount = 1)
        {
            var row = AddBlankRow(t);
            Ensure3Cells(row);
            SetQuestionRowCellWidths(row);

            row.Cells[0].Blocks.AddParagraph().Inlines.AddRun($"{pd.SerialNo})");
            InsertHtmlIntoCell(row.Cells[1], html);
            row.Cells[2].Blocks.AddParagraph().Inlines.AddRun($"({pd.Marks})");
        }

        private void AddOrRow(RadFlowDocument doc, Table t)
            => UpdateMiddleCellOnly(doc, t, "OR");

        private void AddEndRow(RadFlowDocument doc, Table t)
            => UpdateMiddleCellOnly(doc, t, "* * * * * *");

        private void UpdateMiddleCellOnly(RadFlowDocument doc, Table t, string text)
        {
            var row = AddBlankRow(t);
            Ensure3Cells(row);
            SetQuestionRowCellWidths(row);
            var p = row.Cells[1].Blocks.AddParagraph();
            MakeRun(p, text, true, Alignment.Center);
        }

        // Remove only the first <p>...</p> pair (keep inner HTML) – same as your helper
        static string MergeFirstParagraphIntoHost(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return html;
            html = html.TrimStart();
            var m = Regex.Match(html, @"^\s*<p\b[^\>]*>(.*?)</p\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (!m.Success) return html;
            return html.Substring(0, m.Index) + m.Groups[1].Value + html.Substring(m.Index + m.Length);
        }

        private void AddAttemptRow(RadFlowDocument doc, Table outer, List<PaperDetail> details)
        {
            var row = AddBlankRow(outer);
            Ensure3Cells(row);
            SetQuestionRowCellWidths(row);

            var midCell = row.Cells[1];
            var inner = new Table(doc);
            midCell.Blocks.Add(inner);

            int idx = 0;
            foreach (var d in details)
            {
                var rr = inner.Rows.AddTableRow();
                rr.Cells.AddTableCell();
                rr.Cells.AddTableCell();
                SetAttemptRowCellWidths(rr);

                var letter = (char)('a' + idx);
                rr.Cells[0].Blocks.AddParagraph().Inlines.AddRun($"{letter})");

                var html = FirstNonEmpty(d.Description, d.HtmlContent);
                html = MergeFirstParagraphIntoHost(html);
                InsertHtmlIntoCell(rr.Cells[1], html);

                idx++;
            }

            SetTableBorder(inner, BorderStyle.None);
            if (inner.Rows.Count > 1) inner.Rows.RemoveAt(0);
        }

        private void MakeRun(Paragraph p, string text, bool bold, Alignment align)
        {
            p.TextAlignment = align;
            var editor = new RadFlowDocumentEditor(p.Document);
            editor.MoveToParagraphEnd(p);
            editor.CharacterFormatting.FontWeight.LocalValue = bold ? FontWeights.Bold : FontWeights.Normal;
            editor.InsertText(text);
        }

        private static void Ensure3Cells(TableRow r)
        {
            while (r.Cells.Count < 3) r.Cells.AddTableCell();
        }

        private static void SetQuestionRowCellWidths(TableRow r)
        {
            r.Cells[0].PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, Inches(0.69));
            r.Cells[1].PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, Inches(6.40));
            r.Cells[2].PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, Inches(0.60));
        } // TableWidthUnit value is in DIPs (1/96")  [7](https://docs.telerik.com/devtools/document-processing/libraries/radspreadprocessing/working-with-rows-and-columns/resizing)

        private static void SetAttemptRowCellWidths(TableRow r)
        {
            r.Cells[0].PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, Inches(0.40));
            r.Cells[1].PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, Inches(6.00));
        }

        private static void SetTableBorder(Table t, BorderStyle style)
        {
            var border = style == BorderStyle.None
                ? new Border(0, BorderStyle.None, new ThemableColor(System.Windows.Media.Colors.Transparent))
                : new Border(1, BorderStyle.Single, new ThemableColor(System.Windows.Media.Colors.Black));
            t.Borders = new TableBorders(border, border, border, border, border, border);
            foreach (var row in t.Rows)
                foreach (var cell in row.Cells)
                    cell.Borders = new TableCellBorders(border, border, border, border, border, border, border, border);
        } // Borders API  [9](https://docs.telerik.com/devtools/document-processing/common-information/device-independent-pixels)

        private void InsertHtmlIntoCell(TableCell cell, string htmlOrText)
        {
            var p = cell.Blocks.AddParagraph();
            var editor = new RadFlowDocumentEditor(cell.Document);
            editor.MoveToParagraphEnd(p);

            if (!string.IsNullOrWhiteSpace(htmlOrText) &&
                htmlOrText.TrimStart().StartsWith("<", StringComparison.Ordinal))
            {
                var htmlProvider = new HtmlFormatProvider();
                // Import HTML string as a document fragment and insert it at current position
                var fragment = htmlProvider.Import(htmlOrText, TimeSpan.FromSeconds(10));
                editor.InsertDocument(fragment);  // recommended way to inject HTML   [1](https://stackoverflow.com/questions/9467050/changing-the-margins-of-a-word-document)[2](https://docs.telerik.com/devtools/document-processing/libraries/radwordsprocessing/formats-and-conversion/html/html)
            }
            else
            {
                editor.InsertText(htmlOrText ?? string.Empty);
            }
        }

        private void AddInstructions(RadFlowDocument document, Paper paper)
        {
            var subject = _subjectService.GetById(paper.SubjectId ?? 0);
            var section = document.Sections.First();
            var table = section.Blocks.AddTable();

            var row = AddBlankRow(table); // placeholder
            row = AddBlankRow(table);
            row.Cells.AddTableCell();
            row.Cells.AddTableCell();
            row.Cells[0].Blocks.AddParagraph().Inlines.AddRun("NB :");

            int i = 1;
            foreach (var ins in subject?.SubjectInstructionDetails
                         .Where(p => p.PaperCategoryId == paper.PaperCategoryId)
                         .OrderBy(p => p.SerialNo)
                         .ToList() ?? new List<SubjectInstructionDetail>())
            {
                row = AddBlankRow(table);
                row.Cells.AddTableCell();
                row.Cells.AddTableCell();
                row.Cells[0].Blocks.AddParagraph().Inlines.AddRun($"{i++}.");
                InsertHtmlIntoCell(row.Cells[1], ins.Description);
            }

            if (table.Rows.Count > 1) table.Rows.RemoveAt(0);

            // top & bottom line only
            SetTableBorder(table, BorderStyle.None);
            if (table.Rows.Count > 0)
            {
                var top = table.Rows.First();
                foreach (var c in top.Cells)
                    c.Borders = new TableCellBorders(
                        new Border(BorderStyle.None),
                        new Border(1, BorderStyle.Single, new ThemableColor(System.Windows.Media.Colors.Black)),
                        new Border(BorderStyle.None),
                        new Border(BorderStyle.None),
                        new Border(BorderStyle.None),
                        new Border(BorderStyle.None),
                        new Border(BorderStyle.None),
                        new Border(BorderStyle.None));

                var bottom = table.Rows.Last();
                foreach (var c in bottom.Cells)
                    c.Borders = new TableCellBorders(
                        new Border(BorderStyle.None),
                        new Border(BorderStyle.None),
                        new Border(BorderStyle.None),
                        new Border(1, BorderStyle.Single, new ThemableColor(System.Windows.Media.Colors.Black)),
                        new Border(BorderStyle.None),
                        new Border(BorderStyle.None),
                        new Border(BorderStyle.None),
                        new Border(BorderStyle.None));
            }
        }

        private void CreateFooter(RadFlowDocument document)
        {
            var section = document.Sections.First();
            var footer = section.Footers.Add();
            var p = footer.Blocks.AddParagraph();
            p.TextAlignment = Alignment.Right;

            var editor = new RadFlowDocumentEditor(document);
            editor.MoveToParagraphEnd(p);
            editor.InsertField("PAGE", string.Empty);
            editor