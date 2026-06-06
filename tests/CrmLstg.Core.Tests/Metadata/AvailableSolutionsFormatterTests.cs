using CrmLstg.Core.Metadata;

namespace CrmLstg.Core.Tests.Metadata;

public class AvailableSolutionsFormatterTests
{
    [Fact]
    public void FormatMissingSolutionMessage_NoSolutionsFound_IncludesPrefixAndConfigurationHints()
    {
        // Arrange
        IReadOnlyList<SolutionSummary> solutions = Array.Empty<SolutionSummary>();
        const string prefix = "Bkw.Crm.";

        // Act
        var result = AvailableSolutionsFormatter.FormatMissingSolutionMessage(solutions, prefix);

        // Assert
        Assert.Contains("Solution unique name is required", result);
        Assert.Contains("CrmLstg:Solution", result);
        Assert.Contains(prefix, result);
        Assert.Contains("none were found", result);
    }

    [Fact]
    public void FormatMissingSolutionMessage_SolutionsFound_ListsEachSolution()
    {
        // Arrange
        IReadOnlyList<SolutionSummary> solutions =
        [
            new("Bkw.Crm.Indiv", "Indiv Solution", "1.0.0.0"),
            new("Bkw.Crm.Shared", "Shared Solution", "2.1.0.0"),
        ];
        const string prefix = "Bkw.Crm.";

        // Act
        var result = AvailableSolutionsFormatter.FormatMissingSolutionMessage(solutions, prefix);

        // Assert
        Assert.Contains("Bkw.Crm.Indiv (Indiv Solution, version 1.0.0.0)", result);
        Assert.Contains("Bkw.Crm.Shared (Shared Solution, version 2.1.0.0)", result);
        Assert.DoesNotContain("none were found", result);
    }
}
