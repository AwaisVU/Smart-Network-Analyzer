using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SmartNetworkAnalyzer.API.Data;
using SmartNetworkAnalyzer.API.Hubs;
using SmartNetworkAnalyzer.API.Services;

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// 🔹 SECTION 1: SERVICES (Dependency Injection Container)
// =======================================================
// Register all dependencies and framework services here.
// This is where you tell ASP.NET what your app "has".

// Enable controller support (required for APIs)
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddIdentityCore<IdentityUser>(options => {options.User.RequireUniqueEmail = true;}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSignalR();


// Register your custom services
// Scoped = one instance per HTTP request
builder.Services.AddScoped<IPingService, PingService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

// OpenAPI / Swagger (for API documentation/testing)
builder.Services.AddOpenApi();


// =======================================================
// 🔹 SECTION 2: BUILD APPLICATION
// =======================================================
// This compiles everything above into a runnable app.
// After this point, you CANNOT add more services.

var app = builder.Build();


// =======================================================
// 🔹 SECTION 3: MIDDLEWARE PIPELINE (Request Flow)
// =======================================================
// Middleware are executed in order (top → bottom).
// Each middleware can inspect/modify the request/response.

// Only enable OpenAPI in development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// =======================================================
// 🔹 SECTION 4: ENDPOINT MAPPING
// =======================================================
// This connects incoming HTTP requests to controllers.

app.MapHub<DiagnosticsHub>("/hubs/diagnostics");
app.MapControllers();


// =======================================================
// 🔹 SECTION 5: RUN APPLICATION
// =======================================================
// Starts the web server and begins listening for requests.

app.Run();