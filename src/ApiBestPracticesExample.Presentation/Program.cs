using ApiBestPracticesExample.Infrastructure;
using ApiBestPracticesExample.Infrastructure.Database;
using ApiBestPracticesExample.Infrastructure.Endpoints.Users.V1;
using ApiBestPracticesExample.Presentation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddDefaultServices(builder.Configuration, new List<Assembly>
{
	typeof(CreateUserEndpointV1).Assembly
}, Enum.GetValues<ApiSupportedVersions>().Select(version => (int)version).ToList());
var conStr = builder.Configuration.GetRequiredConnectionString("SqlConnection");
builder.Services.AddCustomDbContext<AppDbContext>(conStr);
var app = builder.Build();
app.UseDefaultServices();
if (app.Configuration.GetRequiredValue<bool>("InitDefaultData"))
{
	await app.PerformDbPreparationAsync();
}

await app.RunAsync();
