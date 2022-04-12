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

namespace TBillStakingDashboardWeb.Pages
{
    public class NTFModel : PageModel
    {
        [ViewData]
        public string Wallet { get; set; }
        public List<NFTDetails> NFTs { get; set; }
        public List<NFTSaleDetails> NFTSales { get; set; }
        public List<NFTSalesDaily> NFTsales { get; set; }
        private readonly IConfiguration _configuration;

        public NTFModel(IConfiguration conf)
        {
            _configuration = conf;
        }

        public void OnGet()
        {
            if (!String.IsNullOrEmpty(HttpContext.Request.Query["wallet"]))
            {
                Wallet = HttpContext.Request.Query["wallet"];
            }

            NFTs = new List<NFTDetails>();
            NFTSales = new List<NFTSaleDetails>();

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
                            nft.ImageURL = reader.GetString("nftImage").Equals("") ? "" : "/img/nft/" + reader.GetString("nftImage") + "_30.gif";
                            nft.Sold = reader.GetInt32("sold");
                            nft.MintedTotal = reader.GetInt32("totalMinted");
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
                            nft.ImageURL = reader.GetString("nftImage").Equals("") ? "" : "/img/nft/" + reader.GetString("nftImage") + "_30.gif";
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

                //get daily NFT sales for chart
                using (var command = new SqlCommand("usp_getNFTSalesDaily", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    //should it maybe be it's own class instead of the Tuple?
                    NFTsales = new List<NFTSalesDaily>();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {

                            NFTsales.Add(new NFTSalesDaily()
                            {
                                dateSold = reader.GetString("dateSold"),
                                priceAAA = reader.GetString("priceAAA"),
                                priceC4C = reader.GetString("priceC4C"),
                                price100K = reader.GetString("price100K"),
                                price500K = reader.GetString("price500K"),
                                price1M = reader.GetString("price1M"),
                                priceHodl = reader.GetString("priceHodl"),
                                priceLurk = reader.GetString("priceLurk"),
                                pricePatron = reader.GetString("pricePatron")
                            });

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
