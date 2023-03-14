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
    static class GetPrices
    {
        public static void Execute()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://api.gpool.io/status/tbill");
                var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                var jsonExplorer = wc.DownloadString("https://explorer.thetatoken.org:8443/api/price/all");
                var jsonClassExplorer = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonExplorer);

                string connString = Environment.GetEnvironmentVariable("sql_tbill");
                // connect to SQL
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    string query = @"INSERT INTO [dbo].[prices]
                                   ([tbill]
                                      ,[gnote]
                                      ,[theta]
                                      ,[tfuel]
                                      ,[tdrop])
                             VALUES
                                   (@tbill
                                      ,@gnote
                                      ,@theta
                                      ,@tfuel
                                      ,@tdrop)";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    string tbill_usd = "0";
                    string gnote_usd = "0";
                    decimal theta = 0;
                    decimal tfuel = 0;
                    decimal tdrop = 0;
                    if (((Newtonsoft.Json.Linq.JArray)jsonClass).Count != 0)
                    {
                        foreach (var item in jsonClass)
                        {
                            if (item.pair.ToString().Equals("tbill_usd"))
                            {
                                tbill_usd = item.price;
                            }
                            else if (item.pair.ToString().Equals("gnote_usd"))
                            {
                                gnote_usd = item.price;
                            }
                        }
                    }

                    if (((Newtonsoft.Json.Linq.JArray)jsonClassExplorer.body).Count != 0)
                    {
                        foreach (var item in jsonClassExplorer.body)
                        {
                            if (item._id.ToString().Equals("TDROP"))
                            {
                                tdrop = item.price;
                            }
                            else if (item._id.ToString().Equals("TFUEL"))
                            {
                                tfuel = item.price;
                            }
                            else if (item._id.ToString().Equals("THETA"))
                            {
                                theta = item.price;
                            }
                        }
                    }



                    cmd.Parameters.AddWithValue("@tbill", decimal.Parse(tbill_usd, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@gnote", decimal.Parse(gnote_usd, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@theta", theta);
                    cmd.Parameters.AddWithValue("@tfuel", tfuel);
                    cmd.Parameters.AddWithValue("@tdrop", tdrop);

                    cmd.ExecuteNonQuery();

                    connection.Close();
                }
                // reset
                //dt.Clear();
            }
        }
    }
}
