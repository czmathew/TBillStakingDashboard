using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TBillStakingDashboardFunctions
{
    public static class RefreshTFuelStatsTrigger
    {
        [FunctionName("RefreshTFuelStatsTrigger")]
        public static void Run([TimerTrigger("0 0 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"RefreshTFuelStatsTrigger executed at: {DateTime.Now}");
            Exec.GetTFuelStats.Execute();
            log.LogInformation($"RefreshTFuelStatsTrigger finised at: {DateTime.Now}");
        }
    }
}
