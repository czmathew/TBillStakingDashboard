using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBillStakingDashboardWeb.Models
{
    public class TBillDailyStats
    {
        public string Date { get; set; }
        public string TvLocked { get; set; }
        public string TbillLocked { get; set; }
        public string TfuelLocked { get; set; }
        public string Rewards { get; set; }

    }
}
