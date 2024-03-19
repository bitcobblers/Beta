using System.Xml.Linq;

namespace Beta.TestAdapter.Models;

/// <summary>
///     Defines the common run configuration used by vstest.
/// </summary>
public class RunConfiguration
{
    /// <summary>
    ///     Gets the path to store any results.
    /// </summary>
    public string ResultsDirectory { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the solution directory.
    /// </summary>
    public string SolutionDirectory { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the maximum number of CPUs to use for running tests.
    /// </summary>
    public int MaxCpuCount { get; init; }

    /// <summary>
    ///     Gets any environment variables that were captured by the test runner.
    /// </summary>
    public Dictionary<string, string> EnvironmentVariables { get; private init; } = new();

    /// <summary>
    ///     Gets a value indicating whether to collect source information.
    /// </summary>
    public bool CollectSourceInformation { get; init; }

    /// <summary>
    ///     Gets the target framework of the assembly.
    /// </summary>
    public string TargetFrameworkVersion { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the target platform of the assembly.
    /// </summary>
    public string TargetPlatform { get; init; } = string.Empty;

    /// <summary>
    ///     Gets a value indicating whether the test run is running in design mode.
    /// </summary>
    public bool DesignMode { get; init; }

    /// <summary>
    ///     Gets the maximum batch size to use.
    /// </summary>
    public int BatchSize { get; init; } = 1;

    /// <summary>
    ///     Parses the XML container into the run configuration.
    /// </summary>
    /// <param name="xml">The XML container to parse.</param>
    /// <returns>The parsed run configuration.</returns>
    public static RunConfiguration Parse(XContainer? xml)
    {
        if (xml == null)
        {
            return new RunConfiguration();
        }

        return new RunConfiguration
        {
            ResultsDirectory = ParseString(xml, nameof(ResultsDirectory), string.Empty),
            SolutionDirectory = ParseString(xml, nameof(SolutionDirectory), string.Empty),
            MaxCpuCount = ParseInt(xml, nameof(MaxCpuCount), 0),
            EnvironmentVariables = ParseEnvironmentVariables(xml),
            CollectSourceInformation = ParseBool(xml, nameof(CollectSourceInformation), false),
            TargetFrameworkVersion = ParseString(xml, nameof(TargetFrameworkVersion), string.Empty),
            TargetPlatform = ParseString(xml, nameof(TargetPlatform), string.Empty),
            DesignMode = ParseBool(xml, nameof(DesignMode), false),
            BatchSize = ParseInt(xml, nameof(BatchSize), 1)
        };
    }

    private static string ParseString(XContainer xml, string elementName, string defaultValue) =>
        xml.Element(elementName)?.Value ?? defaultValue;

    private static int ParseInt(XContainer xml, string elementName, int defaultValue) =>
        int.TryParse(xml.Element(elementName)?.Value, out var value)
            ? value
            : defaultValue;

    private static bool ParseBool(XContainer xml, string elementName, bool defaultValue) =>
        bool.TryParse(xml.Element(elementName)?.Value, out var value)
            ? value
            : defaultValue;

    private static Dictionary<string, string> ParseEnvironmentVariables(XContainer xml) =>
        xml.Element("EnvironmentVariables")?
           .Elements()
           .ToDictionary(
               e => e.Name.ToString(),
               e => e.Value.ToString()) ?? new Dictionary<string, string>();
}
