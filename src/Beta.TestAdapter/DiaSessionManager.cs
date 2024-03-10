namespace Beta.TestAdapter;

public class DiaSessionManager : IDisposable
{
    private readonly Func<string, DiaSessionWrapper> _createSession;
    private readonly Dictionary<string, DiaSessionWrapper> _sessions;

    private bool _disposed;

    public DiaSessionManager()
        : this(assemblyFilePath => new DiaSessionWrapper(assemblyFilePath))
    {
    }

    internal DiaSessionManager(Func<string, DiaSessionWrapper> createSession)
    {
        _createSession = createSession;
        _sessions = new Dictionary<string, DiaSessionWrapper>();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~DiaSessionManager()
    {
        Dispose(false);
    }

    public DiaSessionWrapper GetSession(string assemblyFilePath)
    {
        if (_sessions.TryGetValue(assemblyFilePath, out var session))
        {
            return session;
        }

        session = _createSession(assemblyFilePath);
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
