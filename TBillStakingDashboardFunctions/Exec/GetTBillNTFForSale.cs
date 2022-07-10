using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;

namespace TBillStakingDashboardFunctions.Exec
{
    static class GetTBillNTFForSale
    {
        public static void Execute()
        {

            using (WebClient wc = new WebClient())
            {
                //get metadataIDs for all tbill multipliers
                int i = 0;
                //name, imageurl, metadataID
                List<Tuple<string, string, string>> nftMeta = new List<Tuple<string, string, string>>();
                while (true)
                {
                    try
                    {
                        var json = wc.DownloadString("https://api.opentheta.io/search?collection=tbill-multiplier&page=" + i.ToString());
                        var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                        if (((Newtonsoft.Json.Linq.JArray)jsonClass.results).Count == 0)
                        {
                            break;
                        }
                        foreach (var item in jsonClass.results)
                        {
                            nftMeta.Add(new Tuple<string, string, string>(item.name.ToString(), item.imageUrl.ToString(), item.metadataID.ToString()));
                        }

                        i++;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }

                string connString = Environment.GetEnvironmentVariable("sql_tbill");

                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();
                    SqlCommand commandTruncate = new SqlCommand("truncate table nftForSaleNew_rawData", connection);
                    commandTruncate.ExecuteScalar();

                    DataTable tbl = new DataTable();
                    tbl.Columns.Add(new DataColumn("timestamp", typeof(Int32)));
                    tbl.Columns.Add(new DataColumn("name", typeof(string)));
                    tbl.Columns.Add(new DataColumn("imageUrl", typeof(string)));
                    tbl.Columns.Add(new DataColumn("listedPrice", typeof(decimal)));
                    tbl.Columns.Add(new DataColumn("tokenId", typeof(Int32)));

                    foreach (var nftType in nftMeta)
                    {

                        var json = wc.DownloadString("https://api.opentheta.io/edition?contract=0x172d0bd953566538f050aabfeef5e2e8143e09f4&ID=" + nftType.Item3 + " &filter=sale");
                        var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);



                        foreach (var item in jsonClass.editions)
                        {
                            DataRow dr = tbl.NewRow();
                            dr["timestamp"] = Int32.Parse(item.timestamp.ToString());
                            dr["name"] = nftType.Item1;
                            dr["imageUrl"] = nftType.Item2;
                            decimal.TryParse(item.listedPrice.ToString(), NumberStyles.Number, new CultureInfo("en-US"), out decimal price);
                            dr["listedPrice"] = price;
                            dr["tokenId"] = Int32.Parse(item.tokenId.ToString());
                            tbl.Rows.Add(dr);
                        }

                    }

                    SqlBulkCopy bulkCopy = new SqlBulkCopy(
                            connection,
                            SqlBulkCopyOptions.TableLock |
                            SqlBulkCopyOptions.FireTriggers |
                            SqlBulkCopyOptions.UseInternalTransaction,
                            null
                            );

                    // set the destination table name
                    bulkCopy.DestinationTableName = "nftForSaleNew_rawData";

                    bulkCopy.ColumnMappings.Add("timestamp", "timestamp");
                    bulkCopy.ColumnMappings.Add("name", "name");
                    bulkCopy.ColumnMappings.Add("imageUrl", "imageUrl");
                    bulkCopy.ColumnMappings.Add("listedPrice", "listedPrice");
                    bulkCopy.ColumnMappings.Add("tokenId", "tokenId");

                    // write the data in the "dataTable"
                    bulkCopy.WriteToServer(tbl);

                    using (var command = new SqlCommand("usp_refreshNFTsForSaleNew", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        command.ExecuteNonQuery();
                    }

                }
                //var json = wc.DownloadString("https://open-theta.de/api/search/ALL/10006/ascending/0/0?creators=[%22Gworld%22]");
                //DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

                //quick fix to remove last three zeros to make sure it can be converted to decimal
                //foreach (DataRow row in dt.Rows)
                //{
                //    row["price"] = row["price"].ToString().Remove(row["price"].ToString().Length - 3);
                //}

                //string connString = Environment.GetEnvironmentVariable("sql_tbill");
                //// connect to SQL
                //using (SqlConnection connection = new SqlConnection(connString))
                //{
                //    // make sure to enable triggers
                //    // more on triggers in next post
                //    SqlBulkCopy bulkCopy = new SqlBulkCopy(
                //        connection,
                //        SqlBulkCopyOptions.TableLock |
                //        SqlBulkCopyOptions.FireTriggers |
                //        SqlBulkCopyOptions.UseInternalTransaction,
                //        null
                //        );

                //    // set the destination table name
                //    bulkCopy.DestinationTableName = "nftForSale_rawData";

                //    bulkCopy.ColumnMappings.Add("tokenId", "tokenId");
                //    bulkCopy.ColumnMappings.Add("category", "category");
                //    bulkCopy.ColumnMappings.Add("createdTimestamp", "createdTimestamp");
                //    bulkCopy.ColumnMappings.Add("creator", "creator");
                //    bulkCopy.ColumnMappings.Add("marketAddress", "marketAddress");
                //    bulkCopy.ColumnMappings.Add("name", "name");
                //    bulkCopy.ColumnMappings.Add("price", "price");
                //    //bulkCopy.ColumnMappings.Add("tbillAmount", "tbillAmount");
                //    //bulkCopy.ColumnMappings.Add("tbillmult", "tbillmult");
                //    bulkCopy.ColumnMappings.Add("projectName", "projectName");
                //    bulkCopy.ColumnMappings.Add("owner", "owner");
                //    bulkCopy.ColumnMappings.Add("seller", "seller");
                //    bulkCopy.ColumnMappings.Add("itemId", "itemId");
                //    connection.Open();

                //    //    SqlCommand commandTruncate = new SqlCommand("truncate table nftForSale_rawData", connection);
                //    //    commandTruncate.ExecuteScalar();

                //    //    // write the data in the "dataTable"
                //    //    bulkCopy.WriteToServer(dt);

                //    //    using (var command = new SqlCommand("usp_refreshNFTsForSale", connection)
                //    //    {
                //    //        CommandType = CommandType.StoredProcedure
                //    //    })
                //    //    {
                //    //        command.ExecuteNonQuery();
                //    //    }

                //    //    connection.Close();
                //    //}
                //    //// reset
                //    //dt.Clear();
                //}
            }

        }
    }
}
