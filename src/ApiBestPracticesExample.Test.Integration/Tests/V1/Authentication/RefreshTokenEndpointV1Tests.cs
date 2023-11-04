using ApiBestPracticesExample.Infrastructure.Endpoints.Authentication.V1;

namespace ApiBestPracticesExample.Test.Integration.Tests.V1.Authentication;

public sealed class RefreshTokenEndpointV1Tests : BaseTest
{
	[Fact]
	public async Task RefreshSuccess()
	{
		//Arrange
		var (loginRsp, loginRes) = await Anonymous.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(new()
		{
			UserId = "ReadOnlyAdmin@gmail.com",
			Password = "Qwerty123$"
		});
		loginRsp.IsSuccessStatusCode.Should().BeTrue();
		//Act
		var (rsp, res) = await Anonymous.POSTAsync<RefreshTokenEndpointV1, TokenRequest, TokenResponse>(new()
		{
			UserId = loginRes.UserId,
			RefreshToken = loginRes.RefreshToken
		});
		//Assert
		rsp.IsSuccessStatusCode.Should().BeTrue();
		var dbUser = await DbContext.Users.SingleAsync(u => u.Email == res.UserId);
		res.RefreshToken.Should().Be(dbUser.RefreshToken);

		var claims = ParseClaimsFromJwt(res.AccessToken);
		claims.Should().Contain(c => c.Type == "role" && c.Value == dbUser.RoleName);
		claims.Should().Contain(c => c.Type == "email" && c.Value == dbUser.Email);
	}


	public RefreshTokenEndpointV1Tests(EndpointDockerFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
	{
	}
}