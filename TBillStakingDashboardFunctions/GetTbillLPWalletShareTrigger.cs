using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace TBillStakingDashboardFunctions
{
    public static class GetTbillLPWalletShareTrigger
    {
        [FunctionName("GetTbillLPWalletShareTrigger")]
        public static void Run([TimerTrigger("0 20 2 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"RefreshTBillStatsTrigger executed at: {DateTime.Now}");
            Exec.GetTBillLPStats.Execute();
            Exec.GetTBillGnoteLPStats.Execute();
            log.LogInformation($"RefreshTBillStatsTrigger finised at: {DateTime.Now}");
        }
    }
}
