using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using NotificationApp.Components;

namespace TBillStakingDashboardWeb.Pages
{
    public class BullitModel : PageModel
    {

        public string CurrentBULT_TFuel { get; set; }
        public string CurrentTFuel_BULT { get; set; }
        public string BULT_reserve { get; set; }
        public string Tfuel_reserve { get; set; }

        private readonly IConfiguration _configuration;
        private readonly ILoggerManager _logger;
        public BullitModel(IConfiguration conf, ILoggerManager logger)
        {
            _configuration = conf;
            _logger = logger;
        }

        public void OnGet()
        {
            try
            {
                string connString = _configuration.GetConnectionString("sql_tbill");
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("[bult].[ups_getBultStats]", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                CurrentBULT_TFuel = reader.GetString("bult_tfuel");
                                CurrentTFuel_BULT = reader.GetString("tfuel_bult");
                                Tfuel_reserve = reader.GetString("tfuel_reserve");
                                BULT_reserve = reader.GetString("bult_reserve");
                                //lpWalletCount = reader.GetInt32("walletCount").ToString();

                            }
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}

