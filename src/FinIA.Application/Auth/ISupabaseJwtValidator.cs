namespace FinIA.Application.Auth;

public interface ISupabaseJwtValidator
{
    AuthResult Validate(string? bearerToken);
}
