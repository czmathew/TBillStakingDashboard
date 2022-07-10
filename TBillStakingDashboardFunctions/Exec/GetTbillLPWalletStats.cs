using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace TBillStakingDashboardFunctions.Exec
{
    static class GetTbillLPWalletStats
    {
        public static void Execute()
        {
            List<string> splitted = new List<string>();
            string fileList = GetCSV("https://thetascan.io/download_token_transaction/?address=0xa8cbccbd7480dc251abf8b1c1c99973e6b16b111&contract=0x22cb20636c2d853de2b140c2eaddbfd6c3643a39");
            string[] tempStr;
            List<string> todayWallets = new List<string>();

            int datePos = 0;
            int recievingAddressPos = 0;

            tempStr = fileList.Split('\n');
            int i = 0;
            foreach (string item in tempStr)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    string[] tempFields = item.Split(',');
                    if (i == 0)
                    {

                        //splitted.Add(item);
                        int y = 0;
                        foreach (string field in tempFields)
                        {
                            if (field.Replace("\"", "").Equals("Date"))
                            {
                                datePos = y;
                            }
                            else if (field.Replace("\"", "").Equals("Receiving Address"))
                            {
                                recievingAddressPos = y;
                            }
                            y++;
                        }
                    }
                    else
                    {
                        string tmpDate = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                        string tmpAddress = tempFields[recievingAddressPos];
                        if (tempFields[datePos].Equals(tmpDate) && !todayWallets.Contains(tmpAddress))
                        {
                            todayWallets.Add(tmpAddress);
                        }
                    }
                    i++;

                }
            }

           
                string connString = Environment.GetEnvironmentVariable("sql_tbill");
                // connect to SQL
                using (SqlConnection connection = new SqlConnection(connString))
                {
                   

                    connection.Open();

                    string query = "INSERT INTO [dbo].[tbillLPWalletStats] ([walletCount]) VALUES (@walletCount)";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@walletCount", todayWallets.Count);

                    cmd.ExecuteNonQuery();

                    connection.Close();
                }
                // reset
                //dt.Clear();
           
        }

        private static string GetCSV(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }
    }
}
