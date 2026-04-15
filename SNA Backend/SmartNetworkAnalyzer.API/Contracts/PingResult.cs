using System;

namespace SmartNetworkAnalyzer.API.Contracts;

public class PingResult
{
    public string TargetHost { get; set; } = string.Empty;
    public long LatencyMs { get; set; }
    public bool Success { get; set; }
    public DateTime Timestamp { get; set; }
    public string Message { get; set; } = string.Empty;
}

