using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBillStaking.Models
{
    public class NFTDetails
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string ImageURL { get; set; }
        public int Sold { get; set; }
        public int MintedTotal { get; set; }
        public decimal CurrentSalePrice { get; set; }
        public decimal CurrentSalePriceUsd { get; set; }

        public int AvailableForSale { get; set; }

        public decimal AvgPriceForNext5ForSale { get; set; }
    }
}
