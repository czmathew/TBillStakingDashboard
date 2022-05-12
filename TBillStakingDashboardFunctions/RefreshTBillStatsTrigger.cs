using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TBillStakingDashboardFunctions
{
    public static class RefreshTBillStatsTrigger
    {
        [FunctionName("RefreshTBillStatsTrigger")]
        public static void Run([TimerTrigger("0 */10 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"RefreshTBillStatsTrigger executed at: {DateTime.Now}");
            Exec.GetTbillStats.Execute();
            Exec.GetTbillRates.Execute();
            log.LogInformation($"RefreshTBillStatsTrigger finised at: {DateTime.Now}");
        }
    }
}
