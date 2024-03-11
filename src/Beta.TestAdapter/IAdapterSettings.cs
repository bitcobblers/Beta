namespace Beta.TestAdapter;

public interface IAdapterSettings
{
    RunConfigurationSettings RunConfiguration { get; }

    BetaConfigurationSettings BetaConfiguration { get; }

    BetaDiagnosticConfigurationSettings BetaDiagnosticConfiguration { get; }

    IDictionary<string, string> TestProperties { get; }

    void SaveRandomSeed(string dirname);
    void RestoreRandomSeed(string dirname);
    string? SetTestOutputFolder(string workDirectory);
}
