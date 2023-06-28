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
    static class GetTbillRates
    {
        public static void Execute()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://api.gpool.io/tbill/rates");
                var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                decimal tbill_usd = 0;
                decimal tfuel_usd = 0;
                decimal usdc_usd = 0;
                decimal gnote_usd = 0;

                foreach (JObject item in jsonClass.rates)
                {
                    string pair = item.GetValue("pair").ToString();
                    if (pair.Equals("tbill_usd"))
                    {
                        tbill_usd = decimal.Parse(item.GetValue("rate").ToString());
                    }
                    else if (pair.Equals("tfuel_usd"))
                    {
                        tfuel_usd = decimal.Parse(item.GetValue("rate").ToString());
                    }
                    else if (pair.Equals("usdc_usd"))
                    {
                        usdc_usd = decimal.Parse(item.GetValue("rate").ToString());
                    }
                    else if (pair.Equals("gnote_usd"))
                    {
                        gnote_usd = decimal.Parse(item.GetValue("rate").ToString());
                    }
                }

                string connString = Environment.GetEnvironmentVariable("sql_tbill");
                // connect to SQL
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    string query = " INSERT INTO [dbo].[tbillRates] ([tbill_usd],[tfuel_usd],[usdc_usd],[gnote_usd],[targetRate],[rebaseRate],[instantRate],[rebaseTop],[rebaseBottom],[lpTokenRate],[gnoteLpTokenRate]) "
                                + " VALUES "
                                + " (@tbill_usd "
                                + " , @tfuel_usd "
                                + " , @usdc_usd "
                                + " , @gnote_usd "
                                + " , @targetRate "
                                + " , @rebaseRate "
                                + " , @instantRate "
                                + " , @rebaseTop "
                                + " , @rebaseBottom "
                                + " , @lpTokenRate "
                                + " , @gnoteLpTokenRate)";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@tbill_usd", tbill_usd);
                    cmd.Parameters.AddWithValue("@tfuel_usd", tfuel_usd);
                    cmd.Parameters.AddWithValue("@usdc_usd", usdc_usd);
                    cmd.Parameters.AddWithValue("@gnote_usd", gnote_usd);
                    cmd.Parameters.AddWithValue("@targetRate", decimal.Parse(jsonClass.targetRate.ToString()));
                    cmd.Parameters.AddWithValue("@rebaseRate", decimal.Parse(jsonClass.rebaseRate.ToString()));
                    cmd.Parameters.AddWithValue("@instantRate", decimal.Parse(jsonClass.instantRate.ToString()));
                    cmd.Parameters.AddWithValue("@rebaseTop", decimal.Parse(jsonClass.noRebaseRange.top.ToString()));
                    cmd.Parameters.AddWithValue("@rebaseBottom", decimal.Parse(jsonClass.noRebaseRange.bottom.ToString()));
                    //cmd.Parameters.AddWithValue("@lpTokenRate", decimal.Parse(jsonClass.lpTokenRate.ToString()));
                    cmd.Parameters.AddWithValue("@lpTokenRate", decimal.Parse("0"));
                    //cmd.Parameters.AddWithValue("@gnoteLpTokenRate", decimal.Parse(jsonClass.gnoteLpTokenRate.ToString()));
                    cmd.Parameters.AddWithValue("@gnoteLpTokenRate", decimal.Parse("0"));

                    cmd.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }
    }
}
