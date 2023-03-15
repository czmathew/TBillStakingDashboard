using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace TBillStakingDashboardFunctions.Exec
{
    static class GetTBillNTFSales
    {

        public static void Execute(ILogger log)
        {
            string[] tbillMultiplierAddresses = { "0x172d0bd953566538f050aabfeef5e2e8143e09f4" };

            using (WebClient wc = new WebClient())
            {
                string connString = Environment.GetEnvironmentVariable("sql_tbill");

                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    SqlCommand commandTruncate = new SqlCommand("truncate table nftSalesNew_rawData", connection);
                    commandTruncate.ExecuteScalar();

                    int i = 0;
                    while (i <= 10)
                    {
                        log.LogInformation($"GetTBillNTFSales calling: " + "https://api.opentheta.io/recentEvents?page=" + i.ToString() + "&type=10");
                        var json = wc.DownloadString("https://api.opentheta.io/recentEvents?page=" + i.ToString() + "&type=10");
                        var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);


                        DataTable tbl = new DataTable();
                        tbl.Columns.Add(new DataColumn("timestamp", typeof(Int32)));
                        tbl.Columns.Add(new DataColumn("name", typeof(string)));
                        //tbl.Columns.Add(new DataColumn("imageUrl", typeof(string)));
                        tbl.Columns.Add(new DataColumn("price", typeof(decimal)));
                        tbl.Columns.Add(new DataColumn("tokenId", typeof(Int32)));
                        tbl.Columns.Add(new DataColumn("creator", typeof(string)));
                        tbl.Columns.Add(new DataColumn("address", typeof(string)));

                        foreach (var item in jsonClass.events)
                        {
                            string address = item.address.ToString();
                            if (!tbillMultiplierAddresses.Any(x => address.Contains(x)))
                            {
                                continue;
                            }
                            DataRow dr = tbl.NewRow();
                            dr["timestamp"] = Int32.Parse(item.timestamp.ToString());
                            dr["name"] = item.name.ToString();
                            dr["creator"] = item.creatorName.ToString();
                            dr["address"] = address;
                            decimal.TryParse(item.price.ToString(), NumberStyles.Number, new CultureInfo("en-US"), out decimal price);
                            dr["price"] = price;
                            dr["tokenId"] = Int32.Parse(item.tokenID.ToString());
                            tbl.Rows.Add(dr);
                        }

                        // make sure to enable triggers
                        // more on triggers in next post
                        SqlBulkCopy bulkCopy = new SqlBulkCopy(
                            connection,
                            SqlBulkCopyOptions.TableLock |
                            SqlBulkCopyOptions.FireTriggers |
                            SqlBulkCopyOptions.UseInternalTransaction,
                            null
                            );


                        // set the destination table name
                        bulkCopy.DestinationTableName = "nftSalesNew_rawData";

                        bulkCopy.ColumnMappings.Add("timestamp", "timestamp");
                        bulkCopy.ColumnMappings.Add("name", "name");
                        bulkCopy.ColumnMappings.Add("creator", "creator");
                        bulkCopy.ColumnMappings.Add("address", "address");
                        bulkCopy.ColumnMappings.Add("price", "price");
                        bulkCopy.ColumnMappings.Add("tokenId", "tokenId");


                        // write the data in the "dataTable"
                        bulkCopy.WriteToServer(tbl);

                        // reset
                        tbl.Clear();
                        i++;
                    }

                    using (var command = new SqlCommand("usp_refreshNFTSales", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
        }
    }
}
