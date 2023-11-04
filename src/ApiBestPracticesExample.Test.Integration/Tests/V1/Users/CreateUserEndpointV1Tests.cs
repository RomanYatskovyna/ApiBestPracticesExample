using System.Net;
using ApiBestPracticesExample.Contracts.Dtos.User;
using ApiBestPracticesExample.Infrastructure.Endpoints.Users.V1;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

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
	[Fact]
	public async Task CreateUserShouldFailValidation()
	{
		//Arrange
		var request = new UserCreateDto
		{
			Email = "NotEmail",
			Password = "w"
		};
		//Act
		var (rsp, res) = await Admin.POSTAsync<CreateUserEndpointV1, UserCreateDto, ValidationProblemDetails>(request);
		//Assert
		rsp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		res.Errors.Should().ContainKeys("email", "password");
	}
	[Fact]
	public async Task CreateUserShouldFailValidationBecauseOfAlreadyExistingUser()
	{
		//Arrange
		var request = new UserCreateDto
		{
			Email = AppDbContextSeeder.DefaultUser.Email,
			Password = "Qwerty123$"
		};
		//Act
		var (rsp, res) = await Admin.POSTAsync<CreateUserEndpointV1, UserCreateDto, ValidationProblemDetails>(request);
		//Assert
		rsp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		res.Errors.Should().ContainKey("email").WhoseValue.Should().Contain("User with this email already exists");
	}
	public CreateUserEndpointV1Tests(EndpointDockerFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
	{
	}
}
