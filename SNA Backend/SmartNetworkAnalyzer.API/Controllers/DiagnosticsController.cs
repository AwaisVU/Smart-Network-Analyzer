using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNetworkAnalyzer.API.Contracts;
using SmartNetworkAnalyzer.API.Data;
using SmartNetworkAnalyzer.API.Entities;

namespace SmartNetworkAnalyzer.API.Controllers
{
    [Route("api/diagnostics")]
    [ApiController]
    [Authorize]
    public class DiagnosticsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public DiagnosticsController (ApplicationDbContext db)
        {
            _db = db;
        }


        [HttpPost("sessions")]
        public async Task<IActionResult> CreateSession()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var session = new DiagnosticSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StartedAtUtc = DateTime.UtcNow,
                EndedAtUtc = null

            };

            _db.DiagnosticSessions.Add(session);
            await _db.SaveChangesAsync();

            return Created($"/api/diagnostics/sessions/{session.Id}", new CreateSessionResponse(session.Id));
        }


        //Addig Probe Result taken from AddProbeResultRequest
        [HttpPost("sessions/{sessionId:guid}/results")]
        public async Task<IActionResult> AddProbeResult(Guid sessionId, AddProbeResultRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var session = await _db.DiagnosticSessions.FindAsync(sessionId);
            if(session is null)
            {
                return NotFound();
            }
            if(session.UserId != userId)
            {
                return NotFound();
            }

            var probe = new ProbeResult
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                TimestampUtc = request.TimestampUtc ?? DateTime.UtcNow,
                ProbeType = request.ProbeType,
                Success = request.Success,
                LatencyMs = request.LatencyMs,
                Error = request.Error

            };

            _db.ProbeResults.Add(probe);
            await _db.SaveChangesAsync();


            return StatusCode(201);
        }


        [HttpGet("sessions/{sessionId:guid}")]
        public async Task<IActionResult> GetSession(Guid sessionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId is null)
            {
                return Unauthorized();
            }

            var session = await _db.DiagnosticSessions.FindAsync(sessionId);
            if(session is null)
            {
                return NotFound();
            }
            if(session.UserId != userId)
            {
                return NotFound();
            }
            
            var response = new SessionDetailsResponse
            {
                SessionId = session.Id,
                StartedAtUtc = session.StartedAtUtc,
                EndedAtUtc = session.EndedAtUtc,
            
            };

            var probeRows = await _db.ProbeResults
            .Where(x => x.SessionId == sessionId)
            .OrderBy(e => e.TimestampUtc)
            .Select(x => 
                new SessionProbeResultsResponse
                {
                    TimestampUtc = x.TimestampUtc,
                    ProbeType = x.ProbeType,
                    Success = x.Success,
                    LatencyMs = x.LatencyMs,
                    Error = x.Error
                }
            )
            .ToListAsync();
            
            response.Results = probeRows;
            return Ok(response);
        }

        [HttpGet("sessions/{sessionId:guid}/summary")]
        public async Task<IActionResult> GetSummary (Guid sessionId, string mode = "home")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            

            var session = await _db.DiagnosticSessions.FindAsync(sessionId);
            if(session is null)
            {
                return NotFound();
            }

            if(session.UserId != userId)
            {
                return NotFound();
            }

            var probeRows = await _db.ProbeResults
                .Where(x=>x.SessionId == sessionId)
                .OrderBy(x=>x.TimestampUtc)
                .ToListAsync();

            //Manual Probe Logic
            var total = probeRows.Count();
            var failures = probeRows.Count(x=>x.Success == false);
            var avgLatency = probeRows.Where(x=>x.LatencyMs.HasValue)
                                        .Select(x=>x.LatencyMs!.Value)
                                        .DefaultIfEmpty(0)
                                        .Average();

            double failureRate;
            if(total > 0)
            {
                failureRate = (double)failures / total;
            }
            else
            {
                failureRate = 0;
            }

            //Customer Guidelines
            string userMessage;
            List<string> userNextSteps;

            if(total == 0)
            {
                userMessage = "Not enough data yet. Run the test again.";
                userNextSteps = new List<string>();
            }
            else if (failureRate > 0.2)
            {
                userMessage = "Your connection looks unstable right now.";
                userNextSteps = new List<string>
                {
                    "Restart your router and wait 2 minutes", 
                    "Move closer to the Wi-Fi router and retry", 
                    "Try again from another device"
                };
            }
            else if (avgLatency >= 250)
            {
                userMessage = "Your connection looks slow right now.";
                userNextSteps = new List<string>
                {
                    "Close background downloads or streaming apps", 
                    "Restart your router", 
                    "Try switching Wi-Fi bands (2.4GHz / 5GHz)"
                };
            }
            else
            {
                userMessage = "Great! Your network connection is stable.";
                userNextSteps = new List<string>
                {
                    "No action needed, connection is healthy"
                };
            }


            //Conditional Response based on summary type

            if (mode == "home")
            {
                var summaryHome = new SessionSummaryResponse
                {
                    SessionId = sessionId,
                    Mode = "home",
                    Message = userMessage,
                    NextSteps = userNextSteps,
                };

                return Ok(summaryHome);
            }

            if (mode == "pro")
            {
                var summaryPro = new SessionProSummaryResponse {

                    SessionId = sessionId,
                    Mode = "pro",
                    Total = total,
                    Failures = failures,
                    FailureRate = failureRate,
                    AvgLatencyMs = avgLatency,


                };

                return Ok(summaryPro);
                
            }

            return BadRequest("mode must be home or pro");
        }

    }
}
