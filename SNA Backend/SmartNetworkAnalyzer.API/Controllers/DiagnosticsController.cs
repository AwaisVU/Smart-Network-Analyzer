using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    }
}
