using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TBillStaking.Models;

namespace TBillStaking.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Wallet { get; set; }
        public List<NFTDetails> NFTs { get; set; }

        public string tvLocked { get; set; }
        public string tbillLocked { get; set; }
        public string tfuelLocked { get; set; }
        public string rewards { get; set; }
        public void OnGet()
        {
            NFTs = new List<NFTDetails>();
            if (!String.IsNullOrEmpty(HttpContext.Request.Query["wallet"]))
            {
                Wallet = HttpContext.Request.Query["wallet"];
            }

            using (SqlConnection connection = new SqlConnection("Server=tcp:huhusql.database.windows.net,1433;Database=sql-tbill;User ID=functionUser;Password=GpZyjaYW0slVXQPFyST2;Trusted_Connection=False;Encrypt=True;"))
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


            }
        }
    }
}
