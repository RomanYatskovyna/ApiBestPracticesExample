using ApiBestPracticesExample.Infrastructure;
using ApiBestPracticesExample.Infrastructure.Database;
using ApiBestPracticesExample.Infrastructure.Endpoints.Authentication.V1;
using ApiBestPracticesExample.Presentation;
using FastEndpoints;
using FastEndpoints.Swagger;
using Serilog;
using System.Reflection;
var apiVersions= Enum.GetValues<ApiSupportedVersions>().Select(version => (int)version).ToList();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddDefaultServices(builder.Configuration, new List<Assembly>
{
	typeof(LoginEndpointV1).Assembly
}, apiVersions);
var conStr = builder.Configuration.GetRequiredConnectionString("SqlConnection");
builder.Services.AddCustomDbContextPool<AppDbContext>(conStr, builder.Environment.IsDevelopment());
var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultExceptionHandler();
app.UseDefaultFastEndpoints();
app.UseClientGen(apiVersions);
app.UseSwaggerGen();
await app.Services.PrepareDbAsync(true, true, !app.Environment.IsProduction());
await app.RunAsync();
