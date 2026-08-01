namespace FinIA.Application.Auth;

public sealed record AuthResult(
    bool IsAuthenticated,
    AuthenticatedUser? User,
    string? ErrorCode,
    string? ErrorMessage)
{
    public static AuthResult Success(AuthenticatedUser user)
        => new(true, user, null, null);

    public static AuthResult Failure(string code, string message)
        => new(false, null, code, message);
}
