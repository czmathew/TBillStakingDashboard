using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TBillStaking.Models;
using TBillStakingDashboardWeb.Models;

namespace TBillStaking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Api : ControllerBase
    {
        private string apiBase = "https://api.gpool.io/tbill/";
        private readonly IConfiguration _configuration;
        //private readonly ILogger<WeatherForecastController> _logger;

        //public Api(ILogger<WeatherForecastController> logger)
        //{
        //    _logger = logger;
        //}

        public Api(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //[HttpGet]
        //[HttpGet("getIp")]
        //public IActionResult GetIP()
        //{
        //    string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
        //    var externalIp = IPAddress.Parse(externalIpString);

        //    return Ok(externalIp.ToString()) ;
        //}

        [HttpGet]
        [HttpGet("rates")]
        public async Task GetRate()
        {
            string jsonUrl = apiBase + "rates";
            HttpContext.Response.ContentType = "application/json";
            using (var client = new System.Net.WebClient())
            {
                try
                {
                    byte[] bytes = await client.DownloadDataTaskAsync(jsonUrl);
                    //write to response stream aka Response.Body
                    await HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                }
                catch (Exception e)//404 or anything
                {
                    HttpContext.Response.StatusCode = 400;//BadRequest
                }
                await HttpContext.Response.Body.FlushAsync();
                HttpContext.Response.Body.Close();
            }
        }

        [HttpGet]
        [HttpGet("rewards/{wallet}")]
        public async Task GetRewards(string wallet)
        {
            string jsonUrl = apiBase + "rewards?wallet=" + wallet;
            HttpContext.Response.ContentType = "application/json";
            using (var client = new System.Net.WebClient())
            {
                try
                {
                    byte[] bytes = await client.DownloadDataTaskAsync(jsonUrl);
                    //write to response stream aka Response.Body
                    await HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                }
                catch (Exception e)//404 or anything
                {
                    HttpContext.Response.StatusCode = 400;//BadRequest
                }
                await HttpContext.Response.Body.FlushAsync();
                HttpContext.Response.Body.Close();
            }
        }


        [HttpGet]
        [HttpGet("my-overview/{wallet}")]
        public async Task GetMyOverview(string wallet)
        {
            string jsonUrl = apiBase + "my-overview?wallet=" + wallet;
            HttpContext.Response.ContentType = "application/json";
            using (var client = new System.Net.WebClient())
            {
                try
                {
                    byte[] bytes = await client.DownloadDataTaskAsync(jsonUrl);
                    //write to response stream aka Response.Body
                    await HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                }
                catch (Exception e)//404 or anything
                {
                    HttpContext.Response.StatusCode = 400;//BadRequest
                }
                await HttpContext.Response.Body.FlushAsync();
                HttpContext.Response.Body.Close();
            }
        }


        [HttpGet]
        [HttpGet("my-overview-zeroday/{wallet}")]
        public async Task GetMyOverviewZeroday(string wallet)
        {
            string jsonUrl = apiBase + "my-overview?snapshot=2&wallet=" + wallet;
            HttpContext.Response.ContentType = "application/json";
            using (var client = new System.Net.WebClient())
            {
                try
                {
                    byte[] bytes = await client.DownloadDataTaskAsync(jsonUrl);
                    //write to response stream aka Response.Body
                    await HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                }
                catch (Exception e)//404 or anything
                {
                    HttpContext.Response.StatusCode = 400;//BadRequest
                }
                await HttpContext.Response.Body.FlushAsync();
                HttpContext.Response.Body.Close();
            }
        }

        [HttpGet]
        [HttpGet("dailyRewards/{wallet}")]
        public async Task GetDailyRewards(string wallet)
        {
            string jsonUrl = apiBase + "rewards-csv?wallet=" + wallet + "&mode=raw";
            HttpContext.Response.ContentType = "application/json";
            using (var client = new System.Net.WebClient())
            {
                try
                {
                    byte[] bytes = await client.DownloadDataTaskAsync(jsonUrl);
                    //write to response stream aka Response.Body
                    await HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                }
                catch (Exception e)//404 or anything
                {
                    HttpContext.Response.StatusCode = 400;//BadRequest
                }
                await HttpContext.Response.Body.FlushAsync();
                HttpContext.Response.Body.Close();
            }
        }

        [HttpGet]
        [HttpGet("balanceGnote/{wallet}")]
        public async Task GetBalanceGnote(string wallet)
        {
            string jsonUrl = "https://thetastats-nodejs-dev.azurewebsites.net/balance?contract=0xA3d79C4088aE87EF59254120Fe646560828084c3&wallet=" + wallet;
            HttpContext.Response.ContentType = "application/json";
            using (var client = new System.Net.WebClient())
            {
                try
                {
                    byte[] bytes = await client.DownloadDataTaskAsync(jsonUrl);
                    //write to response stream aka Response.Body
                    await HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                }
                catch (Exception e)//404 or anything
                {
                    HttpContext.Response.StatusCode = 400;//BadRequest
                }
                await HttpContext.Response.Body.FlushAsync();
                HttpContext.Response.Body.Close();
            }
        }

        [HttpGet]
        [HttpGet("balanceTdrop/{wallet}")]
        public IActionResult GetBalanceTdrop(string wallet)
        {
            string balance = "";
            string staked = "";
            string jsonUrl = "https://thetastats-nodejs-dev.azurewebsites.net/balance?contract=0x1336739b05c7ab8a526d40dcc0d04a826b5f8b03&wallet=" + wallet;
            string urlStaked = "https://thetastats-nodejs-dev.azurewebsites.net/tdropStaked?wallet=" + wallet;
            HttpContext.Response.ContentType = "application/json";
            using (var client = new HttpClient())
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, jsonUrl);
                    var response = client.Send(request);
                    
                    using var reader = new StreamReader(response.Content.ReadAsStream());
                    var responseBody = reader.ReadToEnd();
                    var balJson = JObject.Parse(responseBody);
                    balance = balJson.GetValue("balance").ToString();
                }
                catch (Exception e)//404 or anything
                {
                    HttpContext.Response.StatusCode = 400;//BadRequest
                }
                
            }
            
            using (var client = new HttpClient())
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, urlStaked);
                    var response = client.Send(request);

                    using var reader = new StreamReader(response.Content.ReadAsStream());
                    var responseBody = reader.ReadToEnd();
                    staked = responseBody;
                }
                catch (Exception e)//404 or anything
                {
                    HttpContext.Response.StatusCode = 400;//BadRequest
                }
                
            }
            var resp = new
            {
                balance = balance,
                staked = staked
            };
            return Ok(resp);
        }

        [HttpGet]
        [HttpGet("getPriceAll")]
        public async Task GetPriceAll()
        {
            string jsonUrl = "https://explorer.thetatoken.org:8443/api/price/all";
            HttpContext.Response.ContentType = "application/json";
            using (var client = new System.Net.WebClient())
            {
                try
                {
                    byte[] bytes = await client.DownloadDataTaskAsync(jsonUrl);
                    //write to response stream aka Response.Body
                    await HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                }
                catch (Exception e)//404 or anything
                {
                    HttpContext.Response.StatusCode = 400;//BadRequest
                }
                await HttpContext.Response.Body.FlushAsync();
                HttpContext.Response.Body.Close();
            }
        }



        [HttpGet]
        [HttpGet("totalSupplyGnote/")]
        public async Task getTotalSupplyGnote(string wallet)
        {
            string jsonUrl = "https://thetastats-nodejs-dev.azurewebsites.net/totalSupplyForTNT20?contract=0xA3d79C4088aE87EF59254120Fe646560828084c3";
            HttpContext.Response.ContentType = "application/json";
            using (var client = new System.Net.WebClient())
            {
                try
                {
                    byte[] bytes = await client.DownloadDataTaskAsync(jsonUrl);
                    //write to response stream aka Response.Body
                    await HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                }
                catch (Exception e)//404 or anything
                {
                    HttpContext.Response.StatusCode = 400;//BadRequest
                }
                await HttpContext.Response.Body.FlushAsync();
                HttpContext.Response.Body.Close();
            }
        }


        [HttpGet]
        [HttpGet("getDailyTBillStats/{days}")]
        public IActionResult GetDailyTBillStats(int days)
        {
            string connString = _configuration.GetConnectionString("sql_tbill");
            List<TBillDailyStats> stats = new List<TBillDailyStats>();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("usp_getDailyTBillStatsHourly", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    connection.Open();
                    command.Parameters.Add("@top", SqlDbType.Int).Value = days * 24;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            TBillDailyStats day = new TBillDailyStats();
                            day.Date = reader.GetString("date");
                            day.TvLocked = Convert.ToInt32(reader.GetDecimal("tvLocked")).ToString();
                            day.TbillLocked = Convert.ToInt32(reader.GetDecimal("tbillLocked")).ToString();
                            day.TfuelLocked = Convert.ToInt32(reader.GetDecimal("tfuelLocked")).ToString();
                            day.Rewards = Convert.ToInt32(reader.GetDecimal("rewards")).ToString();
                            stats.Add(day);
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }

                }
            }
            return Ok(JsonSerializer.Serialize(stats));
        }

        [HttpGet]
        [HttpGet("getBalance/{wallet}")]
        public IActionResult GetBalance(string wallet)
        {
            WalletBalance balance = new WalletBalance();

            // get theta tfuel
            string jsonUrl = "https://www.thetascan.io/api/balance/?address=" + wallet + "";
            HttpContext.Response.ContentType = "application/json";


            using (WebClient wc = new WebClient())
            {
                try
                {
                    var json = wc.DownloadString(jsonUrl);
                    var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                    balance.Theta = decimal.Parse(jsonClass.theta.ToString());
                    balance.TFuel = decimal.Parse(jsonClass.tfuel.ToString());
                    balance.ThetaStake = decimal.Parse(jsonClass.theta_staked.ToString());
                    balance.TFuelStake = decimal.Parse(jsonClass.tfuel_staked.ToString());

                }
                catch (Exception e)//404 or anything
                {
                    return Problem("Error reading data");
                    //HttpContext.Response.StatusCode = 400;//BadRequest
                }
            }

            //get TBill balance
            jsonUrl = "http://www.thetascan.io/api/contract/?contract=0x22cb20636c2d853de2b140c2eaddbfd6c3643a39&address=" + wallet + "";
            HttpContext.Response.ContentType = "application/json";
            using (WebClient wc = new WebClient())
            {
                try
                {
                    var json = wc.DownloadString(jsonUrl);
                    var jsonClass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                    //balance.TBill = decimal.Parse(jsonClass.balance.ToString(), CultureInfo.InvariantCulture);
                    balance.TBill = decimal.Parse(jsonClass.balance.ToString(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);

                }
                catch (Exception e)//404 or anything
                {
                    return Problem("Error reading data");
                    //HttpContext.Response.StatusCode = 400;//BadRequest
                }
            }


            return Ok(JsonSerializer.Serialize(balance));
        }

        [HttpGet]
        [HttpGet("getMyWalletLpStats/{wallet}")]
        public IActionResult GetMyWalletLpStats(string wallet)
        {
            List<MyWalletLpStats> lpStats = new List<MyWalletLpStats>();

            MyWalletLpStats lpStatsTfuel = new MyWalletLpStats();
            MyWalletLpStats lpStatsGnote = new MyWalletLpStats();

            string connString = _configuration.GetConnectionString("sql_tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("usp_getMyWalletLPStats", connection)
                {
                    CommandType = CommandType.StoredProcedure

                })
                {
                    connection.Open();
                    command.Parameters.Add("@wallet", SqlDbType.NVarChar).Value = wallet;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {

                            lpStatsTfuel.lpName = "tfuel";
                            lpStatsTfuel.Position = reader.GetInt32("position");
                            lpStatsTfuel.PositionTotal = reader.GetInt32("posTotal");
                            lpStatsTfuel.Univ2 = reader.GetDecimal("univ2");
                            lpStatsTfuel.Univ2Total = reader.GetDecimal("univ2Total");
                            lpStatsTfuel.MyPct = reader.GetDecimal("myPct");

                            lpStatsGnote.lpName = "gnote";
                            lpStatsGnote.Position = reader.GetInt32("positionGnote");
                            lpStatsGnote.PositionTotal = reader.GetInt32("posTotalGnote");
                            lpStatsGnote.Univ2 = reader.GetDecimal("univ2Gnote");
                            lpStatsGnote.Univ2Total = reader.GetDecimal("univ2TotalGnote");
                            lpStatsGnote.MyPct = reader.GetDecimal("myPctGnote");
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }

                using (var command = new SqlCommand("usp_getMyWalletLPStatsHistory", connection)
                {
                    CommandType = CommandType.StoredProcedure

                })
                {
                    command.Parameters.Add("@wallet", SqlDbType.NVarChar).Value = wallet;
                    SqlDataReader reader = command.ExecuteReader();
                    List<Tuple<string, string>> Univ2Hist = new List<Tuple<string, string>> { };
                    try
                    {
                        while (reader.Read())
                        {
                            Univ2Hist.Add(new Tuple<string, string>(reader.GetString("date"), reader.GetString("univ2")));
                        }
                        lpStatsTfuel.Univ2Hist = Univ2Hist;
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }

                using (var command = new SqlCommand("usp_getMyWalletLPStatsHistoryGnote", connection)
                {
                    CommandType = CommandType.StoredProcedure

                })
                {
                    command.Parameters.Add("@wallet", SqlDbType.NVarChar).Value = wallet;
                    SqlDataReader reader = command.ExecuteReader();
                    List<Tuple<string, string>> Univ2Hist = new List<Tuple<string, string>> { };
                    try
                    {
                        while (reader.Read())
                        {
                            Univ2Hist.Add(new Tuple<string, string>(reader.GetString("date"), reader.GetString("univ2")));
                        }
                        lpStatsGnote.Univ2Hist = Univ2Hist;
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }

            }

            lpStats.Add(lpStatsTfuel);
            lpStats.Add(lpStatsGnote);


            return Ok(JsonSerializer.Serialize(lpStats));
        }

        [HttpGet]
        [HttpGet("getNFTValueInfo")]
        public IActionResult GetNFTValueInfo()
        {
            
            var NFTValueInfo = new List<NFTValueInfo> { };

            string connString = _configuration.GetConnectionString("sql_tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("usp_getNFTValueInfo", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    connection.Open();
                    //command.Parameters.Add("@NFTName", SqlDbType.NVarChar).Value = name;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            //Dictionary<string, decimal> values = new Dictionary<string, decimal>();
                            //values.Add("CurrentSalePrice", reader.GetDecimal("CurrentSalePrice"));
                            //values.Add("CurrentSalePriceUsd", reader.GetDecimal("CurrentSalePriceUsd"));
                            //values.Add("Last5salesAvg", reader.GetDecimal("last5salesAvg"));
                            //values.Add("Last5salesAvgUsd", reader.GetDecimal("last5salesAvgUsd"));
                            //values.Add("LastSale", reader.GetDecimal("lastSale"));
                            //values.Add("LastSaleUsd", reader.GetDecimal("lastSaleUsd"));

                            NFTValueInfo.Add(new NFTValueInfo
                            {
                                Name = reader.GetString("name"),
                                //Values = values
                                CurrentSalePrice = reader.GetDecimal("CurrentSalePrice"),
                                CurrentSalePriceUsd = reader.GetDecimal("CurrentSalePriceUsd"),
                                Last5salesAvg = reader.GetDecimal("last5salesAvg"),
                                Last5salesAvgUsd = reader.GetDecimal("last5salesAvgUsd"),
                                LastSale = reader.GetDecimal("lastSale"),
                                LastSaleUsd = reader.GetDecimal("lastSaleUsd")
                            });
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }


            return Ok(JsonSerializer.Serialize(NFTValueInfo));
        }

        [HttpPost]
        [HttpPost("getNFTSales")]
        public IActionResult GetNFTSales([FromForm] string name)
        {
            if (name == null)
            {
                return Problem("Invalid name");
            }

            var NFTSales = new List<Tuple<string, string>> { };

            string connString = _configuration.GetConnectionString("sql_tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("usp_getNFTSalesSingleNFT", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    connection.Open();
                    command.Parameters.Add("@NFTName", SqlDbType.NVarChar).Value = name;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            NFTSales.Add(new Tuple<string, string>(reader.GetString("date"),reader.GetString("price")));
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }


            return Ok(JsonSerializer.Serialize(NFTSales));
        }
        
        [HttpGet]
        [HttpGet("LptokenTfuelRatio/{days}")]
        public IActionResult GetLptokenTfuelRatio(int days)
        {
            
            var DailyRates = new List<Tuple<string, decimal>> { };

            string connString = _configuration.GetConnectionString("sql_tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("usp_getLptokenTfuelRatio", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    connection.Open();
                    command.Parameters.Add("@top", SqlDbType.Int).Value = days * 24;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            DailyRates.Add(new Tuple<string, decimal>(reader.GetString("date")
                               , reader.GetDecimal("ratio")));
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }


            return Ok(JsonSerializer.Serialize(DailyRates));
        }

        [HttpGet]
        [HttpGet("getDailyRates/{days}")]
        public IActionResult GetDailyRates(int days)
        {
            
            var DailyRates = new List<Tuple<string, string, string, decimal, string>> { };

            string connString = _configuration.GetConnectionString("sql_tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("usp_getDailyRates", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    connection.Open();
                    command.Parameters.Add("@top", SqlDbType.Int).Value = days * 24;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            DailyRates.Add(new Tuple<string, string, string, decimal, string>(reader.GetString("date")
                                , reader.GetDecimal("tbill_usd").ToString()
                                , reader.GetDecimal("tfuel_usd").ToString()
                                , reader.GetDecimal("ratio")
                                , reader.GetDecimal("gnote_usd").ToString()));
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }


            return Ok(JsonSerializer.Serialize(DailyRates));
        }

        [HttpGet]
        [HttpGet("getDailyRatesGnote/{days}")]
        public IActionResult GetDailyRatesGnote(int days)
        {

            var DailyRates = new List<Tuple<string, string>> { };

            string connString = _configuration.GetConnectionString("sql_tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("usp_getDailyRatesGnote", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    connection.Open();
                    command.Parameters.Add("@top", SqlDbType.Int).Value = days;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            DailyRates.Add(new Tuple<string, string>(reader.GetString("date")
                                , reader.GetDecimal("gnote_usd").ToString()));
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }


            return Ok(JsonSerializer.Serialize(DailyRates));
        }

        [HttpGet]
        [HttpGet("getBultPrice")]
        public IActionResult GetBultPrice([FromForm] string name)
        {

            var DailyRates = new List<Tuple<string, string, string>> { };

            string connString = _configuration.GetConnectionString("sql_tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("bult.usp_getBultPrice", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            DailyRates.Add(new Tuple<string, string, string>(reader.GetString("date")
                                , reader.GetString("bult_tfuel")
                                , reader.GetDecimal("tfuel_bult").ToString()));
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }


            return Ok(JsonSerializer.Serialize(DailyRates));
        }

        [HttpGet]
        [HttpGet("getGnoteTbillPrice/{days}")]
        public IActionResult GetGnoteTbillPrice([FromForm] string name, int days)
        {

            var DailyRates = new List<Tuple<string, string, string>> { };

            string connString = _configuration.GetConnectionString("sql_tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("dbo.usp_getGnoteTbillPrice", connection)
                {
                    CommandType = CommandType.StoredProcedure

                })
                {
                    connection.Open();
                    command.Parameters.Add("@top", SqlDbType.Int).Value = days;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            DailyRates.Add(new Tuple<string, string, string>(reader.GetString("date")
                                , reader.GetString("gnote_tbill")
                                , reader.GetString("tbill_gnote").ToString()));
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }


            return Ok(JsonSerializer.Serialize(DailyRates));
        }
        [HttpGet]
        [HttpGet("getTbillPrice/{days}")]
        public IActionResult GetTbillPrice([FromForm] string name, int days)
        {

            var DailyRates = new List<Tuple<string, string, string, string>> { };

            string connString = _configuration.GetConnectionString("sql_tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("dbo.usp_getTbillPrices", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    connection.Open();
                    command.Parameters.Add("@top", SqlDbType.Int).Value = days * 24;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            DailyRates.Add(new Tuple<string, string, string, string>(reader.GetString("date")
                                , reader.GetString("tbill_usd")
                                , reader.GetString("targetRate")
                                , reader.GetString("rebaseRate").ToString()));
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }


            return Ok(JsonSerializer.Serialize(DailyRates));
        }

        [HttpGet]
        [HttpGet("getRebaseStats/{days}")]
        public IActionResult GetRebaseStats(int days)
        {

            var DailyRebase = new List<Tuple<string, string, string, string>> { };

            string connString = _configuration.GetConnectionString("sql_tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("dbo.usp_getRebaseStats", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    connection.Open();
                    command.Parameters.Add("@top", SqlDbType.Int).Value = days;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            DailyRebase.Add(new Tuple<string, string, string, string>(reader.GetString("date")
                                , reader.GetDecimal("supplyToday").ToString(CultureInfo.InvariantCulture)
                                , reader.GetDecimal("supplyYesterday").ToString(CultureInfo.InvariantCulture)
                                , reader.GetDecimal("rebasePercentage").ToString(CultureInfo.InvariantCulture)));
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }


            return Ok(JsonSerializer.Serialize(DailyRebase));
        }

        [HttpGet]
        [HttpGet("getDailyLPToken/{days}")]
        public IActionResult GetDailyLPToken(int days)
        {

            var DailyLPToken = new List<Tuple<string, string>> { };

            string connString = _configuration.GetConnectionString("sql_tbill");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("usp_getDailyLPToken", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    connection.Open();
                    command.Parameters.Add("@top", SqlDbType.Int).Value = days * 24;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            DailyLPToken.Add(new Tuple<string, string>(reader.GetString("date")
                                , reader.GetDecimal("lpTokenRate").ToString()));
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }


            return Ok(JsonSerializer.Serialize(DailyLPToken));
        }

        [HttpPost]
        [HttpPost("getNFTforWallet")]
        public IActionResult GetNFTforWallet([FromForm] string walletAddress)
        {
            if (walletAddress == null)
            {
                return Problem("Invalid wallet address");
            }

            string NFTs2x = "0x172d0bd953566538f050aabfeef5e2e8143e09f4";
            string NFTBigDog1111sticker = "0x3a8246be5efc8660a3618aefd9d767ae47df3c77";
            string NFTAlienlikeTBILLSticker = "0x4de555c77fddab5d40310e3cba254a41647c3af7";
            string NFTMichelleWhitedoveTBILLSticker = "0x77a2d407363c2d68d8cd1d71ec999667c2057c6a";
            string NFTTeddyBSticker = "0x7ed33985d23d39310c01d2becd934991dcedaf03";
            string NFTC4CSticker = "0x44edcfd52ea180c91d6ffb340b0bc2a8acb999c8";


            //get all NFTs for wallet
            string jsonUrl = "http://www.thetascan.io/api/721/?address=" + walletAddress + "&type=list&sort=contract" + "";
            //string jsonUrl = "http://www.thetascan.io/api/721/?contract=0x172d0bd953566538f050aabfeef5e2e8143e09f4&address=" + walletAddress + "";
            HttpContext.Response.ContentType = "application/json";

            var tokenList2x = new List<int> { };
            var tokenListBigDog1111 = new List<int> { };
            var tokenAlienlike = new List<int> { };
            var tokenMW = new List<int> { };
            var tokenTeddyB = new List<int> { };
            var tokenC4C = new List<int> { };
            var unknownNFT = new List<(string, int)> { };
            List<NFTInWallet> nfts = new List<NFTInWallet>();

            using (WebClient wc = new WebClient())
            {
                try
                {
                    var json = wc.DownloadString(jsonUrl);
                    if (json.ToString().Equals("null"))
                    {
                        //no NFTs for wallet
                        return Ok(JsonSerializer.Serialize(nfts));
                    }
                    var data = JArray.Parse(json);
                    foreach (JObject item in data) // <-- Note that here we used JObject instead of usual JProperty
                    {
                        string contract = item.GetValue("contract").ToString();
                        string token = item.GetValue("token").ToString();
                        if (contract.Equals(NFTs2x))
                        {
                            tokenList2x.Add(int.Parse(token));
                        }
                        else if (contract.Equals(NFTBigDog1111sticker))
                        {
                            tokenListBigDog1111.Add(int.Parse(token));

                        }
                        else if (contract.Equals(NFTAlienlikeTBILLSticker))
                        {
                            tokenAlienlike.Add(int.Parse(token));

                        }
                        else if (contract.Equals(NFTMichelleWhitedoveTBILLSticker))
                        {
                            tokenMW.Add(int.Parse(token));

                        }
                        else if (contract.Equals(NFTTeddyBSticker))
                        {
                            tokenTeddyB.Add(int.Parse(token));

                        }
                        else if (contract.Equals(NFTC4CSticker))
                        {
                            tokenC4C.Add(int.Parse(token));

                        }

                    }
                }
                catch (Exception e)//404 or anything
                {
                    return Problem("Error reading data");
                    //HttpContext.Response.StatusCode = 400;//BadRequest
                }
            }


            if (tokenList2x.Count > 0)
            {
                string connString = _configuration.GetConnectionString("sql_tbill");
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    using (var command = new SqlCommand("usp_getDailyTBillStats", connection)
                    {
                        CommandType = CommandType.Text,
                        Connection = connection

                    })
                    {
                        //TODO - fix this query to do left join on edition/tokenId instead of name to get the image
                        var sql = "SELECT [name]" +
                            ",[multiplier]" +
                            ",[tbillAmount]" +
                            ",[boostPercentage]" +
                            ",[edition]" +
                            ",(select top 1 replace(image,'ipfs://','') from dbo.nftMinted m where n.name = m.name) nftImage" +
                            " FROM [dbo].[nftMinted] n WHERE [edition] in ({0})";
                        var index = 0;
                        var parameterList = new List<string>();
                        foreach (var id in tokenList2x)
                        {
                            var paramName = "@idParam" + index;
                            command.Parameters.AddWithValue(paramName, id);
                            parameterList.Add(paramName);
                            index++;
                        }
                        command.CommandText = string.Format(sql, string.Join(",", parameterList));
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                //FIXME - quick fix to make sure there is no link to non existing image
                                string imageFromDB = reader.GetString("nftImage");
                                string image = "";
                                if (!imageFromDB.EndsWith(".gif")) { 
                                    image = "/img/nft/" + reader.GetString("nftImage") + "_30.gif";
                                }
                                NFTInWallet nft = new NFTInWallet();
                                nft.Name = reader.GetString("name");
                                nft.ImageURL = image;
                                nft.Multiplier = reader.GetString("multiplier");
                                nft.TbillAmount = reader.GetInt32("tbillAmount");
                                nft.BoostPercentage = reader.GetInt32("boostPercentage");
                                nft.Edition = reader.GetInt32("edition");
                                nft.Count = 1;
                                nfts.Add(nft);

                                //remove existing NFTs and later check if there are still any left
                                tokenList2x.Remove(reader.GetInt32("edition"));
                            }
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }
                }
            }

            if (tokenList2x.Count > 0)
            {
                string connString = _configuration.GetConnectionString("sql_tbill");
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();
                    foreach (int tokenId in tokenList2x)
                    {
                        using (var command = new SqlCommand("usp_insertNFTtoQueue", connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        })
                        {

                            command.Parameters.Add("@contract", SqlDbType.NVarChar).Value = NFTs2x;
                            command.Parameters.Add("@tokenId", SqlDbType.Int).Value = tokenId;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

            //BigDog NFT
            if (tokenListBigDog1111.Count > 0)
            {
                string connString = _configuration.GetConnectionString("sql_tbill");
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    using (var command = new SqlCommand("usp_getDailyTBillStats", connection)
                    {
                        CommandType = CommandType.Text,
                        Connection = connection

                    })
                    {
                        var sql = "SELECT [name]" +
                            ",replace(image,'ipfs://','')  nftImage" +
                            " FROM [dbo].[nftMintedDeGreatMerge] n WHERE [contract] = '" + NFTBigDog1111sticker + "'";
                        var parameterList = new List<string>();

                        command.CommandText = sql;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                NFTInWallet nft = new NFTInWallet();
                                nft.Name = reader.GetString("name") + " (total: " + tokenListBigDog1111.Count.ToString() + ")";
                                nft.ImageURL = "/img/nft/" + reader.GetString("nftImage") + ".png";
                                nft.Multiplier = "1.25x";
                                nft.TbillAmount = 100;
                                nft.BoostPercentage = 25;
                                nft.Edition = 0;
                                nft.Count = tokenListBigDog1111.Count;
                                nfts.Add(nft);
                            }
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }
                }
            }


            //Alienlike TBILL Sticker
            if (tokenAlienlike.Count > 0)
            {
                string connString = _configuration.GetConnectionString("sql_tbill");
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    using (var command = new SqlCommand("usp_getDailyTBillStats", connection)
                    {
                        CommandType = CommandType.Text,
                        Connection = connection

                    })
                    {
                        var sql = "SELECT [name]" +
                            ",replace(image,'ipfs://','')  nftImage" +
                            " FROM [dbo].[nftMintedDeGreatMerge] n WHERE [contract] = '" + NFTAlienlikeTBILLSticker + "'";
                        var parameterList = new List<string>();

                        command.CommandText = sql;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                NFTInWallet nft = new NFTInWallet();
                                nft.Name = reader.GetString("name") + " (total: " + tokenAlienlike.Count.ToString() + ")";
                                nft.ImageURL = "/img/nft/" + reader.GetString("nftImage") + ".jpg";
                                nft.Multiplier = "1.25x";
                                nft.TbillAmount = 100;
                                nft.BoostPercentage = 25;
                                nft.Edition = 0;
                                nft.Count = tokenAlienlike.Count;
                                nfts.Add(nft);
                            }
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }
                }
            }

            //Michelle Whitedove TBILL Sticker
            if (tokenMW.Count > 0)
            {
                string connString = _configuration.GetConnectionString("sql_tbill");
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    using (var command = new SqlCommand("usp_getDailyTBillStats", connection)
                    {
                        CommandType = CommandType.Text,
                        Connection = connection

                    })
                    {
                        var sql = "SELECT [name]" +
                            ",replace(image,'ipfs://','')  nftImage" +
                            " FROM [dbo].[nftMintedDeGreatMerge] n WHERE [contract] = '" + NFTMichelleWhitedoveTBILLSticker + "'";
                        var parameterList = new List<string>();

                        command.CommandText = sql;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                NFTInWallet nft = new NFTInWallet();
                                nft.Name = reader.GetString("name") + " (total: " + tokenMW.Count.ToString() + ")";
                                nft.ImageURL = "/img/nft/" + reader.GetString("nftImage") + ".jpg";
                                nft.Multiplier = "1.25x";
                                nft.TbillAmount = 100;
                                nft.BoostPercentage = 25;
                                nft.Edition = 0;
                                nft.Count = tokenMW.Count;
                                nfts.Add(nft);
                            }
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }
                }
            }

            //Teddy B TBILL Sticker
            if (tokenTeddyB.Count > 0)
            {
                string connString = _configuration.GetConnectionString("sql_tbill");
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    using (var command = new SqlCommand("usp_getDailyTBillStats", connection)
                    {
                        CommandType = CommandType.Text,
                        Connection = connection

                    })
                    {
                        var sql = "SELECT [name]" +
                            ",replace(image,'ipfs://','')  nftImage" +
                            " FROM [dbo].[nftMintedDeGreatMerge] n WHERE [contract] = '" + NFTTeddyBSticker + "'";
                        var parameterList = new List<string>();

                        command.CommandText = sql;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                NFTInWallet nft = new NFTInWallet();
                                nft.Name = reader.GetString("name") + " (total: " + tokenTeddyB.Count.ToString() + ")";
                                nft.ImageURL = "/img/nft/" + reader.GetString("nftImage") + ".jpg";
                                nft.Multiplier = "1.25x";
                                nft.TbillAmount = 100;
                                nft.BoostPercentage = 25;
                                nft.Edition = 0;
                                nft.Count = tokenTeddyB.Count;
                                nfts.Add(nft);
                            }
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }
                }
            }

            //C4C TBILL Sticker
            if (tokenC4C.Count > 0)
            {
                string connString = _configuration.GetConnectionString("sql_tbill");
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    using (var command = new SqlCommand("usp_getDailyTBillStats", connection)
                    {
                        CommandType = CommandType.Text,
                        Connection = connection

                    })
                    {
                        var sql = "SELECT [name]" +
                            ",replace(image,'ipfs://','')  nftImage" +
                            " FROM [dbo].[nftMintedDeGreatMerge] n WHERE [contract] = '" + NFTC4CSticker + "'";
                        var parameterList = new List<string>();

                        command.CommandText = sql;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                NFTInWallet nft = new NFTInWallet();
                                nft.Name = reader.GetString("name") + " (total: " + tokenC4C.Count.ToString() + ")";
                                nft.ImageURL = "/img/nft/" + reader.GetString("nftImage") + ".jpg";
                                nft.Multiplier = "1.25x";
                                nft.TbillAmount = 100;
                                nft.BoostPercentage = 25;
                                nft.Edition = 0;
                                nft.Count = tokenC4C.Count;
                                nfts.Add(nft);
                            }
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }
                }
            }


            return Ok(JsonSerializer.Serialize(nfts));
        }
    }
}
