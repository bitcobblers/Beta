using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Beta.TestAdapter;

/// <summary>
///     Defines a navigation provider for inspecting the test assembly.
/// </summary>
public class NavigationDataProvider : IDisposable
{
    private readonly DiaSession? _session;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NavigationDataProvider" /> class.
    /// </summary>
    /// <param name="assemblyPath">The path to the assembly to create the navigator for.</param>
    public NavigationDataProvider(string assemblyPath)
    {
        try
        {
            _session = new DiaSession(assemblyPath);
        }
        catch
        {
            // Ignore any exceptions loading the assembly.  Subsequent calls will return null.
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Finalizes an instance of the provider.
    /// </summary>
    [ExcludeFromCodeCoverage]
    ~NavigationDataProvider() => Dispose(false);

    /// <summary>
    ///     Gets the navigation data for a test method.
    /// </summary>
    /// <param name="className">The fully qualified name of the class that defined the test.</param>
    /// <param name="methodName">The name of the test method.</param>
    /// <returns>The navigation data for the test method, or null if not found.</returns>
    public NavigationData? Get(string className, string methodName)
    {
        var symbol = _session?.GetNavigationDataForMethod(className, methodName);

        return symbol == null
            ? null
            : new NavigationData(symbol.FileName!, symbol.MinLineNumber);
    }

    private void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _session?.Dispose();
        }

        _disposed = true;
    }
}
