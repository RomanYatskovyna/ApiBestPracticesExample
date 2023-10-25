using ApiBestPracticesExample.Infrastructure.Endpoints.Authentication.V1;
using ApiBestPracticesExample.Presentation;

namespace ApiBestPracticesExample.Test.Integration.Tests.Endpoints;

public sealed class LoginContextTests : BaseContextTest
{
    public LoginContextTests(EndpointDockerFixture f, ITestOutputHelper o) : base(f, o)
    {
    }

    [Fact]
    public async Task LoginSuccessTests()
    {
        //Arrange

        //Act
        var (rsp, res) = await Fixture.Client.POSTAsync<LoginEndpointV1, LoginRequest, TokenResponse>(new()
        {
            Username = "admin@gmail.com",
            Password = "Qwerty123$"
        });

        //Assert
        rsp.IsSuccessStatusCode.Should().BeTrue();
        var dbToken = DbContext.Users.Single(u => u.Email == res.UserId).RefreshToken;
        res.RefreshToken.Should().Be(dbToken);
    }


}