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
    static class GetMissingNFTs
    {
        public static void Execute()
        {

            string connString = Environment.GetEnvironmentVariable("sql-tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("usp_getDailyTBillStats", connection)
                {
                    CommandType = CommandType.Text,
                    Connection = connection

                })
                {
                    var sql = "SELECT contract, tokenId FROM [dbo].[nftQueue] WHERE status = 10";
                    var parameterList = new List<string>();

                    command.CommandText = sql;
                    connection.Open();
                    var NFTsToProcess = new List<(string, int)> { };
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            string contract = reader.GetString("contract");
                            int tokenId = reader.GetInt32("tokenId");

                            NFTsToProcess.Add((contract,tokenId));
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }

                    foreach(var record in NFTsToProcess)
                    {
                        string contract = record.Item1;
                        int tokenId = record.Item2;
                        string uri = getNFTURI(contract, tokenId);
                        if (uri.Contains("ipfs://"))
                        {
                            uri = uri.Replace("ipfs://", "");
                            bool res = saveNFT("https://ipfs.io/ipfs/" + uri, tokenId);
                            updateQueue(contract, tokenId, res);
                        }
                    }
                }
            }

        }

        public static string getNFTURI(string contract, int tokenId)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    string contractAPIurl = Environment.GetEnvironmentVariable("contractAPIurl");
                    var json = wc.DownloadString(contractAPIurl + "?contract=" + contract + "&tokenId=" + tokenId.ToString() + "");
                    var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
                    return jsonClass.uri.ToString();

                }
                catch (Exception ex)
                {
                    return "";
                }

            }
        }

        public static void updateQueue(string contract, int tokenId, bool success)
        {
            string connString = Environment.GetEnvironmentVariable("sql-tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    string query = "update [dbo].[nftQueue] set status = @status "
                    + " where contract = @contract and tokenId = @tokenId";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@status", success ? 50 : 90);
                    cmd.Parameters.AddWithValue("@contract", contract);
                    cmd.Parameters.AddWithValue("@tokenId", tokenId);

                    connection.Open();
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    //return false;
                }

                connection.Close();
            }
        }

        public static bool saveNFT(string NFTurl, int tokenId)
        {
            using (WebClient wc = new WebClient())
            {
                string connString = Environment.GetEnvironmentVariable("sql-tbill");
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    try
                    {
                        var json = wc.DownloadString(NFTurl);
                        var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);


                        string query = "INSERT INTO[dbo].[nftMinted]([name],[date],[image],[multiplier],[tbillAmount],[boostPercentage],[edition])"
                        + "VALUES(@name, @date, @image, @multiplier, @tbillAmount, @boostPercentage, @edition)";

                        //string connString = Environment.GetEnvironmentVariable("sql-tbill");
                        // connect to SQL


                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        dateTime = dateTime.AddMilliseconds(double.Parse(jsonClass.date.ToString())).ToLocalTime();

                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@name", jsonClass.name.ToString());
                        cmd.Parameters.AddWithValue("@date", dateTime);
                        cmd.Parameters.AddWithValue("@image", jsonClass.image.ToString());
                        cmd.Parameters.AddWithValue("@multiplier", jsonClass.attributes[0].value.ToString());
                        cmd.Parameters.AddWithValue("@tbillAmount", int.Parse(jsonClass.attributes[1].value.ToString()));
                        cmd.Parameters.AddWithValue("@boostPercentage", int.Parse(jsonClass.attributes[2].value.ToString()));
                        cmd.Parameters.AddWithValue("@edition", tokenId);

                        connection.Open();
                        cmd.ExecuteNonQuery();

                    }
                    catch (System.Net.WebException ex)
                    {
                        return false;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                    connection.Close();
                }
            }
            return true;
        }
    }
}
