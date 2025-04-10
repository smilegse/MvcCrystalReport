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
using Microsoft.AspNetCore.Http;

namespace MvcCrystalReport.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public async Task<ActionResult> Index()
        {
            var query = "SELECT * FROM [Sales].[vStoreWithDemographics]"; 
            //var dataTable = await GetDataAsync(query);
            var dataTable = GetData(query);
            return View(dataTable);
        }

        public async Task<ActionResult> LongRunningReport()
        {
            // Simulating long-running process
            System.Threading.Thread.Sleep(20000);

            ReportDocument reportDocument = new ReportDocument();
            string reportPath = Server.MapPath("~/Reports/rptCustomer.rpt");
            reportDocument.Load(reportPath);

            // Optionally set data source if it's dynamic
            var query = "SELECT * FROM [Sales].[vStoreWithDemographics]";
            //var dt = await GetDataAsync(query);
            var dt = GetData(query);
            reportDocument.SetDataSource(dt);
            // Export the report to a stream in PDF format
            Stream stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);
            
            // Return the PDF as a FileResult
            return File(stream, "application/pdf", "Report.pdf");
        }

        public async Task<ActionResult> FastReport()
        {
            ReportDocument reportDocument = new ReportDocument();
            string reportPath = Server.MapPath("~/Reports/rptSales.rpt");
            reportDocument.Load(reportPath);

            // Optionally set data source if it's dynamic
            var query = "SELECT TOP 100 * FROM [Sales].[vStoreWithDemographics]";
            //var dt = await GetDataAsync(query);
            var dt = GetData(query);
            reportDocument.SetDataSource(dt);

            // Export the report to a stream in PDF format
            Stream stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

            // Return the PDF as a FileResult
            return File(stream, "application/pdf", "Report.pdf");
        }

        private async Task<DataTable> GetDataAsync(string query)
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
            var dataTable = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                using (var adapter = new SqlDataAdapter(command))
                {
                    await connection.OpenAsync();
                    await Task.Run(() => adapter.Fill(dataTable));
                }
            }
            return dataTable;
        }

        private DataTable GetData(string query)
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
            var dataTable = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                using (var adapter = new SqlDataAdapter(command))
                {
                    connection.Open();
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
    }
}