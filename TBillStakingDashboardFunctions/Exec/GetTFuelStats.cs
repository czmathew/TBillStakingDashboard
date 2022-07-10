using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;

namespace TBillStakingDashboardFunctions.Exec
{
    static class GetTFuelStats
    {
        public static void Execute()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://explorer.thetatoken.org:8443/api/supply/tfuel");
                var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                decimal total_supply = 0;
                decimal circulation_supply = 0;

                total_supply = decimal.Parse(jsonClass.total_supply.ToString());
                circulation_supply = decimal.Parse(jsonClass.circulation_supply.ToString());

                string connString = Environment.GetEnvironmentVariable("sql_tbill");
                // connect to SQL
                try { 
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    string query = " INSERT INTO [dbo].[tfuelStats] ([total_supply],[circulation_supply]) "
                                + " VALUES "
                                + " (@total_supply "
                                + " , @circulation_supply "
                                + " )";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@total_supply", total_supply);
                    cmd.Parameters.AddWithValue("@circulation_supply", circulation_supply);

                    cmd.ExecuteNonQuery();

                    connection.Close();
                }
                } catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
