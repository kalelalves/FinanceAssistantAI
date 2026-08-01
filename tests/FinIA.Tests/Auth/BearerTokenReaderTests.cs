using FinIA.Application.Auth;

namespace FinIA.Tests.Auth;

public sealed class BearerTokenReaderTests
{
    [Fact]
    public void Read_ShouldExtractBearerToken()
    {
        var reader = new BearerTokenReader();

        var token = reader.Read("Bearer abc.def.ghi");

        Assert.Equal("abc.def.ghi", token);
    }

    [Fact]
    public void Read_ShouldReturnNullForMissingBearerPrefix()
    {
        var reader = new BearerTokenReader();

        var token = reader.Read("Basic abc");

        Assert.Null(token);
    }
}
