// ReSharper disable once CheckNamespace

using Beta.Engine.Api.Extensibility;

namespace Beta.Engine;

public enum ServiceStatus
{
    /// <summary>Service was never started or has been stopped</summary>
    Stopped,

    /// <summary>Started successfully</summary>
    Started,

    /// <summary>Service failed to start and is unavailable</summary>
    Error
}

[TypeExtensionPoint(Description = "Provides a service within the engine and possibly externally as well.")]
public interface IService
{
    /// <summary>
    ///     The ServiceContext
    /// </summary>
    IServiceLocator ServiceContext { get; set; }

    /// <summary>
    ///     Gets the ServiceStatus of this service
    /// </summary>
    ServiceStatus Status { get; }

    /// <summary>
    ///     Initialize the Service
    /// </summary>
    void StartService();

    /// <summary>
    ///     Do any cleanup needed before terminating the service
    /// </summary>
    void StopService();
}
