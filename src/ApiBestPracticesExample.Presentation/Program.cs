using ApiBestPracticesExample.Presentation;
using ApiBestPracticesExample.Presentation.Endpoints.Authentication.V1;
using FastEndpoints.Swagger;

var apiVersions = Enum.GetValues<ApiSupportedVersions>().Select(version => (int)version).ToList();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultServices(builder.Configuration, apiVersions, typeof(LoginEndpointV1).Assembly);

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

await app.PrepareDbAsync();

await app.RunAsync();
