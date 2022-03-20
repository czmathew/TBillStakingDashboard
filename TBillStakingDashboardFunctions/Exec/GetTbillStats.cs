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
                
                string connString = Environment.GetEnvironmentVariable("sql-tbill");
                // connect to SQL
                using (SqlConnection connection = new SqlConnection(connString))
                {
                   

                    connection.Open();

                    string query = "INSERT INTO [dbo].[tbillStats]([tvLocked],[tbillLocked],[tfuelLocked],[rewards],[endTime])";
                    query += " VALUES (@tvLocked, @tbillLocked, @tfuelLocked, @rewards, @endTime)";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@tvLocked", decimal.Parse(jsonClass.data.tv.ToString()));
                    cmd.Parameters.AddWithValue("@tbillLocked", decimal.Parse(jsonClass.data.tbill.ToString()));
                    cmd.Parameters.AddWithValue("@tfuelLocked", decimal.Parse(jsonClass.data.tfuel.ToString()));
                    cmd.Parameters.AddWithValue("@rewards", decimal.Parse(jsonClass.data.reward.ToString()));
                    cmd.Parameters.AddWithValue("@endTime", int.Parse(jsonClass.data.end_time.ToString()));

                    cmd.ExecuteNonQuery();

                    connection.Close();
                }
                // reset
                //dt.Clear();
            }
        }
    }
}
