using Beta.Engine;

namespace Beta.TestAdapter.Engine;

public interface IBetaEngineAdapter
{
    BetaResults Explore();
    void CloseRunner();
    BetaResults Explore(TestFilter filter);
    BetaResults Run(ITestEventListener listener, TestFilter filter);
    void StopRun();

    T GetService<T>() where T : class;

    void GenerateTestOutput(BetaResults testResults, string assemblyPath, string testOutputXmlFolder);
    string GetXmlFilePath(string folder, string defaultFileName, string extension);
}

public class BetaEngineAdapter : IBetaEngineAdapter, IDisposable
{
    private readonly ITestLogger _logger;
    private readonly IAdapterSettings _settings;

    public BetaEngineAdapter(IAdapterSettings settings, ITestLogger logger)
    {
        _settings = settings;
        _logger = logger;

        var engine = TestEngineActivator.CreateInstance();

        if (engine == null)
        {
            // engine = new TestEngine();
        }
    }

    /// <inheritdoc />
    public BetaResults Explore()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void CloseRunner()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public BetaResults Explore(TestFilter filter)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public BetaResults Run(ITestEventListener listener, TestFilter filter)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void StopRun()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public T GetService<T>() where T : class
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void GenerateTestOutput(BetaResults testResults, string assemblyPath, string testOutputXmlFolder)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public string GetXmlFilePath(string folder, string defaultFileName, string extension)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
