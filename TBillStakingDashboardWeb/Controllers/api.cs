using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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
    }
}
