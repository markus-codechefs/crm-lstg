namespace CrmLstg.Core.Metadata;

public static class AvailableSolutionsFormatter
{
    public const string DefaultSolutionUniqueNamePrefix = "Bkw.Crm.";

    public static string FormatMissingSolutionMessage(
        IReadOnlyList<SolutionSummary> solutions,
        string solutionUniqueNamePrefix)
    {
        if (solutions.Count == 0)
        {
            return
                "Solution unique name is required. Provide --solution, set CrmLstg:Solution in appsettings.json, " +
                $"use dotnet user-secrets set \"CrmLstg:Solution\" \"<solution-unique-name>\", " +
                $"set the CrmLstg__Solution environment variable, " +
                $"or choose one of the available solutions matching prefix '{solutionUniqueNamePrefix}' " +
                $"(none were found in the connected environment).";
        }

        var lines = new List<string>
        {
            "Solution unique name is required. Provide --solution, set CrmLstg:Solution in appsettings.json, " +
            "use dotnet user-secrets set \"CrmLstg:Solution\" \"<solution-unique-name>\", " +
            "set the CrmLstg__Solution environment variable, " +
            $"or choose one of the available solutions matching prefix '{solutionUniqueNamePrefix}':",
            string.Empty,
        };

        foreach (var solution in solutions)
        {
            lines.Add($"  {solution.UniqueName} ({solution.FriendlyName}, version {solution.Version})");
        }

        return string.Join(Environment.NewLine, lines);
    }
}
