using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TBillStakingDashboardWeb.Pages
{
    public class NTFModel : PageModel
    {
        [ViewData]
        public string Wallet { get; set; }
        public void OnGet()
        {
            if (!String.IsNullOrEmpty(HttpContext.Request.Query["wallet"]))
            {
                Wallet = HttpContext.Request.Query["wallet"];
            }
        }
    }
}
