using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
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
        [HttpGet("getDailyTBillStats")]
        public IActionResult GetDailyTBillStats(string wallet)
        {
            string connString = _configuration.GetConnectionString("sql-tbill");
            List<TBillDailyStats> stats = new List<TBillDailyStats>();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("usp_getDailyTBillStats", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    connection.Open();
                    command.Parameters.Add("@top", SqlDbType.Int).Value = 365;
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            TBillDailyStats day = new TBillDailyStats();
                            day.Date = reader.GetDateTime("date").Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
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

        [HttpPost]
        [HttpPost("getNFTforWallet")]
        public IActionResult GetNFTforWallet([FromForm] string walletAddress)
        {
            if (walletAddress == null)
            {
                return Problem("Invalid wallet address");
            }
            string jsonUrl = "http://www.thetascan.io/api/721/?contract=0x172d0bd953566538f050aabfeef5e2e8143e09f4&address=" + walletAddress + "";
            HttpContext.Response.ContentType = "application/json";

            var tokenList = new List<int> { };

            using (WebClient wc = new WebClient())
            {
                try
                {
                    var json = wc.DownloadString(jsonUrl);
                    var data = JArray.Parse(json);
                    foreach (JObject item in data) // <-- Note that here we used JObject instead of usual JProperty
                    {
                        string token = item.GetValue("token").ToString();
                        tokenList.Add(int.Parse(token));
                    }
                }
                catch (Exception e)//404 or anything
                {
                    return Problem("Error reading data");
                    //HttpContext.Response.StatusCode = 400;//BadRequest
                }
            }
            List<NFTInWallet> nfts = new List<NFTInWallet>();
            if (tokenList.Count > 0)
            {
                string connString = _configuration.GetConnectionString("sql-tbill");
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    using (var command = new SqlCommand("usp_getDailyTBillStats", connection)
                    {
                        CommandType = CommandType.Text,
                        Connection = connection

                    })
                    {
                        var sql = "SELECT [name]" +
                            ",[multiplier]" +
                            ",[tbillAmount]" +
                            ",[boostPercentage]" +
                            ",[edition]" +
                            ",(select top 1 replace(image,':/','') from dbo.nftMinted m where n.name = m.name) nftImage" +
                            " FROM [dbo].[nftMinted] n WHERE [edition] in ({0})";
                        var index = 0;
                        var parameterList = new List<string>();
                        foreach (var id in tokenList)
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
                                NFTInWallet nft = new NFTInWallet();
                                nft.Name = reader.GetString("name");
                                nft.ImageURL = "https://ipfs.io/" + reader.GetString("nftImage");
                                nft.Multiplier = reader.GetString("multiplier");
                                nft.TbillAmount = reader.GetInt32("tbillAmount");
                                nft.BoostPercentage = reader.GetInt32("boostPercentage");
                                nft.Edition = reader.GetInt32("edition");
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
