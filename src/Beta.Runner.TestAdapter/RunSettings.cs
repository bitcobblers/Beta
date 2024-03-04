using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Beta.Runner.TestAdapter;

public class RunSettings
{
    public enum AppDomainSupport
    {
        IfAvailable = 1,
        Denied = 3
    }

    public string SolutionDirectory { get; private set; }
    public string ResultsDirectory { get; private set; }
    public AppDomainSupport? AppDomain { get; private set; }
    public bool CollectSourceInformation { get; private set; } = true;
    public bool DisableSerialization { get; private set; } = false;
    public bool? ParallelizeAssembly { get; private set; } = true;
    public bool? ParallelizeTestCollections { get; private set; } = true;
    public string? TargetFrameworkVersion { get; private set; }
    public string? TargetPlatform { get; private set; }
    public int BatchSize { get; private set; }
    public int MaxCpuCount { get; private set; }

    public RunSettings()
    {
    }

    public static RunSettings Parse(string? xmlContent)
    {
        RunSettings settings = new();
        XDocument parsed;

        if (string.IsNullOrWhiteSpace(xmlContent))
        {
            return settings;
        }

        try
        {
            parsed = XDocument.Parse(xmlContent);
        }
        catch
        {
            // TODO: Add exceptions to ignore.
            return settings;
        }


        var configElement = parsed.Element("RunConfiguration");

        if (configElement == null)
        {
            return settings;
        }

        settings.SolutionDirectory = ParseString(configElement, "SolutionDirectory") ?? string.Empty;
        settings.ResultsDirectory = ParseString(configElement, "ResultsDirectory") ?? string.Empty;
        settings.CollectSourceInformation = ParseNonNullableBool(configElement, "CollectSourceInformation");
        settings.DisableSerialization = !ParseNonNullableBool(configElement, "DesignMode");
        settings.AppDomain = ParseAppDomainSupport(configElement, "DisableAppDomain");
        settings.ParallelizeAssembly = !ParseNullableBool(configElement, "DisableParallelization") ?? false;
        settings.ParallelizeTestCollections = !ParseNullableBool(configElement, "DisableParallelization") ?? false;
        settings.TargetFrameworkVersion = ParseString(configElement, "TargetFrameworkVersion");
        settings.TargetPlatform = ParseString(configElement, "TargetPlatform");
        settings.BatchSize = ParseNonNullableInt(configElement, "BatchSize");
        settings.MaxCpuCount = ParseNonNullableInt(configElement, "MaxCpuCount");

        return settings;
    }

    private static AppDomainSupport? ParseAppDomainSupport(XContainer xml, string name)
    {
        return ParseNullableBool(xml, name) switch
        {
            true => AppDomainSupport.Denied,
            false => AppDomainSupport.IfAvailable,
            _ => null
        };
    }

    private static bool? ParseNullableBool(XContainer xml, string name)
    {
        if (bool.TryParse(xml.Element(name)?.Value, out var value))
        {
            return value;
        }

        return null;
    }

    private static bool ParseNonNullableBool(XContainer xml, string name)
    {
        return bool.TryParse(xml.Element(name)?.Value, out var value) && value;
    }

    private static int ParseNonNullableInt(XContainer xml, string name)
    {
        return int.TryParse(xml.Element(name)?.Value, out var value) ? value : 1;
    }

    private static string? ParseString(XContainer xml, string name)
    {
        return xml.Element(name)?.Value;
    }
}