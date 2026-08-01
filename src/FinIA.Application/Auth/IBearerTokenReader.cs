namespace FinIA.Application.Auth;

public interface IBearerTokenReader
{
    string? Read(string? authorizationHeader);
}
