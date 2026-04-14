using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartNetworkAnalyzer.API.Entities;

namespace SmartNetworkAnalyzer.API.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
    {
        
    }

    public DbSet<DiagnosticSession> DiagnosticSessions {get; set;}
    public DbSet<ProbeResult> ProbeResults { get; set; }

}
