using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TBillStakingDashboardFunctions
{
    public static class GetGnoteTfuelPriceTrigger
    {
        [FunctionName("GetGnoteTfuelPriceTrigger")]
        public static void Run([TimerTrigger("0 */10 * * * *")]TimerInfo myTimer, ILogger log)
        {
            Exec.GetGnoteTfuelPrice.Execute();
            
        }
    }
}
