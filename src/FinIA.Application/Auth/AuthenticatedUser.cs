namespace FinIA.Application.Auth;

public sealed record AuthenticatedUser(
    Guid UserId,
    string? Email);
