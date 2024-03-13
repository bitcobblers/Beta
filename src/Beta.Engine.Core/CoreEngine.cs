using System.Diagnostics;
using Beta.Engine.Core;
using Beta.Engine.Internal;
using Beta.Engine.Services;

namespace Beta.Engine;

public class CoreEngine : IDisposable
{
    private bool _disposed;

    public CoreEngine()
    {
        Services = new ServiceContext();
        WorkDirectory = Environment.CurrentDirectory;
        InternalTraceLevel = InternalTraceLevel.Default;
    }

    public ServiceContext Services { get; }

    public string WorkDirectory { get; set; }

    public InternalTraceLevel InternalTraceLevel { get; set; }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        Services.ServiceManager.StopServices();
    }

    /// <summary>
    ///     Initialize the engine. This includes initializing mono addins,
    ///     setting the trace level and creating the standard set of services
    ///     used in the Engine.
    ///     This interface is not normally called by user code. Programs linking
    ///     only to the nunit.engine.api assembly are given a
    ///     pre-initialized instance of TestEngine. Programs
    ///     that link directly to nunit.engine usually do so
    ///     in order to perform custom initialization.
    /// </summary>
    public void InitializeServices()
    {
        if (InternalTraceLevel != InternalTraceLevel.Off && !InternalTrace.Initialized)
        {
            var logName = string.Format("InternalTrace.{0}.log", Process.GetCurrentProcess().Id);
            InternalTrace.Initialize(Path.Combine(WorkDirectory, logName), InternalTraceLevel);
        }

        // If caller added services beforehand, we don't add any
        if (Services.ServiceCount == 0)
        {
            // Services that depend on other services must be added after their dependencies
            // For example, ResultService uses ExtensionService, so ExtensionService is added
            // later.
            Services.Add(new DriverService());
            Services.Add(new ExtensionService());
        }

        Services.ServiceManager.StartServices();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Services.ServiceManager.Dispose();
            }

            _disposed = true;
        }
    }
}
