using System.Xml.Linq;

namespace Beta.TestAdapter.Internal;

public static class XContainerExtensions
{
    public static int CountElements(XContainer? container, string element)
    {
        return container == null ? 0 : container.Elements(element).Count();
    }

    public static int ParseInt(XContainer? container, string name, int defaultValue)
    {
        var value = container?.Element(name)?.Value;

        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    public static int? ParseNullableInt(XContainer? container, string name)
    {
        var value = container?.Element(name)?.Value;

        return int.TryParse(value, out var result) ? result : default;
    }

    public static T ParseEnum<T>(XContainer? container, string name, T defaultValue)
        where T : struct, Enum
    {
        var value = container?.Element(name)?.Value;

        return Enum.TryParse(value, true, out T result) ? result : defaultValue;
    }

    public static bool ParseBool(XContainer? container, string name, bool defaultValue)
    {
        var value = container?.Element(name)?.Value;

        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    public static string? ParseString(XContainer? container, string name, string? defaultValue = null)
    {
        return container?.Element(name)?.Value ?? defaultValue;
    }
}
