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
builder.Services.AddCustomDbContext<AppDbContext>(builder.Configuration.GetRequiredConnectionString("DefaultConnection"));
var app = builder.Build();
app.UseDefaultServices();
//await app.Services.MigrateDbContextAsync<AppDbContext>(async ( services) =>
//{
//	await using var scope= services.CreateAsyncScope();
//	var context=scope.ServiceProvider.GetRequiredService<AppDbContext>();
//	var logger=scope.ServiceProvider.GetRequiredService<ILogger>();
//	await context.SeedDataAsync(logger);
//});
await app.PerformDbPreparationAsync();
await app.RunAsync();