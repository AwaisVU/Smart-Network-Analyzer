using System;
using SmartNetworkAnalyzer.API.Contracts;
using System.Threading.Tasks;

namespace SmartNetworkAnalyzer.API.Services;

public interface IPingService
{
    Task<PingResult> PingHostAsync(string host);
}
