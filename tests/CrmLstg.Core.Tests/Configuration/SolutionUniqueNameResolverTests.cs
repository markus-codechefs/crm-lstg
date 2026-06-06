using CrmLstg.Core.Configuration;

namespace CrmLstg.Core.Tests.Configuration;

public class SolutionUniqueNameResolverTests
{
    [Fact]
    public void Resolve_CommandLineProvided_ReturnsCommandLineValue()
    {
        // Arrange
        const string commandLineValue = "Bkw.Crm.Indiv";
        const string configurationValue = "Bkw.Crm.Other";

        // Act
        var result = SolutionUniqueNameResolver.Resolve(commandLineValue, configurationValue);

        // Assert
        Assert.Equal(commandLineValue, result);
    }

    [Fact]
    public void Resolve_CommandLineMissing_ReturnsConfigurationValue()
    {
        // Arrange
        const string configurationValue = "Bkw.Crm.Indiv";

        // Act
        var result = SolutionUniqueNameResolver.Resolve(null, configurationValue);

        // Assert
        Assert.Equal(configurationValue, result);
    }

    [Fact]
    public void Resolve_NoValuesProvided_ReturnsEmptyString()
    {
        // Arrange

        // Act
        var result = SolutionUniqueNameResolver.Resolve(null, null);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Resolve_WhitespaceCommandLine_ReturnsConfigurationValue()
    {
        // Arrange
        const string configurationValue = "Bkw.Crm.Indiv";

        // Act
        var result = SolutionUniqueNameResolver.Resolve("   ", configurationValue);

        // Assert
        Assert.Equal(configurationValue, result);
    }
}
