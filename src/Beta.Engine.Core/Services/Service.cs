// ReSharper disable once CheckNamespace

namespace Beta.Engine.Services;

/// <summary>
///     Abstract base class for services that can use it. Some Services
///     already inherit from a different class and can't use this, which
///     is why we define the IService interface as well.
/// </summary>
public abstract class Service : IService, IDisposable
{
    protected bool _disposed = false;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    /// <inheritdoc />
    public IServiceLocator ServiceContext { get; set; }

    /// <inheritdoc />
    public ServiceStatus Status { get; protected set; }

    /// <inheritdoc />
    public virtual void StartService()
    {
        Status = ServiceStatus.Started;
    }

    /// <inheritdoc />
    public virtual void StopService()
    {
        Status = ServiceStatus.Stopped;
    }

    protected virtual void Dispose(bool disposing)
    {
    }
}
