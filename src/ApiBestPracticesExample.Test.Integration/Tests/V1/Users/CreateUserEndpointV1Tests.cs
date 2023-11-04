using ApiBestPracticesExample.Contracts.Dtos.User;
using ApiBestPracticesExample.Infrastructure.Endpoints.Users.V1;

namespace ApiBestPracticesExample.Test.Integration.Tests.V1.Users;
public sealed class CreateUserEndpointV1Tests : BaseTest
{
	[Fact]
	public async Task CreateUserShouldBeSuccessful()
	{
		//Arrange
		var request = new UserCreateDto
		{
			Email = "TestUser@gmail.com",
			Password = "Qwerty123$"
		};
		//Act
		var (rsp, res) = await Admin.POSTAsync<CreateUserEndpointV1, UserCreateDto, UserDto>(request);
		//Assert
		rsp.IsSuccessStatusCode.Should().BeTrue();
		res.Email.Should().Be(res.Email);
	}
	public CreateUserEndpointV1Tests(EndpointDockerFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
	{
	}
}
