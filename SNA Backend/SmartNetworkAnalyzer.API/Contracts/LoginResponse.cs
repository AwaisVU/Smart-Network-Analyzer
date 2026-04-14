namespace SmartNetworkAnalyzer.API.Contracts;

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc);
