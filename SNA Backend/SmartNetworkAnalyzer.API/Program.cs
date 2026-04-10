using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using SmartNetworkAnalyzer.API.Data;
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

// Register your custom services
// Scoped = one instance per HTTP request
builder.Services.AddScoped<IPingService, PingService>();

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


// =======================================================
// 🔹 SECTION 4: ENDPOINT MAPPING
// =======================================================
// This connects incoming HTTP requests to controllers.

app.MapControllers();


// =======================================================
// 🔹 SECTION 5: RUN APPLICATION
// =======================================================
// Starts the web server and begins listening for requests.

app.Run();