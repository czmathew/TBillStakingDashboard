﻿using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace TBillStakingDashboardFunctions.Exec
{
    static class GetTBillNTFForSale
    {
        public static void Execute()
        {

            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://open-theta.de/api/search/ALL/10006/ascending/0/0?creators=[%22Gworld%22]");
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

                //quick fix to remove last three zeros to make sure it can be converted to decimal
                foreach (DataRow row in dt.Rows)
                {
                    row["price"] = row["price"].ToString().Remove(row["price"].ToString().Length - 3);
                }

                string connString = Environment.GetEnvironmentVariable("sql-tbill");
                // connect to SQL
                using (SqlConnection connection = new SqlConnection(connString))
                {
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
                    bulkCopy.DestinationTableName = "nftForSale_rawData";

                    bulkCopy.ColumnMappings.Add("tokenId", "tokenId");
                    bulkCopy.ColumnMappings.Add("category", "category");
                    bulkCopy.ColumnMappings.Add("createdTimestamp", "createdTimestamp");
                    bulkCopy.ColumnMappings.Add("creator", "creator");
                    bulkCopy.ColumnMappings.Add("marketAddress", "marketAddress");
                    bulkCopy.ColumnMappings.Add("name", "name");
                    bulkCopy.ColumnMappings.Add("price", "price");
                    //bulkCopy.ColumnMappings.Add("tbillAmount", "tbillAmount");
                    //bulkCopy.ColumnMappings.Add("tbillmult", "tbillmult");
                    bulkCopy.ColumnMappings.Add("projectName", "projectName");
                    bulkCopy.ColumnMappings.Add("owner", "owner");
                    bulkCopy.ColumnMappings.Add("seller", "seller");
                    bulkCopy.ColumnMappings.Add("itemId", "itemId");
                    connection.Open();

                    SqlCommand commandTruncate = new SqlCommand("truncate table nftForSale_rawData", connection);
                    commandTruncate.ExecuteScalar();

                    // write the data in the "dataTable"
                    bulkCopy.WriteToServer(dt);

                    using (var command = new SqlCommand("usp_refreshNFTsForSale", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
                // reset
                dt.Clear();
            }

        }
    }
}
