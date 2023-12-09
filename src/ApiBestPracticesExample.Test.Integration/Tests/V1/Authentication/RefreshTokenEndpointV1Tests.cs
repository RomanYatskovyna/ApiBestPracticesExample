using System.Security.Claims;
using ApiBestPracticesExample.Infrastructure.Endpoints.Authentication.V1;

namespace ApiBestPracticesExample.Test.Integration.Tests.V1.Authentication;
[Collection("DockerCollection")]
public sealed class RefreshTokenEndpointV1Tests : BaseTest
{
	[Fact]
	public async Task Returns_200UpdatedCredentials_When_Expected()
	{
		//Arrange
		var (loginRsp, loginRes) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(new()
		{
			UserId = AppDbContextSeeder.DefaultAdmin.Email,
			Password = AppDbContextSeeder.DefaultAdminPassword
		});
		loginRsp.IsSuccessStatusCode.Should().BeTrue();
		//Act
		var (rsp, res) = await Anonymous.POSTAsync<RefreshTokenEndpointV1, TokenRequest, TokenResponse>(new()
		{
			UserId = loginRes.UserId,
			RefreshToken = loginRes.RefreshToken
		});
		//Assert
		rsp.StatusCode.Should().Be(HttpStatusCode.OK);
		var dbUser = await DbContext.Users.SingleAsync(u => u.Email == res.UserId);
		res.RefreshToken.Should().Be(dbUser.RefreshToken);

		var claims = ParseClaimsFromJwt(res.AccessToken);
		claims.Should().Contain(c => c.Type == "role" && c.Value == dbUser.RoleName);
		claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == dbUser.Email);
	}
	[Fact]
	public async Task Returns_404UserNotFound_When_UserIdIsMissing()
	{
		//Arrange
		var (loginRsp, loginRes) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(new()
		{
			UserId = AppDbContextSeeder.DefaultAdmin.Email,
			Password = AppDbContextSeeder.DefaultAdminPassword
		});
		loginRsp.IsSuccessStatusCode.Should().BeTrue();
		//Act
		var (rsp, res) = await Anonymous.POSTAsync<RefreshTokenEndpointV1, TokenRequest, ValidationProblemDetails>(new()
		{
			UserId = "WrongUserId",
			RefreshToken = loginRes.RefreshToken
		});
		//Assert
		rsp.StatusCode.Should().Be(HttpStatusCode.NotFound);
		res.Errors.Should().ContainKey("userId");
	}
	[Fact]
	public async Task Returns_400ProblemDetails_When_TokenIsWrong()
	{
		//Arrange
		var (loginRsp, loginRes) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(new()
		{
			UserId = AppDbContextSeeder.DefaultAdmin.Email,
			Password = AppDbContextSeeder.DefaultAdminPassword
		});
		loginRsp.IsSuccessStatusCode.Should().BeTrue();
		//Act
		var (rsp, res) = await Anonymous.POSTAsync<RefreshTokenEndpointV1, TokenRequest, ValidationProblemDetails>(new()
		{
			UserId = loginRes.UserId,
			RefreshToken = "WrongToken"
		});
		//Assert
		rsp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		res.Errors.Should().ContainKey("refreshToken");
	}


	public RefreshTokenEndpointV1Tests(ApiFixture fixture) : base(fixture)
	{
	}
}