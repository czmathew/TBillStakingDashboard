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
    static class GetGnoteTfuelPrice
    {
        public static void Execute()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://api.gpool.io/status/tbill");
                var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                string connString = Environment.GetEnvironmentVariable("sql_tbill");
                // connect to SQL
                using (SqlConnection connection = new SqlConnection(connString))
                {


                    connection.Open();

                    string query = @"INSERT INTO [dbo].[gnote_tfuel]
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
                    string gnote_tfuel = "";
                    string tfuel_gnote = "";
                    if (((Newtonsoft.Json.Linq.JArray)jsonClass).Count == 0)
                    {
                       return;
                    }
                    foreach (var item in jsonClass)
                    {
                        if (item.pair.ToString().Equals("gnote_tfuel"))
                        {
                            gnote_tfuel = item.price;
                        } else if (item.pair.ToString().Equals("tfuel_gnote"))
                        {
                            tfuel_gnote = item.price;
                        }
                    }
                    cmd.Parameters.AddWithValue("@gnote_tfuel", decimal.Parse(gnote_tfuel, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@tfuel_gnote", decimal.Parse(tfuel_gnote, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@gnote_reserve", 0);
                    cmd.Parameters.AddWithValue("@tfuel_reserve", 0);

                    cmd.ExecuteNonQuery();

                    connection.Close();
                }
                // reset
                //dt.Clear();
            }
        }
    }
}
