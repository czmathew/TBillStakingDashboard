using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using TBillStaking.Models;
using TBillStakingDashboardWeb.Models;

namespace TBillStaking.Pages
{
    public class IndexModel : PageModel
    {
        
        public string Message { get; set; }
        [ViewData]
        public string Wallet { get; set; }
        public List<NFTDetails> NFTs { get; set; }
        public List<NFTSaleDetails> NFTSales { get; set; }

        public string tvLocked { get; set; }
        public string tbillLocked { get; set; }
        public string tfuelLocked { get; set; }
        public string rewards { get; set; }

        public string lpWalletCount { get; set; }

        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration conf)
        {
            _configuration = conf;
        }
        public void OnGet()
        {

            NFTs = new List<NFTDetails>();
            NFTSales = new List<NFTSaleDetails>();
            if (!String.IsNullOrEmpty(HttpContext.Request.Query["wallet"]))
            {
                Wallet = HttpContext.Request.Query["wallet"];
            }
            string connString = _configuration.GetConnectionString("sql-tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                using (var command = new SqlCommand("usp_getNFTDetails", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            NFTDetails nft = new NFTDetails();
                            nft.Name = reader.GetString("name");
                            nft.Sold = reader.GetInt32("sold");
                            nft.CurrentSalePrice = reader.GetDecimal("CurrentSalePrice");
                            nft.CurrentSalePriceUsd = reader.GetDecimal("CurrentSalePriceUsd");
                            nft.AvailableForSale = reader.GetInt32("AvailableForSale");
                            nft.AvgPriceForNext5ForSale = reader.GetDecimal("AvgPriceForNext5ForSale");
                            NFTs.Add(nft);
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }

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
                            //tvLocked = reader.GetDecimal("tvLocked").ToString();
                            tvLocked = String.Format("{0:n}", reader.GetDecimal("tvLocked"));
                            tbillLocked = String.Format("{0:n}", reader.GetDecimal("tbillLocked"));
                            tfuelLocked = String.Format("{0:n}", reader.GetDecimal("tfuelLocked"));
                            rewards = String.Format("{0:n}", reader.GetDecimal("rewards"));

                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }


                using (var command = new SqlCommand("usp_getNFTSalesDetails", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    command.Parameters.Add("@top", SqlDbType.Int).Value = 50;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            NFTSaleDetails nft = new NFTSaleDetails();
                            nft.Name = reader.GetString("name");
                            nft.Timestamp = reader.GetDateTime("soldTimestamp");
                            nft.Price = reader.GetDecimal("price");
                            nft.PriceUsd = reader.GetDecimal("priceUsd");
                            nft.Buyer = reader.GetString("buyer");
                            NFTSales.Add(nft);
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }

                using (var command = new SqlCommand("[dbo].[ups_getLPWalletCount]", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    command.Parameters.Add("@top", SqlDbType.Int).Value = 1;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            lpWalletCount = reader.GetInt32("walletCount").ToString();
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
    }
}
