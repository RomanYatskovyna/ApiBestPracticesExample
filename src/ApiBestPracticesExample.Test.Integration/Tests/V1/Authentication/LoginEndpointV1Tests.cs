using ApiBestPracticesExample.Presentation.Endpoints.Authentication.V1;
using System.Security.Claims;

namespace ApiBestPracticesExample.Test.Integration.Tests.V1.Authentication;

[Collection("TestCollection")]
public sealed class LoginEndpointV1Tests : BaseTest
{
    public LoginEndpointV1Tests(ApiFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Returns_200Token_When_AdminExists()
    {
        //Arrange
        var request = new LoginRequest
        {
            Email = AppDbContextSeeder.DefaultAdmin.Email, Password = AppDbContextSeeder.DefaultAdminPassword,
        };
        //Act
        var (rsp, res) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(request);
        //Assert
        rsp.StatusCode.Should().Be(HttpStatusCode.OK);

        var dbUser = await DbContext.Users
            .Include(user => user.RefreshToken)
            .SingleAsync(u => u.Email == res.UserId);

        res.RefreshToken.Should().Be(dbUser.RefreshToken!.Token);

        var claims = GetClaimsFromJwt(res.AccessToken);
        claims.Should().Contain(c => c.Type == "role" && c.Value == dbUser.RoleName);
        claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == dbUser.Email);
    }

    [Fact]
    public async Task Returns_200Token_When_UserExists()
    {
        //Arrange
        var request = new LoginRequest
        {
            Email = AppDbContextSeeder.DefaultUser.Email, Password = AppDbContextSeeder.DefaultUserPassword,
        };
        //Act
        var (rsp, res) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(request);
        //Assert
        rsp.StatusCode.Should().Be(HttpStatusCode.OK);

        var dbUser = await DbContext.Users
            .Include(user => user.RefreshToken)
            .SingleAsync(u => u.Email == res.UserId);
        res.RefreshToken.Should().Be(dbUser.RefreshToken!.Token);

        var claims = GetClaimsFromJwt(res.AccessToken);
        claims.Should().Contain(c => c.Type == "role" && c.Value == dbUser.RoleName);
        claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == dbUser.Email);
    }

    [Fact]
    public async Task Returns_404_When_UserIdAbsent()
    {
        //Arrange
        var request = new LoginRequest { Email = "WrongEmail", Password = AppDbContextSeeder.DefaultAdminPassword };
        //Act
        var (rsp, _) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, string>(request);
        //Assert
        rsp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Returns_404_When_PasswordIsWrong()
    {
        //Arrange
        var request = new LoginRequest { Email = AppDbContextSeeder.DefaultAdmin.Email, Password = "WrongPassword" };
        //Act
        var (rsp, _) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(request);
        //Assert
        rsp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}