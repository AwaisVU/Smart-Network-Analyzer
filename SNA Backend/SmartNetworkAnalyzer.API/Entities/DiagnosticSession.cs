using System;

namespace SmartNetworkAnalyzer.API.Entities;

public class DiagnosticSession
{
    public Guid Id {get; set;}
    public string UserId { get; set; } = string.Empty;
    public DateTime StartedAtUtc { get; set; }

    public DateTime? EndedAtUtc { get; set; }
}
