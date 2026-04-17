using System;

namespace SmartNetworkAnalyzer.API.Contracts;

public class SessionProbeResultsResponse
{
    public DateTime TimestampUtc { get; set; }
    public string ProbeType { get; set; } = string.Empty;
    public bool Success { get; set; }
    public int? LatencyMs { get; set; }
    public string? Error { get; set; } 
}
