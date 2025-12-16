using Corno.Data.Reports;
using Corno.Globals;
using Corno.OnlineExam.Areas.Services.Interfaces;
using Corno.Reports.CrystalReports;
using Corno.Reports.DataSets;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Corno.OnlineExam.Areas.Services;

public class HallTicketService : IHallTicketService
{

    #region -- Private Methods --
    private static async Task<Data_HallTicket> GetHallTicketDataSetAsync(HallTicketViewModel dto, int instanceId)
    {
        var dataSet = new Data_HallTicket();

        using var sqlConnection = new SqlConnection(GlobalVariables.ConnectionStringExamServer);
        await sqlConnection.OpenAsync();

        using var cmd = new SqlCommand("sp_HallTicket_Show", sqlConnection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 6000
        };

        cmd.Parameters.AddWithValue("@EXAMINST", instanceId);
        if (dto.CoursePartId != null)
            cmd.Parameters.AddWithValue("@Course_Part", (int)dto.CoursePartId);
        if (dto.CollegeId != null)
            cmd.Parameters.AddWithValue("@College", (int)dto.CollegeId);
        if (dto.BranchId > 0)
            cmd.Parameters.AddWithValue("@Branch", (int)dto.BranchId);

        cmd.Parameters.AddWithValue("@SeatNoFrom", Convert.ToInt64(dto.FromSeatNo));
        cmd.Parameters.AddWithValue("@SeatNoTo", Convert.ToInt64(dto.ToSeatNo));
        cmd.Parameters.AddWithValue("@PRN", dto.PrnNo ?? string.Empty);

        using var adapter = new SqlDataAdapter(cmd);
        adapter.Fill(dataSet, "Hall_Ticket");

        return dataSet;
    }


    /*private static Data_HallTicket GetHallTicketDataSet(HallTicketViewModel dto, int instanceId)
    {
        using var sqlConnection = new SqlConnection(GlobalVariables.ConnectionStringExamServer);
        sqlConnection.Open();

        using var cmd = new SqlCommand();
        cmd.Connection = sqlConnection;

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "sp_HallTicket_Show";
        cmd.CommandTimeout = 6000;

        cmd.Parameters.AddWithValue("@EXAMINST", instanceId);
        if (dto.CoursePartId != null)
            cmd.Parameters.AddWithValue("@Course_Part", (int)dto.CoursePartId);
        if (dto.CollegeId != null)
            cmd.Parameters.AddWithValue("@College", (int)dto.CollegeId);
        if (dto.BranchId is > 0)
            cmd.Parameters.AddWithValue("@Branch", (int)dto.BranchId);
        var seatNoFrom = Convert.ToInt64(dto.FromSeatNo);
        var seatNoTo = Convert.ToInt64(dto.ToSeatNo);
        cmd.Parameters.AddWithValue("@SeatNoFrom", seatNoFrom);
        cmd.Parameters.AddWithValue("@SeatNoTo", seatNoTo);

        var prnNo = dto.PrnNo ?? string.Empty;
        cmd.Parameters.AddWithValue("@PRN", prnNo);

        using var sqlDataAdapter = new SqlDataAdapter(cmd);
        // Fill the DataSet using default values for DataTable names, etc
        var dataSet = new Data_HallTicket();
        sqlDataAdapter.Fill(dataSet, "Hall_Ticket");

        sqlConnection.Close();
        return dataSet;
    }*/

    /*private static Data_HallTicket GetHallTicketDataSet(HallTicketViewModel dto, int instanceId)
    {
        var prnNo = dto.PrnNo ?? string.Empty;

        var parameters = new[]
        {
            new SqlParameter("@EXAMINST", instanceId),
            new SqlParameter("@Course_Part", (object?)dto.CoursePartId ?? DBNull.Value),
            new SqlParameter("@College", (object?)dto.CollegeId ?? DBNull.Value),
            new SqlParameter("@Branch", (object?)dto.BranchId ?? DBNull.Value),
            new SqlParameter("@SeatNoFrom", Convert.ToInt64(dto.FromSeatNo)),
            new SqlParameter("@SeatNoTo", Convert.ToInt64(dto.ToSeatNo)),
            new SqlParameter("@PRN", prnNo)
        };

        var dataSet = new Data_HallTicket();

        var unitOfWorkCore = Bootstrapper.Get<IUnitOfWorkCore>();

        using var connection = unitOfWorkCore.DbContext.Database.Connection;
        using var command = connection.CreateCommand();
        command.CommandText = "sp_HallTicket_Show";
        command.CommandType = CommandType.StoredProcedure;
        command.CommandTimeout = 6000;

        foreach (var param in parameters)
            command.Parameters.Add(param);

        /*if (connection.State != ConnectionState.Open)
            connection.Open();#1#

        using var adapter = new SqlDataAdapter((SqlCommand)command);
        adapter.Fill(dataSet, "Hall_Ticket");
        
        /*if (connection.State == ConnectionState.Open)
            connection.Close();#1#

        return dataSet;
    }*/


    public byte[] ImageToByteArray(Bitmap imageIn)
    {
        var ms = new MemoryStream();
        imageIn.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }
    #endregion

    #region -- Public Methods --
    public async Task<ReportDocument> GetCrystalReport(HallTicketViewModel dto, int instanceId)
    {
        var dataHallTicket = await GetHallTicketDataSetAsync(dto, instanceId);

        //DataColumn column = dataHallTicket.Tables["Hall_Ticket"].Columns["Ima_ST_PHOTO"];
        var blankBitmap = new Bitmap(256, 256);
        using (var g = Graphics.FromImage(blankBitmap)) { g.Clear(Color.White); }

        foreach (DataTable table in dataHallTicket.Tables)
        {
            foreach (DataRow row in table.Rows)
            {
                if (row["Ima_ST_PHOTO"] == DBNull.Value || row["Ima_Flg"] == DBNull.Value)
                {
                    row["Ima_ST_PHOTO"] = ImageToByteArray(blankBitmap);
                    continue;
                }

                var data = (byte[])row["Ima_ST_PHOTO"];
                if (null == data || data.Length <= 0)
                    row["Ima_ST_PHOTO"] = ImageToByteArray(blankBitmap);
            }
        }

        // If Center is selected, delete all records other than prn no.
        if (dto.CentreId is > 0)
        {
            // Get Center name from Center Id
            dataHallTicket.Tables["Hall_Ticket"].Rows.OfType<DataRow>()
                .Where(r => r.Field<short>("Num_FK_DistCenter_ID") != dto.CentreId)
                .ToList()
                .ForEach(r => r.Delete());
        }

        // If PRN is selected, delete all records other than prn no.
        if (!string.IsNullOrEmpty(dto.PrnNo))
        {
            dataHallTicket.Tables["Hall_Ticket"].Rows.OfType<DataRow>()
                .Where(r => r.Field<string>("Chr_FK_PRN_NO") != dto.PrnNo)
                .ToList()
                .ForEach(r => r.Delete());
        }

        if (string.IsNullOrEmpty(dto.PrnNo))
            dto.PrnNo = string.Empty;

        var report = new HallTicket();
        report.SetDataSource(dataHallTicket.Tables["Hall_Ticket"]);

        /*ViewData["ReportName"] = "Hall Ticket";
        if (HttpContext.Session["CrystalReport"] is ReportDocument existingReport)
        {
            existingReport.Close();
            existingReport.Dispose();
        }*/

        return report;
    }

    #endregion
}