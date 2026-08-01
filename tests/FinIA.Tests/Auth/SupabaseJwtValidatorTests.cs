using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinIA.Application.Auth;
using FinIA.Application.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinIA.Tests.Auth;

public sealed class SupabaseJwtValidatorTests
{
    [Fact]
    public void Validate_ShouldAcceptValidSupabaseStyleToken()
    {
        const string secret = "super-secret-key-with-enough-length";
        var userId = Guid.NewGuid();
        var validator = new SupabaseJwtValidator(new FinIaOptions { SupabaseJwtSecret = secret });
        var token = CreateToken(secret, userId);

        var result = validator.Validate(token);

        Assert.True(result.IsAuthenticated);
        Assert.Equal(userId, result.User?.UserId);
    }

    [Fact]
    public void Validate_ShouldRejectMissingToken()
    {
        var validator = new SupabaseJwtValidator(new FinIaOptions { SupabaseJwtSecret = "secret" });

        var result = validator.Validate(null);

        Assert.False(result.IsAuthenticated);
        Assert.Equal("auth.missing_token", result.ErrorCode);
    }

    private static string CreateToken(string secret, Guid userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: [new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())],
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
