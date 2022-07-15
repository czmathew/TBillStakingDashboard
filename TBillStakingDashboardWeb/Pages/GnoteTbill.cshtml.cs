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
    public class GnoteTbillModel : PageModel
    {

        public string CurrentGnote_Tbill { get; set; }
        public string CurrentTbill_Gnote { get; set; }
        public string Gnote_reserve { get; set; }
        public string Tbill_reserve { get; set; }
        public string Wallet { get; set; }

        private readonly IConfiguration _configuration;
        private readonly ILoggerManager _logger;
        public GnoteTbillModel(IConfiguration conf, ILoggerManager logger)
        {
            _configuration = conf;
            _logger = logger;
        }

        public void OnGet()
        {
            if (!String.IsNullOrEmpty(HttpContext.Request.Query["wallet"]))
            {
                Wallet = HttpContext.Request.Query["wallet"];
            }
            try
            {
                string connString = _configuration.GetConnectionString("sql_tbill");
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("[dbo].[usp_getGnoteTbillStats]", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                CurrentGnote_Tbill = reader.GetString("gnote_tbill");
                                CurrentTbill_Gnote = reader.GetString("tbill_gnote");
                                Tbill_reserve = reader.GetString("tbill_reserve");
                                Gnote_reserve = reader.GetString("gnote_reserve");
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

