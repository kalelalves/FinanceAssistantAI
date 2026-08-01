using System.Security.Cryptography;
using System.Text;
using FinIA.Application.Configuration;

namespace FinIA.Application.Security;

public sealed class HmacSha256UserAnonymizer(FinIaOptions options) : IUserAnonymizer
{
    public Guid Anonymize(Guid userId)
    {
        if (string.IsNullOrWhiteSpace(options.AnonymizationSecret) || options.AnonymizationSecret.Length < 32)
        {
            throw new InvalidOperationException("User anonymization is not configured.");
        }

        var key = Encoding.UTF8.GetBytes(options.AnonymizationSecret);
        var input = Encoding.UTF8.GetBytes($"finia:user:{userId:D}");
        var hash = HMACSHA256.HashData(key, input);
        Span<byte> guidBytes = stackalloc byte[16];
        hash.AsSpan(0, guidBytes.Length).CopyTo(guidBytes);

        return new Guid(guidBytes);
    }
}
