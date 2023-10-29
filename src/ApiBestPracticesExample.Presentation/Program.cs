using ApiBestPracticesExample.Infrastructure;
using ApiBestPracticesExample.Infrastructure.Database;
using ApiBestPracticesExample.Infrastructure.Endpoints.Users.v1;
using ApiBestPracticesExample.Presentation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDefaultServices(builder.Configuration, new List<Assembly>
{
	typeof(CreateUserEndpointV1).Assembly
}, Enum.GetValues<ApiSupportedVersions>().Select(version => (int)version).ToList());
var conStr = builder.Configuration.GetRequiredConnectionString("SqlConnection");
builder.Services.AddCustomDbContext<AppDbContext>(conStr);
var app = builder.Build();
app.UseDefaultServices();
await app.PerformDbPreparationAsync();
await app.RunAsync();