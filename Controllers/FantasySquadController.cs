using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using IssueTracking.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace IssueTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FantasySquadController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly ApplicationDbContext _context;

        public FantasySquadController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getSquad")]
        public async Task<IActionResult> GetSquad()
        {
            var today = DateTime.Today;
            var existingResponse = _context.FantasySquadResponses.FirstOrDefault(r => r.Date == today);

            if (existingResponse != null)
            {
                return Ok(existingResponse.Response);
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.cricapi.com/v1/match_squad?apikey=d12c72ff-84f6-4d09-b3a3-a472fb71155a&offset=0&id=0b12f428-98ab-4009-831d-493d325bc555"),
                Headers =
                {
                    { "accept", "application/json" },
                },
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
                    Response = formattedResponse
                };

                _context.FantasySquadResponses.Add(fantasySquadResponse);
                await _context.SaveChangesAsync();

                return Ok(formattedResponse);
            }
        }
    }
}
