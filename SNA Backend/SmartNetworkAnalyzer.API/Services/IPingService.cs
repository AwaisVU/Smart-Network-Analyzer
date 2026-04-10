using System;
using SmartNetworkAnalyzer.API.Models;
using System.Threading.Tasks;

namespace SmartNetworkAnalyzer.API.Services;

public interface IPingService
{
    Task<PingResult> PingHostAsync(string host);
}
