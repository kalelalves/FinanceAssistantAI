namespace FinIA.Application.Health;

public sealed record HealthResponse(
    string Status,
    string Service,
    string Runtime,
    DateTimeOffset CheckedAtUtc,
    IReadOnlyCollection<string> MissingSettings);
