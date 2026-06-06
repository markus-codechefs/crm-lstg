using CrmLstg.Core.Configuration;

namespace CrmLstg.Core.Tests.Configuration;

public class ConnectionStringResolverTests
{
    [Fact]
    public void Resolve_CommandLineProvided_ReturnsCommandLineValue()
    {
        // Arrange
        const string commandLineValue = "AuthType=ClientSecret;Url=https://org.crm.dynamics.com;";
        const string configurationValue = "AuthType=OAuth;Url=https://other.crm.dynamics.com;";

        // Act
        var result = ConnectionStringResolver.Resolve(commandLineValue, configurationValue);

        // Assert
        Assert.Equal(commandLineValue, result);
    }

    [Fact]
    public void Resolve_CommandLineMissing_ReturnsConfigurationValue()
    {
        // Arrange
        const string configurationValue = "AuthType=ClientSecret;Url=https://org.crm.dynamics.com;";

        // Act
        var result = ConnectionStringResolver.Resolve(null, configurationValue);

        // Assert
        Assert.Equal(configurationValue, result);
    }

    [Fact]
    public void Resolve_NoValuesProvided_ReturnsEmptyString()
    {
        // Arrange

        // Act
        var result = ConnectionStringResolver.Resolve(null, null);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Resolve_WhitespaceCommandLine_ReturnsConfigurationValue()
    {
        // Arrange
        const string configurationValue = "AuthType=ClientSecret;Url=https://org.crm.dynamics.com;";

        // Act
        var result = ConnectionStringResolver.Resolve("   ", configurationValue);

        // Assert
        Assert.Equal(configurationValue, result);
    }
}
