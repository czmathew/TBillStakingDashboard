using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TBillStakingDashboardFunctions
{
    public static class GetGnoteTbillPriceTrigger
    {
        [FunctionName("GetGnoteTbillPriceTrigger")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            Exec.GetGnoteTbillPrice.Execute();
            
        }
    }
}
