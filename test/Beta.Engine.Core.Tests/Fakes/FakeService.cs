namespace Beta.Engine.Core.Tests.Fakes;

internal interface IFakeService;

public class FakeService : IFakeService, IService
{
    private ServiceStatus _status;

    public bool FailToStart { get; set; }

    public bool FailedToStop { get; set; }

    IServiceLocator IService.ServiceContext { get; set; }

    ServiceStatus IService.Status => _status;

    void IService.StartService()
    {
        _status = FailToStart
            ? ServiceStatus.Error
            : ServiceStatus.Started;
    }

    void IService.StopService()
    {
        _status = ServiceStatus.Stopped;

        if (FailedToStop)
        {
            throw new ArgumentException(nameof(FailedToStop));
        }
    }
}
