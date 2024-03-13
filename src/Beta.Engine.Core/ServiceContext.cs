using Beta.Engine.Services;

namespace Beta.Engine.Core;

public class ServiceContext : IServiceLocator
{
    public ServiceContext()
    {
        ServiceManager = new ServiceManager();
    }

    public ServiceManager ServiceManager { get; }

    public int ServiceCount => ServiceManager.ServiceCount;

    public T GetService<T>() where T : class
    {
        return ServiceManager.GetService(typeof(T)) as T;
    }

    public object GetService(Type serviceType)
    {
        return ServiceManager.GetService(serviceType);
    }

    public void Add(IService service)
    {
        ServiceManager.AddService(service);
        service.ServiceContext = this;
    }
}