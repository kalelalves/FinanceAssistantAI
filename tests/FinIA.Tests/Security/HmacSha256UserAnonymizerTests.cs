using FinIA.Application.Configuration;
using FinIA.Application.Security;

namespace FinIA.Tests.Security;

public sealed class HmacSha256UserAnonymizerTests
{
    [Fact]
    public void Anonymize_ShouldReturnStablePseudonymousIdentifier()
    {
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var anonymizer = new HmacSha256UserAnonymizer(new FinIaOptions
        {
            AnonymizationSecret = "stable-test-secret-with-enough-entropy"
        });

        var first = anonymizer.Anonymize(userId);
        var second = anonymizer.Anonymize(userId);

        Assert.Equal(first, second);
        Assert.NotEqual(userId, first);
    }

    [Fact]
    public void Anonymize_ShouldChangeWhenSecretChanges()
    {
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var first = new HmacSha256UserAnonymizer(new FinIaOptions
        {
            AnonymizationSecret = "first-secret-with-enough-entropy"
        });
        var second = new HmacSha256UserAnonymizer(new FinIaOptions
        {
            AnonymizationSecret = "second-secret-with-enough-entropy"
        });

        Assert.NotEqual(first.Anonymize(userId), second.Anonymize(userId));
    }

    [Fact]
    public void Anonymize_ShouldFailWhenSecretIsMissing()
    {
        var anonymizer = new HmacSha256UserAnonymizer(new FinIaOptions());

        Assert.Throws<InvalidOperationException>(() => anonymizer.Anonymize(Guid.NewGuid()));
    }
}
