using ApiBestPracticesExample.Contracts.Dtos.User;
using ApiBestPracticesExample.Infrastructure.Database;
using ApiBestPracticesExample.Infrastructure.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiBestPracticesExample.Infrastructure.Endpoints.Users.v1;

public sealed class CreateUserEndpointV1 : Endpoint<UserCreateDto, UserDto>
{
	private readonly AppDbContext _context;

	public override void Configure()
	{
		Post("users/create");
		//Roles("Bom");
		Description(d => { d.WithDisplayName("CreateUser"); });
		Summary(s =>
		{
			s.Summary = "short summary goes here";
			s.Description = "long description goes here";
		});
		Version((int)ApiSupportedVersions.V1);
	}

	public CreateUserEndpointV1(AppDbContext context)
	{
		_context = context;
	}
	public override async Task HandleAsync(UserCreateDto req, CancellationToken ct)
	{
		if (await _context.Users.AnyAsync(u => u.Email == req.Email, ct))
			ThrowError($"User with email {req.Email} already exists", 400);
		var user = req.MapUserCreateDtoToUser();
		user.RoleName = "Bom";
		await _context.AddAsync(user, ct);
		await _context.SaveChangesAsync(ct);
		TypedResults.Ok(user.MapUserToUserDto());
	}
	

	//public override Task<UserDto> ExecuteAsync(UserCreateDto req, CancellationToken ct)
	//{
	//	return base.ExecuteAsync(req, ct);
	//}
}