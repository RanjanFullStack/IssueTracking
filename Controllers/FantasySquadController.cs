using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using IssueTracking.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Microsoft.Extensions.Configuration;

namespace IssueTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FantasySquadController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public FantasySquadController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("getSquad")]
        public async Task<IActionResult> GetSquad()
        {
            var today = DateTime.Today;
            var apiName = "GetSquad";
            var existingResponse = _context.FantasySquadResponses.FirstOrDefault(r => r.Date == today && r.ApiName == apiName);

            if (existingResponse != null)
            {
                return Ok(existingResponse.Response);
            }

            var requestUri = _configuration["GetSquad:RequestUri"];
            var apiKey = _configuration["GetSquad:ApiKey"];

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{requestUri}?apikey={apiKey}&offset=0&id=0b12f428-98ab-4009-831d-493d325bc555"),
                Headers =
                {
                    { "accept", "application/json" },
                }
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(body);

                var formattedResponse = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var fantasySquadResponse = new FantasySquadResponse
                {
                    Date = today,
                    Response = formattedResponse,
                    ApiName = apiName
                };

                _context.FantasySquadResponses.Add(fantasySquadResponse);
                await _context.SaveChangesAsync();

                return Ok(formattedResponse);
            }
        }

        [HttpGet("getCurrentMatches")]
        public async Task<IActionResult> GetCurrentMatches()
        {
            var today = DateTime.Today;
            var apiName = "GetCurrentMatches";
            var existingResponse = _context.FantasySquadResponses.FirstOrDefault(r => r.Date == today && r.ApiName == apiName);

            if (existingResponse != null)
            {
                return Ok(existingResponse.Response);
            }

            var requestUri = _configuration["GetCurrentMatches:RequestUri"];
            var apiKey = _configuration["GetCurrentMatches:ApiKey"];

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{requestUri}?apikey={apiKey}&offset=0"),
                Headers =
                {
                    { "accept", "application/json" },
                }
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(body);

                var formattedResponse = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var fantasySquadResponse = new FantasySquadResponse
                {
                    Date = today,
                    Response = formattedResponse,
                    ApiName = apiName
                };

                _context.FantasySquadResponses.Add(fantasySquadResponse);
                await _context.SaveChangesAsync();

                return Ok(formattedResponse);
            }
        }
    }
}
