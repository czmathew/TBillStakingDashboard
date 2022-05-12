using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TBillStakingDashboardFunctions
{
    public static class GetBultPriceTrigger
    {
        [FunctionName("GetBultPriceTrigger")]
        public static void Run([TimerTrigger("0 0 * * * *")]TimerInfo myTimer, ILogger log)
        {
            Exec.GetBultPrice.Execute();
            
        }
    }
}
