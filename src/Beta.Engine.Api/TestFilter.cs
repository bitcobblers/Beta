// ReSharper disable once CheckNamespace

namespace Beta.Engine;

[Serializable]
public class TestFilter
{
    /// <summary>
    ///     Empty filter that always passes.
    /// </summary>
    public static readonly TestFilter Empty = new("<filter />");

    /// <summary>
    ///     Initializes a new instance of the <see cref="TestFilter" /> class.
    /// </summary>
    /// <param name="xmlText">The XML text for the filter expression.</param>
    public TestFilter(string xmlText)
    {
        Text = xmlText;
    }

    /// <summary>
    ///     Gets an XML representation of the filter as a string.
    /// </summary>
    public string Text { get; }
}
