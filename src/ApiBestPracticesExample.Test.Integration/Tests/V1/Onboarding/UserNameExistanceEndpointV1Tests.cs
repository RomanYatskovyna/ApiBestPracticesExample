using ApiBestPracticesExample.Presentation.Endpoints.OnBoarding.V1;

namespace ApiBestPracticesExample.Test.Integration.Tests.V1.Onboarding;

[Collection("DockerCollection")]
public sealed class UserNameExistenceEndpointV1Tests : BaseTest
{
    public UserNameExistenceEndpointV1Tests(ApiFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Returns_200True_When_UserWithUserNameExists()
    {
        //Arrange
        //Act
        var (rsp, res) =
            await Anonymous.POSTAsync<UserNameExistenceEndpointV1, string, bool>(
                AppDbContextSeeder.DefaultUser.UserName);
        //Assert
        rsp.StatusCode.Should().Be(HttpStatusCode.OK);
        res.Should().BeTrue();
    }

    [Fact]
    public async Task Returns_200False_When_UserWithUserNameDoesNotExists()
    {
        //Arrange
        //Act
        var (rsp, res) =
            await Anonymous.POSTAsync<UserNameExistenceEndpointV1, string, bool>(Fixture.Fake.Internet.UserName());
        //Assert
        rsp.StatusCode.Should().Be(HttpStatusCode.OK);
        res.Should().BeFalse();
    }
}