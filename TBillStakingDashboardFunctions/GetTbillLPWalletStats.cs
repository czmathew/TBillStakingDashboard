using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TBillStakingDashboardFunctions
{
    public static class GetTbillLPWalletStats
    {
        [FunctionName("GetTbillLPWalletStats")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Exec.GetTbillLPWalletStats.Execute();

            string responseMessage = "Function executed successfully.";


            return new OkObjectResult(responseMessage);
        }
    }
}
