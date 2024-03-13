namespace Beta.Engine;

public interface IServiceLocator
{
    /// <summary>
    /// Return a specified type of service
    /// </summary>
    T GetService<T>() where T : class;

    /// <summary>
    /// Return a specified type of service
    /// </summary>
    object GetService(Type serviceType);
}