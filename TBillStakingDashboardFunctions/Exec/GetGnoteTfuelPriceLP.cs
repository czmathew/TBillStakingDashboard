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
    static class GetGnoteTfuelPriceLP
    {
        public static void Execute()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://thetastats-nodejs-prd.azurewebsites.net/gnote_tfuel");
                var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                string connString = Environment.GetEnvironmentVariable("sql_tbill");
                // connect to SQL
                using (SqlConnection connection = new SqlConnection(connString))
                {


                    connection.Open();

                    string query = @"INSERT INTO [dbo].[gnote_tfuel_lp]
                                   ([gnote_tfuel]
                                   ,[tfuel_gnote]
                                   ,gnote_reserve
                                   ,tfuel_reserve)
                             VALUES
                                   (@gnote_tfuel
                                   ,@tfuel_gnote
                                   ,@gnote_reserve
                                   ,@tfuel_reserve)";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@gnote_tfuel", decimal.Parse(jsonClass.GNOTE_TFUEL.ToString(), CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@tfuel_gnote", decimal.Parse(jsonClass.TFUEL_GNOTE.ToString(), CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@gnote_reserve", decimal.Parse(jsonClass.GNOTE_RESERVE.ToString(), CultureInfo.InvariantCulture));
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
