using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Text;

namespace TBillStakingDashboardFunctions.Exec
{
    static class GetTfuelTbillPrice
    {
        public static void Execute()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://thetastats-nodejs-prd.azurewebsites.net/tbill_tfuel");
                var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                string connString = Environment.GetEnvironmentVariable("sql_tbill");
                // connect to SQL
                using (SqlConnection connection = new SqlConnection(connString))
                {


                    connection.Open();

                    string query = @"INSERT INTO [dbo].[tfuel_tbill]
                                   ([tfuel_tbill]
                                   ,[tbill_tfuel]
                                   ,tfuel_reserve
                                   ,tbill_reserve)
                             VALUES
                                   (@tfuel_tbill
                                   ,@tbill_tfuel
                                   ,@tfuel_reserve
                                   ,@tbill_reserve)";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@tfuel_tbill", decimal.Parse(jsonClass.TFUEL_TBILL.ToString(), CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@tbill_tfuel", decimal.Parse(jsonClass.TBILL_TFUEL.ToString(), CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@tfuel_reserve", decimal.Parse(jsonClass.TFUEL_RESERVE.ToString(), CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@tbill_reserve", decimal.Parse(jsonClass.TBILL_RESERVE.ToString(), CultureInfo.InvariantCulture));

                    cmd.ExecuteNonQuery();

                    connection.Close();
                }
                // reset
                //dt.Clear();
            }
        }
    }
}
