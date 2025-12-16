using System;
using Corno.Reports.Base;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Appointment
{
    public partial class SubjectStickerRpt : BaseReport
    {
        private readonly string _sqlCommand;

        public SubjectStickerRpt()
        {
            InitializeComponent();

            
        }

        private void SubjectStickersRpt_NeedDataSource(object sender, EventArgs e)
        {
            DataSource = null;
            var rpt = (Telerik.Reporting.Processing.Report)sender;

            if (null == rpt.Parameters["Faculty"].Value || string.IsNullOrEmpty(rpt.Parameters["Faculty"].Value.ToString()))
                sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("AND (FacultyId IN (@FacultyId))", " ");
            else
                sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@FacultyId", rpt.Parameters["Faculty"].Value.ToString());

            if (null == rpt.Parameters["College"].Value || string.IsNullOrEmpty(rpt.Parameters["College"].Value.ToString()))
                sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("AND (Paper.CollegeId IN (@CollegeId))", " ");
            else
                sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@CollegeId", rpt.Parameters["College"].Value.ToString());

            if (null == rpt.Parameters["Course"].Value || string.IsNullOrEmpty(rpt.Parameters["Course"].Value.ToString()))
                sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("AND (Paper.CourseId IN (@CourseId))", " ");
            else
                sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@CourseId", rpt.Parameters["Course"].Value.ToString());

            if (null == rpt.Parameters["CoursePart"].Value || string.IsNullOrEmpty(rpt.Parameters["CoursePart"].Value.ToString()))
                sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("AND (Paper.CoursePartId IN (@CoursePartId))", " ");
            else
                sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@CoursePartId", rpt.Parameters["CoursePart"].Value.ToString());

            if (null == rpt.Parameters["Subject"].Value || string.IsNullOrEmpty(rpt.Parameters["Subject"].Value.ToString()))
                sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("AND (Paper.SubjectId IN (@SubjectId))", " ");
            else
                sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@SubjectId", rpt.Parameters["Subject"].Value.ToString());


            /*using (var connection = new SqlConnection(ApplicationGlobals.ConnectionString))
            {
                // create a command object
                using (var command = new SqlCommand(sdsMain.SelectCommand, connection))
                {
                    SqlDataReader dataReader = null;
                    try
                    {
                        // open the connection
                        connection.Open();
                        dataReader = command.ExecuteReader();
                        var dataTable = new DataTable();
                        dataTable.Load(dataReader);

                        var dtAll = dataTable.Clone();
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            if (Convert.IsDBNull(dataRow[FieldConstants.SetsToBeDrawn]))
                                continue;
                            for (var index = 0; index < Convert.ToInt32(dataRow[FieldConstants.SetsToBeDrawn]); index++)
                                dtAll.ImportRow(dataRow);
                        }
                        table1.DataSource = dtAll;

                        dataReader.Close();
                    }
                    catch (Exception exception)
                    {
                        dataReader?.Close();
                        Logger.LogHandler.LogError(exception);
                    }
                    dataReader?.Close();
                }
            }*/

            // Reset the sql command
            sdsMain.SelectCommand = _sqlCommand;
        }
    }
}