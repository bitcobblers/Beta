using System.Xml.Linq;

namespace Beta.TestAdapter;

public record RunSettings
{
    public static RunSettings Empty { get; } = new();

    public string? ResultsDirectory { get; private set; }
    public string? SolutionDirectory { get; private set; }
    public int MaxCpuCount { get; private set; }
    public bool CollectSourceInformation { get; private set; }
    public string? TargetFrameworkVersion { get; private set; }
    public string? TargetPlatform { get; private set; }
    public bool DesignMode { get; private set; }
    public int BatchSize { get; private set; }

    public static RunSettings Parse(string? xmlContent)
    {
        RunSettings settings = new();
        XDocument parsed;

        if (string.IsNullOrWhiteSpace(xmlContent))
        {
            return Empty;
        }

        try
        {
            parsed = XDocument.Parse(xmlContent);
        }
        catch
        {
            // TODO: Add exceptions to ignore.
            return Empty;
        }

        var configElement = parsed.Root?.Element("RunConfiguration");

        if (configElement == null)
        {
            return Empty;
        }

        return new RunSettings
        {
            SolutionDirectory = ParseString(configElement, nameof(SolutionDirectory)),
            ResultsDirectory = ParseString(configElement, nameof(ResultsDirectory)),
            CollectSourceInformation = ParseBool(configElement, nameof(CollectSourceInformation)),
            TargetFrameworkVersion = ParseString(configElement, nameof(TargetFrameworkVersion)),
            TargetPlatform = ParseString(configElement, nameof(TargetPlatform)),
            BatchSize = ParseInt(configElement, nameof(BatchSize)),
            DesignMode = ParseBool(configElement, nameof(DesignMode)),
            MaxCpuCount = ParseInt(configElement, nameof(MaxCpuCount))
        };

        //settings.SolutionDirectory = ParseString(configElement, nameof(SolutionDirectory));
        //settings.ResultsDirectory = ParseString(configElement, nameof(ResultsDirectory));
        //settings.CollectSourceInformation = ParseBool(configElement, nameof(CollectSourceInformation));
        //settings.TargetFrameworkVersion = ParseString(configElement, nameof(TargetFrameworkVersion));
        //settings.TargetPlatform = ParseString(configElement, nameof(TargetPlatform));
        //settings.BatchSize = ParseInt(configElement, nameof(BatchSize));
        //settings.DesignMode = ParseBool(configElement, nameof(DesignMode));
        //settings.MaxCpuCount = ParseInt(configElement, nameof(MaxCpuCount));

        //return settings;
    }

    private static bool ParseBool(XContainer xml, string name, bool defaultValue = false)
    {
        return bool.TryParse(xml.Element(name)?.Value, out var value) ? value : defaultValue;
    }

    private static int ParseInt(XContainer xml, string name, int defaultValue = 0)
    {
        return int.TryParse(xml.Element(name)?.Value, out var value) ? value : defaultValue;
    }

    private static string ParseString(XContainer xml, string name, string defaultValue = "")
    {
        return xml.Element(name)?.Value ?? defaultValue;
    }
}
