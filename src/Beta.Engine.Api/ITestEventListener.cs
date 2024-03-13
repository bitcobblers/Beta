// ReSharper disable once CheckNamespace

using Beta.Engine.Api.Extensibility;

namespace Beta.Engine;

[TypeExtensionPoint(Description =
    "Allows an extension to process progress reports and other events from the test.")]
public interface ITestEventListener
{
    /// <summary>
    ///     Triggered when a test event occurs.
    /// </summary>
    /// <param name="report">The reporting data of the event.</param>
    void OnTestEvent(string report);
}