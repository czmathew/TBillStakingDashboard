using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using NotificationApp.Components;
using TBillStaking.Models;
using TBillStakingDashboardWeb.Models;

namespace TBillStaking.Pages
{
    public class IndexModel : PageModel
    {
        [ViewData]
        public string Wallet { get; set; }
        [ViewData]
        public string DisplayDevMessage { get; set; }
        [ViewData]
        public string DisplayNewDomainMessage { get; set; }
        public List<Tuple<string, string>> LpShare { get; set; }
        public List<Tuple<string, string>> LpRange { get; set; }
        public List<Tuple<string, string>> WalletList { get; set; }
        public List<Tuple<string, string, string>> RebaseList { get; set; }
        public List<NFTDetails> NFTs { get; set; }
        public List<NFTSaleDetails> NFTSales { get; set; }

        public string TvLocked { get; set; }
        public string TbillLocked { get; set; }
        public string TfuelLocked { get; set; }
        public string TfuelLocked24h { get; set; }
        public string Rewards { get; set; }

        public string lpWalletCount { get; set; }

        private readonly IConfiguration _configuration;
        private readonly ILoggerManager _logger;

        public IndexModel(IConfiguration conf, ILoggerManager logger)
        {
            _configuration = conf;
            _logger = logger;
        }
        public void OnGet()
        {
            try
            {
                DisplayDevMessage = "false";
                if (_configuration["DisplayDevMessage"] != null)
                {
                    DisplayDevMessage = _configuration["DisplayDevMessage"];
                }
                DisplayNewDomainMessage = "false";
                if (_configuration["DisplayNewDomainMessage"] != null)
                {
                    DisplayNewDomainMessage = _configuration["DisplayNewDomainMessage"];
                }
                NFTs = new List<NFTDetails>();
                NFTSales = new List<NFTSaleDetails>();
                //there probably is a better way to handle the wallet in request then having the same code on each page, but it should work for now
                if (!String.IsNullOrEmpty(HttpContext.Request.Query["wallet"]))
                {
                    Wallet = HttpContext.Request.Query["wallet"];
                }
                string connString = _configuration.GetConnectionString("sql_tbill");
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();
                    //using (var command = new SqlCommand("usp_getNFTDetails", connection)
                    //{
                    //    CommandType = CommandType.StoredProcedure
                    //})
                    //{
                    //    SqlDataReader reader = command.ExecuteReader();
                    //    try
                    //    {
                    //        while (reader.Read())
                    //        {
                    //            NFTDetails nft = new NFTDetails();
                    //            nft.Name = reader.GetString("name");
                    //            nft.Sold = reader.GetInt32("sold");
                    //            nft.CurrentSalePrice = reader.GetDecimal("CurrentSalePrice");
                    //            nft.CurrentSalePriceUsd = reader.GetDecimal("CurrentSalePriceUsd");
                    //            nft.AvailableForSale = reader.GetInt32("AvailableForSale");
                    //            nft.AvgPriceForNext5ForSale = reader.GetDecimal("AvgPriceForNext5ForSale");
                    //            NFTs.Add(nft);
                    //        }
                    //    }
                    //    finally
                    //    {
                    //        // Always call Close when done reading.
                    //        reader.Close();
                    //    }
                    //}

                    using (var command = new SqlCommand("usp_getLatestTbillStats", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                var tfuel24h = reader.GetDecimal("tfuelLocked24h");
                                //tvLocked = reader.GetDecimal("tvLocked").ToString();
                                TvLocked = String.Format("{0:n}", reader.GetDecimal("tvLocked"));
                                TbillLocked = String.Format("{0:n}", reader.GetDecimal("tbillLocked"));
                                TfuelLocked = String.Format("{0:n}", reader.GetDecimal("tfuelLocked"));
                                TfuelLocked24h = (tfuel24h > 0 ? "+" : "") + String.Format("{0:n}", tfuel24h);
                                Rewards = String.Format("{0:n}", reader.GetDecimal("rewards"));

                            }
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }


                    //using (var command = new SqlCommand("usp_getNFTSalesDetails", connection)
                    //{
                    //    CommandType = CommandType.StoredProcedure
                    //})
                    //{
                    //    command.Parameters.Add("@top", SqlDbType.Int).Value = 50;
                    //    SqlDataReader reader = command.ExecuteReader();
                    //    try
                    //    {
                    //        while (reader.Read())
                    //        {
                    //            NFTSaleDetails nft = new NFTSaleDetails();
                    //            nft.Name = reader.GetString("name");
                    //            nft.Timestamp = reader.GetDateTime("soldTimestamp");
                    //            nft.Price = reader.GetDecimal("price");
                    //            nft.PriceUsd = reader.GetDecimal("priceUsd");
                    //            nft.Buyer = reader.GetString("buyer");
                    //            NFTSales.Add(nft);
                    //        }
                    //    }
                    //    finally
                    //    {
                    //        // Always call Close when done reading.
                    //        reader.Close();
                    //    }
                    //}

                    using (var command = new SqlCommand("[dbo].[ups_getLPWalletCount]", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        command.Parameters.Add("@top", SqlDbType.Int).Value = 100;
                        WalletList = new List<Tuple<string, string>>();
                        SqlDataReader reader = command.ExecuteReader();
                        int rowCnt = 1;
                        int walletCalc = 0;
                        try
                        {
                            while (reader.Read())
                            {
                                if (rowCnt == 1)
                                {
                                    walletCalc = reader.GetInt32("walletCount");
                                    //lpWalletCount = reader.GetInt32("walletCount").ToString();
                                }
                                else if (rowCnt == 2)
                                {
                                    lpWalletCount = walletCalc.ToString() + " (24h change: " + (walletCalc - reader.GetInt32("walletCount")).ToString() + ")";
                                }

                                WalletList.Add(new Tuple<string, string>(reader.GetDateTime("timestamp").Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                                                                            , reader.GetInt32("walletCount").ToString()));
                                rowCnt++;
                            }
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }

                    using (var command = new SqlCommand("[dbo].[usp_getRebaseStats]", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        RebaseList = new List<Tuple<string, string, string>>();
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                string tmp = String.Format(CultureInfo.InvariantCulture, "{0:0.##}", reader.GetDecimal("rebasePercentage").ToString());
                                RebaseList.Add(new Tuple<string, string, string>(
                                        reader.GetString("date")
                                        , reader.GetDecimal("supplyToday").ToString()
                                        , reader.GetDecimal("rebasePercentage").ToString().Replace(',', '.')));
                            }
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }

                    using (var command = new SqlCommand("[dbo].[usp_getLpPct]", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        LpShare = new List<Tuple<string, string>>();
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                LpShare.Add(new Tuple<string, string>(
                                        reader.GetString("txt")
                                        , reader.GetDecimal("holdPct").ToString("0.00")));
                            }
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }

                    using (var command = new SqlCommand("[dbo].[usp_getLpRange]", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        LpRange = new List<Tuple<string, string>>();
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                LpRange.Add(new Tuple<string, string>(
                                        reader.GetString("txt")
                                        , reader.GetInt32("cnt").ToString()));
                            }
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }


                }
            } catch (Exception ex)
            {
                _logger.LogException("Index exception", ex);
            }
        }
    }
}
