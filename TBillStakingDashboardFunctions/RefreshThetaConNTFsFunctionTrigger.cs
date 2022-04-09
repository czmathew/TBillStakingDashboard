using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TBillStakingDashboardFunctions
{
    public static class RefreshThetaConNTFsFunctionTrigger
    {
        [FunctionName("RefreshThetaConNTFsFunctionTrigger")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"RefreshThetaConNTFsFunctionTrigger executed at: {DateTime.Now}");
            Exec.GetThetaConNTFSales.Execute();
            log.LogInformation($"RefreshThetaConNTFsFunctionTrigger finised at: {DateTime.Now}");
        }
    }
}
