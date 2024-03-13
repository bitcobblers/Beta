using Beta.Engine.Internal;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Services;

/// <summary>
///     ServiceManager handles access to all services - global
///     facilities shared by all instances of TestEngine.
/// </summary>
public class ServiceManager : IDisposable
{
    private static readonly Logger log = InternalTrace.GetLogger(typeof(ServiceManager));

    private readonly Dictionary<Type, IService> _serviceIndex = new();
    private readonly List<IService> _services = [];

    private bool _disposed;

    public bool ServicesInitialized { get; private set; }

    public int ServiceCount => _services.Count;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IService? GetService(Type serviceType)
    {
        if (!_serviceIndex.TryGetValue(serviceType, out var theService))
        {
            foreach (var service in _services.Where(serviceType.IsInstanceOfType))
            {
                _serviceIndex[serviceType] = service;
                theService = service;
                break;
            }
        }

        if (theService == null)
        {
            log.Error($"Requested service {serviceType.FullName} was not found");
        }
        else
        {
            log.Debug($"Request for service {serviceType.Name} satisfied by {theService.GetType().Name}");
        }

        return theService;
    }

    public void AddService(IService service)
    {
        _services.Add(service);
        log.Debug("Added " + service.GetType().Name);
    }

    public void StartServices()
    {
        foreach (var service in _services)
        {
            if (service.Status != ServiceStatus.Stopped)
            {
                continue;
            }

            var name = service.GetType().Name;
            log.Info("Initializing " + name);

            try
            {
                service.StartService();
                if (service.Status == ServiceStatus.Error)
                {
                    throw new InvalidOperationException("Service failed to initialize " + name);
                }
            }
            catch (Exception ex)
            {
                // TODO: Should we pass this exception through?
                log.Error("Failed to initialize " + name);
                log.Error(ex.ToString());
                throw;
            }
        }

        ServicesInitialized = true;
    }

    public void StopServices()
    {
        // Stop services in reverse of initialization order
        var index = _services.Count;
        while (--index >= 0)
        {
            var service = _services[index];
            if (service.Status != ServiceStatus.Started)
            {
                continue;
            }

            var name = service.GetType().Name;
            log.Info("Stopping " + name);

            try
            {
                service.StopService();
            }
            catch (Exception ex)
            {
                log.Error("Failure stopping service " + name);
                log.Error(ex.ToString());
            }
        }
    }

    public void ClearServices()
    {
        log.Info("Clearing Service list");
        _services.Clear();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        StopServices();

        if (disposing)
        {
            foreach (var disposable in _services.OfType<IDisposable>())
            {
                disposable.Dispose();
            }
        }

        _disposed = true;
    }
}
