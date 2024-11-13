using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using CrystalDecisions.Shared;

namespace MvcCrystalReport.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public async Task<ActionResult> Index()
        {
            var dataTable = await GetDataAsync();
            return View(dataTable);
        }

        public async Task<ActionResult> GenerateReport()
        {
            // Simulating long-running process
            System.Threading.Thread.Sleep(10000);

            ReportDocument reportDocument = new ReportDocument();
            string reportPath = Server.MapPath("~/Reports/rptCustomer.rpt");
            reportDocument.Load(reportPath);

            // Optionally set data source if it's dynamic
            var dt = await GetDataAsync();
            reportDocument.SetDataSource(dt);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            // Export the report to a stream in PDF format
            Stream stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);
            reportDocument.Close();
            reportDocument.Dispose();
            
            // Return the PDF as a FileResult
            return File(stream, "application/pdf", "Report.pdf");
        }


        private async Task<DataTable> GetDataAsync()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
            var dataTable = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "SELECT * FROM [Sales].[vStoreWithDemographics]";
                using (var command = new SqlCommand(query, connection))
                using (var adapter = new SqlDataAdapter(command))
                {
                    await connection.OpenAsync();
                    await Task.Run(() => adapter.Fill(dataTable));
                }
            }
            return dataTable;
        }
    }
}