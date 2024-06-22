using ApiBestPracticesExample.Presentation.Endpoints.OnBoarding.V1;

namespace ApiBestPracticesExample.Test.Integration.Tests.V1.Onboarding;

[Collection("TestCollection")]
public sealed class EmailExistenceEndpointV1Tests : BaseTest
{
    public EmailExistenceEndpointV1Tests(ApiFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Returns_200True_When_UserWithEmailExists()
    {
        //Arrange
        //Act
        var (rsp, res) =
            await Anonymous.POSTAsync<EmailExistenceEndpointV1, string, bool>(AppDbContextSeeder.DefaultUser.Email);

        //Assert
        rsp.StatusCode.Should().Be(HttpStatusCode.OK);
        res.Should().BeTrue();
    }

    [Fact]
    public async Task Returns_200False_When_UserWithEmailDoesNotExists()
    {
        //Arrange
        //Act
        var (rsp, res) =
            await Anonymous.POSTAsync<EmailExistenceEndpointV1, string, bool>(Fixture.Fake.Internet.Email());

        //Assert
        rsp.StatusCode.Should().Be(HttpStatusCode.OK);
        res.Should().BeFalse();
    }
}