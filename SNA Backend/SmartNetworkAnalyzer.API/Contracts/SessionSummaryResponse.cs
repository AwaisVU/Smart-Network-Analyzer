using System;

namespace SmartNetworkAnalyzer.API.Contracts;

public class SessionSummaryResponse
{
    public Guid SessionId { get; set; }
    public string Mode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> NextSteps { get; set; } = [];
}
