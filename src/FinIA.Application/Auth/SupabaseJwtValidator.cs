using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinIA.Application.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinIA.Application.Auth;

public sealed class SupabaseJwtValidator(FinIaOptions options) : ISupabaseJwtValidator
{
    public AuthResult Validate(string? bearerToken)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            return AuthResult.Failure("auth.missing_token", "Bearer token is required.");
        }

        if (string.IsNullOrWhiteSpace(options.SupabaseJwtSecret))
        {
            return AuthResult.Failure("auth.missing_configuration", "Supabase JWT validation is not configured.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SupabaseJwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(bearerToken, validationParameters, out _);
            var userIdValue = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value
                ?? principal.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return AuthResult.Failure("auth.invalid_subject", "JWT subject is not a valid user id.");
            }

            var email = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value
                ?? principal.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;

            return AuthResult.Success(new AuthenticatedUser(userId, email));
        }
        catch (SecurityTokenException)
        {
            return AuthResult.Failure("auth.invalid_token", "Bearer token is invalid.");
        }
        catch (ArgumentException)
        {
            return AuthResult.Failure("auth.invalid_token", "Bearer token is invalid.");
        }
    }
}
