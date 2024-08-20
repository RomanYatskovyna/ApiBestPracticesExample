using ApiBestPracticesExample.Presentation.Endpoints.V1.OnBoarding;

namespace ApiBestPracticesExample.Test.Integration.Tests.V1.Onboarding;

public sealed class PhoneNumberExistenceEndpointV1Tests : BaseTest
{
    public PhoneNumberExistenceEndpointV1Tests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Returns_200True_When_UserWithEmailExists()
    {
        //Arrange
        //Act
        var (rsp, res) =
            await Anonymous.POSTAsync<PhoneNumberExistenceEndpointV1, string, bool>(AppDbContextSeeder.DefaultUser
                .PhoneNumber!);

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
            await Anonymous.POSTAsync<PhoneNumberExistenceEndpointV1, string, bool>(
                Fixture.Fake.Phone.PhoneNumber("(###) ###-####"));

        //Assert
        rsp.StatusCode.Should().Be(HttpStatusCode.OK);
        res.Should().BeFalse();
    }
}