using ApiBestPracticesExample.Infrastructure;
using ApiBestPracticesExample.Infrastructure.Database;
using ApiBestPracticesExample.Infrastructure.Endpoints.Users.v1;
using ApiBestPracticesExample.Presentation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDefaultServices(builder.Configuration, new List<Assembly>
{
	typeof(CreateUserEndpointV1).Assembly
}, Enum.GetValues<ApiSupportedVersions>().Select(version => (int)version).ToList());
builder.Services.AddCustomDbContext<AppDbContext>(builder.Configuration.GetRequiredConnectionString("DefaultConnection"));
var app = builder.Build();
app.UseDefaultServices();

// REVIEW: This is done for development ease but shouldn't be here in production
await using (var scope = app.Services.CreateAsyncScope())
{
	var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	var logger = app.Services.GetRequiredService<ILogger>();
	await context.Database.MigrateAsync();
	await context.SeedAsync(app.Environment, logger);
}

await app.RunAsync();
