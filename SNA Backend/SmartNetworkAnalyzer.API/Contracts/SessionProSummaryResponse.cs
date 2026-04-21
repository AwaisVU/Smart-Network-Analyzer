using System;

namespace SmartNetworkAnalyzer.API.Contracts;

public sealed class SessionProSummaryResponse
{
    public Guid SessionId { get; set; }
    public string Mode { get; set; } = "pro";
    public int Total { get; set; }
    public int Failures { get; set; }
    public double FailureRate { get; set; }
    public double AvgLatencyMs { get; set; }
    public string Message { get; set; } = string.Empty;
}

