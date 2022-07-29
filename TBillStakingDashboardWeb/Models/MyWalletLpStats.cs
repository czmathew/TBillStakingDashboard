using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBillStaking.Models
{
    public class MyWalletLpStats
    {
        public string lpName { get; set; }
        public int Position { get; set; }
        public int PositionTotal { get; set; }
        public decimal Univ2 { get; set; }
        public decimal Univ2Total { get; set; }
        public decimal MyPct { get; set; }
        public List<Tuple<string, string>> Univ2Hist { get; set; }

    }
}
