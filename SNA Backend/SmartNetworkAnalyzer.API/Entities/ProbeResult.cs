using System;

namespace SmartNetworkAnalyzer.API.Entities;

public class ProbeResult
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public DateTime TimestampUtc { get; set; }
    public string ProbeType { get; set; } = string.Empty;
    public bool Success { get; set; }
    public int? LatencyMs { get; set; }
    public string? Error { get; set; }

}
