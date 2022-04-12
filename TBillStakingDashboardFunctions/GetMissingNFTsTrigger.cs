using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TBillStakingDashboardFunctions
{
    public static class GetMissingNFTsTrigger
    {
        [FunctionName("GetMissingNFTsTrigger")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"RefreshNTFsFunctionTrigger executed at: {DateTime.Now}");
            Exec.GetMissingNFTs.Execute();
            log.LogInformation($"RefreshNTFsFunctionTrigger finised at: {DateTime.Now}");
        }
    }
}
