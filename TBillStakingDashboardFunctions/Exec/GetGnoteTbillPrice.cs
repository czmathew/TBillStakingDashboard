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
    static class GetGnoteTbillPrice
    {
        public static void Execute()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://thetastats-nodejs-prd.azurewebsites.net/gnote_tbill");
                var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                string connString = Environment.GetEnvironmentVariable("sql_tbill");
                // connect to SQL
                using (SqlConnection connection = new SqlConnection(connString))
                {


                    connection.Open();

                    string query = @"INSERT INTO [dbo].[gnote_tbill]
                                   ([gnote_tbill]
                                   ,[tbill_gnote]
                                   ,gnote_reserve
                                   ,tbill_reserve)
                             VALUES
                                   (@gnote_tbill
                                   ,@tbill_gnote
                                   ,@gnote_reserve
                                   ,@tbill_reserve)";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@gnote_tbill", decimal.Parse(jsonClass.GNOTE_TBILL.ToString(), CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@tbill_gnote", decimal.Parse(jsonClass.TBILL_GNOTE.ToString(), CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@gnote_reserve", decimal.Parse(jsonClass.GNOTE_RESERVE.ToString(), CultureInfo.InvariantCulture));
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
