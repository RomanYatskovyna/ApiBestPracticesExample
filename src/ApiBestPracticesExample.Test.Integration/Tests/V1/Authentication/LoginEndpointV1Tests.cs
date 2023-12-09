using System.Security.Claims;
using ApiBestPracticesExample.Infrastructure.Endpoints.Authentication.V1;

namespace ApiBestPracticesExample.Test.Integration.Tests.V1.Authentication;
[Collection("DockerCollection")]
public sealed class LoginEndpointV1Tests : BaseTest
{
	[Fact]
	public async Task Returns_200Token_When_AdminExists()
	{
		//Arrange
		var request = new LoginRequest
		{
			UserId = AppDbContextSeeder.DefaultAdmin.Email,
			Password = AppDbContextSeeder.DefaultAdminPassword
		};
		//Act
		var (rsp, res) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(request);
		//Assert
		rsp.StatusCode.Should().Be(HttpStatusCode.OK);
		var dbUser = await DbContext.Users.SingleAsync(u => u.Email == res.UserId);
		res.RefreshToken.Should().Be(dbUser.RefreshToken);

		var claims = ParseClaimsFromJwt(res.AccessToken);
		claims.Should().Contain(c => c.Type=="role" && c.Value == dbUser.RoleName);
		claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == dbUser.Email);
	}
	[Fact]
	public async Task Returns_200Token_When_UserExists()
	{
		//Arrange
		var request = new LoginRequest
		{
			UserId = AppDbContextSeeder.DefaultUser.Email,
			Password = AppDbContextSeeder.DefaultUserPassword
		};
		//Act
		var (rsp, res) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(request);
		//Assert
		rsp.StatusCode.Should().Be(HttpStatusCode.OK);
		var dbUser = await DbContext.Users.SingleAsync(u => u.Email == res.UserId);
		res.RefreshToken.Should().Be(dbUser.RefreshToken);

		var claims = ParseClaimsFromJwt(res.AccessToken);
		claims.Should().Contain(c => c.Type == "role" && c.Value == dbUser.RoleName);
		claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == dbUser.Email);
	}

	[Fact]
	public async Task Returns_404_When_UserIdAbsent()
	{
		//Arrange
		var request = new LoginRequest
		{
			UserId = "WrongEmail",
			Password = AppDbContextSeeder.DefaultAdminPassword
		};
		//Act
		var (rsp, res) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, string>(request);
		//Assert
		rsp.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}
	[Fact]
	public async Task Returns_404_When_PasswordIsWrong()
	{
		//Arrange
		var request = new LoginRequest
		{
			UserId = AppDbContextSeeder.DefaultAdmin.Email,
			Password = "WrongPassword"
		};
		//Act
		var (rsp, res) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(request);
		//Assert
		rsp.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}
	public LoginEndpointV1Tests(EndpointDockerFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
	{
	}
}