using System;

namespace SmartNetworkAnalyzer.API.Contracts;

public class SessionDetailsResponse
{
    public Guid SessionId { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }
    public List<SessionProbeResultsResponse> Results { get; set; } = new();
}
