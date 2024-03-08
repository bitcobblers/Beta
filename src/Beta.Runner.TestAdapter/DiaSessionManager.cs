namespace Beta.Runner.TestAdapter;

public class DiaSessionManager : IDisposable
{
    private readonly Dictionary<string, DiaSessionWrapper> _sessions = new();

    private bool _disposed;

    ~DiaSessionManager()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public DiaSessionWrapper GetSession(string assemblyFilePath)
    {
        if (_sessions.TryGetValue(assemblyFilePath, out var session))
        {
            return session;
        }

        session = new DiaSessionWrapper(assemblyFilePath);
        _sessions[assemblyFilePath] = session;

        return session;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            foreach (var session in _sessions.Values)
            {
                session.Dispose();
            }

            _sessions.Clear();
        }

        _disposed = true;
    }
}
