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

    }
}
