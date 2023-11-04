using ApiBestPracticesExample.Infrastructure;
using ApiBestPracticesExample.Infrastructure.Database;
using ApiBestPracticesExample.Infrastructure.Endpoints.Users.V1;
using ApiBestPracticesExample.Presentation;
using System.Reflection;
using ApiBestPracticesExample.Infrastructure.Validators;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddDefaultServices(builder.Configuration, new List<Assembly>
{
	typeof(CreateUserEndpointV1).Assembly
}, new List<Assembly>
{
	typeof(UserCreateDtoValidator).Assembly
}, 
	Enum.GetValues<ApiSupportedVersions>().Select(version => (int)version).ToList());
var conStr = builder.Configuration.GetRequiredConnectionString("SqlConnection");
builder.Services.AddCustomDbContext<AppDbContext>(conStr, builder.Environment.IsDevelopment());
var app = builder.Build();
app.UseDefaultServices();
await app.Services.PerformDbPreparationAsync();
await app.RunAsync();
