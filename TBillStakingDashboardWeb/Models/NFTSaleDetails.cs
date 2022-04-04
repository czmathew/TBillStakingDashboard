using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBillStakingDashboardWeb.Models
{
    public class NFTSaleDetails
    {
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Price { get; set; }
        public decimal PriceUsd { get; set; }
        public string Buyer { get; set; }
    }
}
