using System;
using SmartNetworkAnalyzer.API.Models;
using System.Net.NetworkInformation;

namespace SmartNetworkAnalyzer.API.Services;

public class PingService : IPingService
{
    public async Task<PingResult> PingHostAsync (string host)
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(host);

            if(reply.Status == IPStatus.Success)
            {
                return new PingResult
                {
                    TargetHost = host,
                    LatencyMs = reply.RoundtripTime,
                    Success = true,
                    Timestamp = DateTime.UtcNow,
                    Message = "Ping successfull!"
                };
            }
            else
            {
                return new PingResult
                {
                    TargetHost = host,
                    LatencyMs = 0,
                    Success = false,
                    Timestamp = DateTime.UtcNow,
                    Message = "Ping Failed"
                };
            }
        }
        catch(Exception ex)
        {
            return new PingResult
            {
                TargetHost = host,
                LatencyMs = 0,
                Success = false,
                Timestamp = DateTime.UtcNow,
                Message = ex.Message.ToString()
            };
        }
    }
}
