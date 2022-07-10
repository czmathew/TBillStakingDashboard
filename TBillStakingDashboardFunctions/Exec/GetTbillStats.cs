using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;

namespace TBillStakingDashboardFunctions.Exec
{
    static class GetTbillStats
    {
        public static void Execute()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://api.gpool.io/tbill/overview");
                var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                //get tbill supply
                var jsonSupply = wc.DownloadString("http://www.thetascan.io/api/contract/?contract=0x22cb20636c2d853de2b140c2eaddbfd6c3643a39");
                var jsonSupplyClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonSupply);

                string connString = Environment.GetEnvironmentVariable("sql_tbill");
                // connect to SQL
                using (SqlConnection connection = new SqlConnection(connString))
                {
                   

                    connection.Open();

                    string query = "INSERT INTO [dbo].[tbillStats]([tvLocked],[tbillLocked],[tfuelLocked],[rewards],[endTime],[tbillSupply])";
                    query += " VALUES (@tvLocked, @tbillLocked, @tfuelLocked, @rewards, @endTime, @tbillSupply)";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@tvLocked", decimal.Parse(jsonClass.data.tv.ToString()));
                    cmd.Parameters.AddWithValue("@tbillLocked", decimal.Parse(jsonClass.data.tbill.ToString()));
                    cmd.Parameters.AddWithValue("@tfuelLocked", decimal.Parse(jsonClass.data.tfuel.ToString()));
                    cmd.Parameters.AddWithValue("@rewards", decimal.Parse(jsonClass.data.reward.ToString()));
                    cmd.Parameters.AddWithValue("@endTime", int.Parse(jsonClass.data.end_time.ToString()));
                    cmd.Parameters.AddWithValue("@tbillSupply", decimal.Parse(jsonSupplyClass.supply.ToString()));

                    cmd.ExecuteNonQuery();

                    connection.Close();
                }
                // reset
                //dt.Clear();
            }
        }
    }
}
