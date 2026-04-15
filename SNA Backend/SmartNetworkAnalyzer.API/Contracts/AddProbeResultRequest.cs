namespace SmartNetworkAnalyzer.API.Contracts;

public sealed record AddProbeResultRequest(
    string ProbeType,
    bool Success,
    int? LatencyMs,
    string? Error,
    DateTime? TimestampUtc
);

