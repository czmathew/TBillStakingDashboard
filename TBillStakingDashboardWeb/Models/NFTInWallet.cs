using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBillStaking.Models
{
    public class NFTInWallet
    {
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string Multiplier { get; set; }
        public int TbillAmount { get; set; }
        public int BoostPercentage { get; set; }
        public int Edition { get; set; }
    }
}
