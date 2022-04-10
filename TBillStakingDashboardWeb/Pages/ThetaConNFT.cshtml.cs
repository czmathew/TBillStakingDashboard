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
    public class ThetaConNFTModel : PageModel
    {
        [ViewData]
        public string Wallet { get; set; }
        public List<ThetaConNFTSalesDaily> NFTs { get; set; }

        private readonly IConfiguration _configuration;

        public ThetaConNFTModel(IConfiguration conf)
        {
            _configuration = conf;
        }

        public void OnGet()
        {
            NFTs = new List<ThetaConNFTSalesDaily>();

            string connString = _configuration.GetConnectionString("sql-tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                using (var command = new SqlCommand("usp_getThetaconNFTSalesDaily", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            ThetaConNFTSalesDaily nft = new ThetaConNFTSalesDaily();
                            nft.DateSold = reader.GetString("dateSold");
                            nft.ImageURL = reader.GetString("image");
                            nft.Creator = reader.GetString("Creator");
                            nft.Price = reader.GetString("price");
                            NFTs.Add(nft);
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
