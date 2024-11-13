using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace MvcCrystalReport.Repository
{
    public class RepositoryService
    {
        private SqlConnection con;

        // To Handle connection related activities
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["MyDbContext"].ToString();
            con = new SqlConnection(constr);
        }


        // To view employee details with generic list
        public DataTable GetAllStoreSales()
        {
            connection();
            string query = "SELECT * FROM [Sales].[vStoreWithDemographics]";
            SqlCommand com = new SqlCommand(query, con);
            com.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();
            con.Open();
            da.Fill(dt);
            con.Close();
            return dt;
        }


    }
}