using ApiBestPracticesExample.Infrastructure.Endpoints.Authentication.V1;

namespace ApiBestPracticesExample.Test.Integration.Tests.V1.Authentication;
public sealed class LoginEndpointV1Tests : BaseTest
{
    [Fact]
    public async Task LoginSuccessTests()
    {
        //Arrange

        //Act
        var (rsp, res) = await Fixture.Client.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(new()
        {
            UserId = "ReadOnlyAdmin@gmail.com",
            Password = "Qwerty123$"
        });

        //Assert
        rsp.IsSuccessStatusCode.Should().BeTrue();
        var dbUser = await DbContext.Users.SingleAsync(u => u.Email == res.UserId);
        res.RefreshToken.Should().Be(dbUser.RefreshToken);
    }


    public LoginEndpointV1Tests(EndpointDockerFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }
}