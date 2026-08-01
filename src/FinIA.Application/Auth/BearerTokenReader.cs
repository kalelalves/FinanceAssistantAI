namespace FinIA.Application.Auth;

public sealed class BearerTokenReader : IBearerTokenReader
{
    private const string Prefix = "Bearer ";

    public string? Read(string? authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return null;
        }

        return authorizationHeader.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase)
            ? authorizationHeader[Prefix.Length..].Trim()
            : null;
    }
}
