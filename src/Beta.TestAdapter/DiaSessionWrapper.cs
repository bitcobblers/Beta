using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Navigation;

namespace Beta.TestAdapter;

public class DiaSessionWrapper : IDisposable
{
    private readonly DiaSession? _session;
    private bool _disposed;

    public DiaSessionWrapper(string assemblyFilePath)
    {
        try
        {
            _session = new DiaSession(assemblyFilePath);
        }
        catch
        {
            // ignored -- all resolutions will return null.
        }
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~DiaSessionWrapper()
    {
        Dispose(false);
    }

    public INavigationData? GetNavigationData(MethodBase? method)
    {
        if (method?.DeclaringType?.FullName == null || _session == null)
        {
            return null;
        }

        var attr = method.GetCustomAttribute<AsyncStateMachineAttribute>()?.StateMachineType;

        return attr is { FullName: not null }
            ? _session.GetNavigationDataForMethod(attr.FullName, "MoveNext")
            : _session.GetNavigationDataForMethod(method.DeclaringType.FullName, method.Name);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _session?.Dispose();
        }

        _disposed = true;
    }
}
