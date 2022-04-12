using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace TBillStakingDashboardFunctions
{
    public static class GetTbillLPWalletStatsTrigger
    {
        [FunctionName("GetTbillLPWalletStatsTrigger")]
        public static void Run([TimerTrigger("0 0 1 * * *")]TimerInfo myTimer, ILogger log)
        {
            //log.LogInformation($"RefreshTBillStatsTrigger executed at: {DateTime.Now}");
            
            //this has been replaced by GetTBillLPStats
            //we'll calculate number of wallets with univ2 token instead
            //Exec.GetTbillLPWalletStats.Execute();
            
            //log.LogInformation($"RefreshTBillStatsTrigger finised at: {DateTime.Now}");
        }
    }
}
