using Microsoft.Extensions.Configuration;
using Moq;
using RuleEditor.Helpers;

namespace RuleEditor.Tests;

public class KeyStoreTests
{
    [Fact]
    public void ReadKey()
    {
        var myConfiguration = new Dictionary<string, string?>
        {
            { "publicKey", "../../../Data/key.txt" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();
        var keyStore = new KeyStore(configuration);
        Assert.Equal("OK", keyStore.PublicKey);
    }

    [Fact]
    public void Error()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(null)
            .Build();
        Assert.Throws<ArgumentNullException>(() => new KeyStore(configuration));
    }
}