using Corno.Data.Corno.Question_Bank;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Data.ViewModels.Appointment;
using Corno.Services.Corno.Interfaces;
using DevExpress.XtraRichEdit;
using System.Web;
using Telerik.Windows.Documents.Flow.Model;

namespace Corno.Services.Corno.Question_Bank.Interfaces;

public interface IPaperService : IMainService<Paper>
{
    #region -- Methods --

    Paper GetById(int id);
    Paper GetQuestions(Paper questionBankModel);

    void ValidateFields(Paper paper, bool validateDetails = true);

    int GetTaxonomy(string question);
    void UpdateTaxonomy(Paper paper);
    void Save(Paper paper);


    RadFlowDocument Draw(Paper paper);
    RichEditDocumentServer DrawRichEdit(Paper paper);
    string GetQuestionTypeName(int? questionTypeId);

    void Create(ReportModel reportModel, int setsToBeDrawn);

    void UploadModelAnswer(HttpPostedFileBase pdfModelAnswer, string basePath, Paper paer);

    #endregion
}