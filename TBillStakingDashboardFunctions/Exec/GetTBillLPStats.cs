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
    static class GetTBillLPStats
    {
        public static void Execute()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("http://thetascan.io/api/contract/tbill/");
                var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                DataTable dt = (DataTable)JsonConvert.DeserializeObject(jsonClass.stake.ToString(), (typeof(DataTable)));

                string connString = Environment.GetEnvironmentVariable("sql_tbill");
                // connect to SQL
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    SqlBulkCopy bulkCopy = new SqlBulkCopy(
                        connection,
                        SqlBulkCopyOptions.TableLock |
                        SqlBulkCopyOptions.FireTriggers |
                        SqlBulkCopyOptions.UseInternalTransaction,
                        null
                        );

                    // set the destination table name
                    bulkCopy.DestinationTableName = "lpWalletStats_raw";

                    bulkCopy.ColumnMappings.Add("address", "wallet");
                    bulkCopy.ColumnMappings.Add("share", "univ2");
                    bulkCopy.ColumnMappings.Add("tbill", "tbill");
                    bulkCopy.ColumnMappings.Add("wtfuel", "tfuel");
                    
                    connection.Open();
                    try { 
                        // write the data in the "dataTable"
                        bulkCopy.WriteToServer(dt);

                        SqlCommand commandTruncate = new SqlCommand("truncate table lpWalletStats_raw", connection);
                        commandTruncate.ExecuteScalar();

                        // write the data in the "dataTable"
                        bulkCopy.WriteToServer(dt);

                        using (var command = new SqlCommand("usp_refreshLPWalletStats", connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        })
                        {
                            command.ExecuteNonQuery();
                        }
                    } catch(Exception ex)
                    {

                    }



                    connection.Close();
                }
                // reset
                dt.Clear();
            }
        }
    }
}
