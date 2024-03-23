using System.Xml.Linq;

namespace Beta.TestAdapter.Models;

/// <summary>
///     Gets the run settings for the adapter.
/// </summary>
public class RunSettings
{
    /// <summary>
    ///     Gets the common run configuration used by vstest.
    /// </summary>
    public RunConfiguration Configuration { get; private init; } = new();

    /// <summary>
    ///     Parses an XML string into a settings object.
    /// </summary>
    /// <param name="runSettingsXml">The XML string to parse.</param>
    /// <returns>The parsed run settings.</returns>
    public static RunSettings Parse(string? runSettingsXml)
    {
        if (runSettingsXml is null)
        {
            return new RunSettings();
        }

        try
        {
            var xmlDoc = XDocument.Parse(runSettingsXml);

            return new RunSettings
            {
                Configuration = RunConfiguration.Parse(xmlDoc.Root!.Element("RunConfiguration"))
            };
        }
        catch (Exception)
        {
            return new RunSettings();
        }
    }
}
