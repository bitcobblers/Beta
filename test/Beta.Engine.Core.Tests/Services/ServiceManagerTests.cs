using Beta.Engine.Core.Tests.Fakes;
using Beta.Engine.Services;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Tests.Services;

public class ServiceManagerTests
{
    private readonly IService _extensionService;
    private readonly IService _fakeService;
    private readonly ServiceManager _serviceManager;

    // [SetUp]
    // public void SetUp()
    public ServiceManagerTests()
    {
        _serviceManager = new ServiceManager();

        _fakeService = new FakeService();
        _serviceManager.AddService(_fakeService);

        _extensionService = new ExtensionService();
        _serviceManager.AddService(_extensionService);
    }

    [Fact]
    public void InitializeServices()
    {
        _serviceManager.StartServices();

        var service1 = _serviceManager.GetService(typeof(IFakeService));
        service1.Status.ShouldBe(ServiceStatus.Started);

        var service2 = _serviceManager.GetService(typeof(IExtensionService));
        service2.Status.ShouldBe(ServiceStatus.Started);
    }

    [Fact]
    public void InitializationFailure()
    {
        ((FakeService)_fakeService).FailToStart = true;
        Should.Throw<InvalidOperationException>(() => _serviceManager.StartServices());
    }

    [Fact]
    public void TerminationFailure()
    {
        ((FakeService)_fakeService).FailedToStop = true;
        _fakeService.StartService();

        Should.NotThrow(() => _serviceManager.StopServices());
    }

    [Fact]
    public void AccessServiceByClass()
    {
        var service = _serviceManager.GetService(typeof(FakeService));

        service.ShouldBeSameAs(_fakeService);
    }

    [Fact]
    public void AccessServiceByInterface()
    {
        var service = _serviceManager.GetService(typeof(IFakeService));

        service.ShouldBeSameAs(_fakeService);
    }
}
