using System;
using Microsoft.AspNetCore.Identity;

namespace SmartNetworkAnalyzer.API.Services;

public interface IJwtTokenService
{
    (string token, DateTime expiresAtUtc) CreateToken(IdentityUser user);
}
