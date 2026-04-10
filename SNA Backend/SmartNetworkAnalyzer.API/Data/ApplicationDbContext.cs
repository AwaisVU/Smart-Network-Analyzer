using System;
using Microsoft.EntityFrameworkCore;

namespace SmartNetworkAnalyzer.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
    {
        
    }
}
