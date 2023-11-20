using ApiBestPracticesExample.Infrastructure.Endpoints.OnBoarding.V1;
using FastEndpoints.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApiBestPracticesExample.Test.Integration.Tests.V1.Onboarding;
public sealed class PhoneNumberExistenceEndpointV1Tests:BaseTest
{
	[Fact]
	public async Task Returns_200True_When_UserWithEmailExists()
	{
		//Arrange
		//Act
		var (rsp, res) = await Admin.POSTAsync<PhoneNumberExistenceEndpointV1, string, bool>(AppDbContextSeeder.DefaultUser.PhoneNumber!);

		//Assert
		rsp.StatusCode.Should().Be(HttpStatusCode.OK);
		res.Should().BeTrue();
	}
	[Fact]
	public async Task Returns_200False_When_UserWithEmailDoesNotExists()
	{
		//Arrange
		//Act
		var (rsp, res) = await Admin.POSTAsync<PhoneNumberExistenceEndpointV1, string, bool>(Fixture.Fake.Phone.PhoneNumber("(###) ###-####"));

		//Assert
		rsp.StatusCode.Should().Be(HttpStatusCode.OK);
		res.Should().BeFalse();
	}

	public PhoneNumberExistenceEndpointV1Tests(EndpointDockerFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
	{
	}
}
