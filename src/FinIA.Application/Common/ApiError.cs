namespace FinIA.Application.Common;

public sealed record ApiError(
    string Code,
    string Message,
    string? TraceId = null);
