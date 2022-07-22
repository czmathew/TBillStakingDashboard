using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBillStaking.Models
{
    public class NFTValueInfo
    {
        public string Name { get; set; }
        //public Dictionary<string,decimal> Values { get; set; }

        public decimal CurrentSalePrice { get; set; }
        public decimal CurrentSalePriceUsd { get; set; }
        public decimal Last5salesAvg { get; set; }
        public decimal Last5salesAvgUsd { get; set; }
        public decimal LastSale { get; set; }
        public decimal LastSaleUsd { get; set; }

    }
}
