using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TBillStakingDashboardFunctions
{
    public static class GetPricesTrigger
    {
        [FunctionName("GetPricesTrigger")]
        public static void Run([TimerTrigger("0 */10 * * * *")]TimerInfo myTimer, ILogger log)
        {
            Exec.GetPrices.Execute();
            
        }
    }
}
