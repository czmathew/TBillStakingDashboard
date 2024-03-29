﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;

namespace TBillStakingDashboardFunctions.Exec
{
    static class GetThetaConNTFSales
    {
        public static void Execute()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://open-theta.de/api/nft/sold/recent/100/0x53ae8cdc2e60c81f4a1967dc381452a203dee836");
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

                string connString = Environment.GetEnvironmentVariable("sql_tbill");
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
                    bulkCopy.DestinationTableName = "nftSalesThetaCon_rawData";

                    bulkCopy.ColumnMappings.Add("tokenId", "tokenId");
                    bulkCopy.ColumnMappings.Add("category", "category");
                    bulkCopy.ColumnMappings.Add("createdTimestamp", "createdTimestamp");
                    bulkCopy.ColumnMappings.Add("soldTimestamp", "soldTimestamp");
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
                    bulkCopy.ColumnMappings.Add("nftContract", "nftContract");
                    connection.Open();

                    SqlCommand commandTruncate = new SqlCommand("truncate table nftSalesThetaCon_rawData", connection);
                    commandTruncate.ExecuteScalar();

                    // write the data in the "dataTable"
                    bulkCopy.WriteToServer(dt);

                    using (var command = new SqlCommand("usp_refreshThetaConNFTSales", connection)
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
