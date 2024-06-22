using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiBestPracticesExample.Infrastructure.Settings;
public sealed record JwtOptions
{
    [Range(0, int.MaxValue, ErrorMessage = "AccessTokenExpirationInMinutes must be greater than 0")]
    public int AccessTokenExpirationInMinutes { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "RefreshTokenExpirationInHours must be greater than 0")]
    public int RefreshTokenExpirationInHours { get; set; }
    [MinLength(128, ErrorMessage = "AccessTokenSigningKey length must be greater than 128 symbols")]
    public string AccessTokenSigningKey { get; set; } = null!;
}
