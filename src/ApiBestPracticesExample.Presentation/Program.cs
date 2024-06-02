using ApiBestPracticesExample.Presentation;
using ApiBestPracticesExample.Presentation.Endpoints.Authentication.V1;
using FastEndpoints.Swagger;
using Serilog;

var apiVersions = Enum.GetValues<ApiSupportedVersions>().Select(version => (int)version).ToList();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultServices(builder.Configuration, [typeof(LoginEndpointV1).Assembly], apiVersions);

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

await app.Services.PrepareDbAsync();

await app.RunAsync();