using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TBillStakingDashboardFunctions
{
    public static class RefreshNTFsFunctionTrigger
    {
        [FunctionName("RefreshNTFsFunctionTrigger")]
        public static void Run([TimerTrigger("0 */10 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"RefreshNTFsFunctionTrigger executed at: {DateTime.Now}");
            Exec.GetTBillNTFForSale.Execute(log);
            Exec.GetTBillNTFSales.Execute(log);

            log.LogInformation($"RefreshNTFsFunctionTrigger finised at: {DateTime.Now}");
        }
    }
}
