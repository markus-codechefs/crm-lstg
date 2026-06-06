namespace CrmLstg.Core.Configuration;

public static class SolutionUniqueNameResolver
{
    public static string Resolve(string? commandLineValue, string? configurationValue)
    {
        if (!string.IsNullOrWhiteSpace(commandLineValue))
        {
            return commandLineValue;
        }

        if (!string.IsNullOrWhiteSpace(configurationValue))
        {
            return configurationValue;
        }

        return string.Empty;
    }
}
