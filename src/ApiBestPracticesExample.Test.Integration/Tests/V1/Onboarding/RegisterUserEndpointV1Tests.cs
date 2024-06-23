using ApiBestPracticesExample.Contracts.Dtos.Onboarding;
using ApiBestPracticesExample.Presentation.Endpoints.OnBoarding.V1;

namespace ApiBestPracticesExample.Test.Integration.Tests.V1.Onboarding;

[Collection("TestCollection")]
public sealed class RegisterUserEndpointV1Tests : BaseTest
{
    public RegisterUserEndpointV1Tests(ApiFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Returns_200CreatedUser_When_Expected()
    {
        //Arrange
        var request = new UserCreateDto
        {
            PhoneNumber = Fixture.Fake.Phone.PhoneNumber("+1 ###-###-####"),
            Email = Fixture.Fake.Internet.Email(),
            Password = "Qwerty123$",
        };

        //Act
        var (rsp, res) = await Admin.POSTAsync<RegisterUserEndpointV1, UserCreateDto, UserDto>(request);

        //Assert
        await rsp.Content.ReadAsStringAsync();

        rsp.StatusCode.Should().Be(HttpStatusCode.OK);
        res.Email.Should().Be(res.Email);
    }

    [Fact]
    public async Task Returns_400ProblemDetails_When_DataIsInvalid()
    {
        //Arrange
        var request = new UserCreateDto { PhoneNumber = "1", Email = "WrongEmail", Password = "w" };

        //Act
        var (rsp, res) =
            await Anonymous.POSTAsync<RegisterUserEndpointV1, UserCreateDto, ValidationProblemDetails>(request);

        //Assert
        rsp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        res.Errors.Should().ContainKeys("email", "password", "phoneNumber");
    }

    [Fact]
    public async Task Returns_400ProblemDetails_When_UserWithProvidedUserNameOrEmailOrPhoneNumberAlreadyExists()
    {
        //Arrange
        var request = new UserCreateDto
        {
            PhoneNumber = AppDbContextSeeder.DefaultUser.PhoneNumber,
            Email = AppDbContextSeeder.DefaultUser.Email,
            Password = AppDbContextSeeder.DefaultUserPassword,
        };

        //Act
        var (rsp, res) =
            await Anonymous.POSTAsync<RegisterUserEndpointV1, UserCreateDto, ValidationProblemDetails>(request);

        //Assert
        rsp.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using var scope = new AssertionScope();
        res.Errors.Should().ContainKey("email").WhoseValue.Should().Contain("User with this email already exists");
        res.Errors.Should().ContainKey("phoneNumber").WhoseValue.Should()
            .Contain("User with this phoneNumber already exists");
    }
}