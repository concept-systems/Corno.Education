namespace ReportLibrary.Exam
{
    using Globals;
    //using OnlineExam.DAL.Classes;
    //using OnlineExam.Helpers;

    /// <summary>
    /// Summary description for AdmitCardRpt.
    /// </summary>
    public partial class AdmitCardRpt : Telerik.Reporting.Report
    {
        public AdmitCardRpt()
        {
            // Required for telerik Reporting designer support
            InitializeComponent();
        }
        public AdmitCardRpt(int instanceID)
        {
            InitializeComponent();
            sdsCollege.ConnectionString = GlobalVariables.ConnectionString_ExamServer;
            sdsCourse.ConnectionString = GlobalVariables.ConnectionString_ExamServer;

            sdsAdmitCard.ConnectionString = sdsAdmitCard.ConnectionString;
            sdsAdmitCard.SelectCommand = sdsAdmitCard.SelectCommand.Replace("@InstanceID", instanceID.ToString());


            //IEstimateService miService = (IEstimateService)Bootstrapper.GetService(typeof(EstimateService));

            //this.DataSource = miService.EstimateRepository.Get(est => est.ID == estimateID)
            //    .Select(est => new
            //    {
            //        CustomerName = est.Customer.Name,
            //        est.ChallanNo,

            //    });
        }

    }
}