using ApiBestPracticesExample.Contracts.Dtos.User;
using ApiBestPracticesExample.Infrastructure.Database;
using ApiBestPracticesExample.Infrastructure.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiBestPracticesExample.Infrastructure.Endpoints.Users.V1;

public sealed class CreateUserEndpointV1 : Endpoint<UserCreateDto, UserDto>
{
	private readonly AppDbContext _context;
	public override void Configure()
	{
		Post("users/create");
		Roles(Contracts.SupportedRoles.Admin);
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
			ThrowError(r=>r.Email,"User with this email already exists");
		var user = req.MapUserCreateDtoToUser();
		user.RoleName = Contracts.SupportedRoles.User;
		await _context.AddAsync(user, ct);
		await _context.SaveChangesAsync(ct);
		Response = user.MapUserToUserDto();
	}
}