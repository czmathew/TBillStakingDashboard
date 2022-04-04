using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBillStaking.Models
{
    public class WalletBalance
    {
        public decimal Theta { get; set; }
        public decimal ThetaStake { get; set; }
        public decimal TFuel { get; set; }
        public decimal TFuelStake { get; set; }
        public decimal TBill { get; set; }

    }
}
