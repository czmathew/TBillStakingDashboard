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
    static class GetBultPrice
    {
        public static void Execute()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://thetastats-nodejs-dev.azurewebsites.net/bult_tfuel");
                var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                string connString = Environment.GetEnvironmentVariable("sql_tbill");
                // connect to SQL
                using (SqlConnection connection = new SqlConnection(connString))
                {
                   

                    connection.Open();

                    string query = @"INSERT INTO [bult].[bult_tfuel]
                                   ([bult_tfuel]
                                   ,[tfuel_bult]
                                   ,bult_reserve
                                   ,tfuel_reserve)
                             VALUES
                                   (@bult_tfuel
                                   ,@tfuel_bult
                                   ,@bult_reserve
                                   ,@tfuel_reserve)";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@bult_tfuel", decimal.Parse(jsonClass.BULT_TFUEL.ToString(), CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@tfuel_bult", decimal.Parse(jsonClass.TFUEL_BULT.ToString(), CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@bult_reserve", decimal.Parse(jsonClass.BULT_RESERVE.ToString(), CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@tfuel_reserve", decimal.Parse(jsonClass.TFUEL_RESERVE.ToString(), CultureInfo.InvariantCulture));

                    cmd.ExecuteNonQuery();

                    connection.Close();
                }
                // reset
                //dt.Clear();
            }
        }
    }
}
