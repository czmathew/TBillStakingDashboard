using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TBillStakingDashboardFunctions
{
    public static class RefreshNTFsFunctionTrigger
    {
        [FunctionName("RefreshNTFsFunctionTrigger")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"RefreshNTFsFunctionTrigger executed at: {DateTime.Now}");
            //Exec.GetTBillNTFSales.Execute();
            Exec.GetTBillNTFForSale.Execute();

            log.LogInformation($"RefreshNTFsFunctionTrigger finised at: {DateTime.Now}");
        }
    }
}
