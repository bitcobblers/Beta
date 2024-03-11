using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Beta.TestAdapter;

public interface IAdapterSettings
{
    RunConfigurationSettings RunConfiguration { get; }

    BetaConfigurationSettings BetaConfiguration { get; }

    BetaDiagnosticConfigurationSettings BetaDiagnosticConfiguration { get; }

    IDictionary<string, string> TestProperties { get; }

    void Load(IDiscoveryContext context, TestLogger testLogger = null);

    void Load(string settingsXml);
    void SaveRandomSeed(string dirname);
    void RestoreRandomSeed(string dirname);
    string SetTestOutputFolder(string workDirectory);
}
