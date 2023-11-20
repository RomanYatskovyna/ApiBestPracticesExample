﻿using ApiBestPracticesExample.Infrastructure.Endpoints.OnBoarding.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApiBestPracticesExample.Test.Integration.Tests.V1.Onboarding;
public sealed class UserNameExistenceEndpointV1Tests:BaseTest
{
	[Fact]
	public async Task Returns_200True_When_UserWithUserNameExists()
	{
		//Arrange
		//Act
		var (rsp, res) = await Admin.POSTAsync<UserNameExistenceEndpointV1, string, bool>(AppDbContextSeeder.DefaultUser.UserName);
		//Assert
		rsp.StatusCode.Should().Be(HttpStatusCode.OK);
		res.Should().BeTrue();
	}
	[Fact]
	public async Task Returns_200False_When_UserWithUserNameDoesNotExists()
	{
		//Arrange
		//Act
		var (rsp, res) = await Admin.POSTAsync<UserNameExistenceEndpointV1, string, bool>(Fixture.Fake.Internet.UserName());
		//Assert
		rsp.StatusCode.Should().Be(HttpStatusCode.OK);
		res.Should().BeFalse();
	}
	public UserNameExistenceEndpointV1Tests(EndpointDockerFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
	{
	}
}
