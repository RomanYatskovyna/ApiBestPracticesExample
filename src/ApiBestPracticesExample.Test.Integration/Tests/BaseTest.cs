using ApiBestPracticesExample.Presentation.Endpoints.V1.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace ApiBestPracticesExample.Test.Integration.Tests;

[Collection("TestCollection")]
public abstract class BaseTest : IAsyncLifetime
{
    protected readonly AppDbContext DbContext;
    protected readonly TestFixture Fixture;

    protected BaseTest(TestFixture fixture)
    {
        Fixture = fixture;
        DbContext = Fixture.Services.GetRequiredService<AppDbContext>();
    }

    protected HttpClient Admin { get; private set; } = null!;
    protected HttpClient Client { get; private set; } = null!;
    protected HttpClient Anonymous { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await Fixture.ResetDatabaseAsync();

        await Fixture.InitDatabaseAsync();
        Anonymous = Fixture.Client;

        await SetupDefaultAdminAsync();
        await SetupDefaultClientAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
    }

    protected List<Claim> GetClaimsFromJwt(string accessToken)
    {
        var token = GetTokenFromJwt(accessToken);

        return token.Claims.ToList();
    }

    protected JwtSecurityToken GetTokenFromJwt(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(accessToken))
        {
            throw new ArgumentException("The token doesn't seem to be in a proper JWT format.");
        }

        var token = handler.ReadJwtToken(accessToken);
        return token;
    }

    private async Task SetupDefaultAdminAsync()
    {
        var (rsp, res) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(new LoginRequest
        {
            Email = AppDbContextSeeder.DefaultAdmin.Email, Password = AppDbContextSeeder.DefaultAdminPassword,
        });
        rsp.IsSuccessStatusCode.Should().BeTrue();

        Admin = Fixture.CreateClient(c =>
            c.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, res.AccessToken));
    }

    private async Task SetupDefaultClientAsync()
    {
        var (rsp, res) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(new LoginRequest
        {
            Email = AppDbContextSeeder.DefaultUser.Email, Password = AppDbContextSeeder.DefaultUserPassword,
        });
        rsp.IsSuccessStatusCode.Should().BeTrue();

        Client = Fixture.CreateClient(c =>
            c.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, res.AccessToken));
    }
}