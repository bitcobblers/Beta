namespace Beta;

public abstract class BaseTestBuilder : ITestBuilder
{
    private readonly IServiceProvider _provider;

    protected BaseTestBuilder(IServiceProvider provider) => _provider = provider;

    public Delegate Handler { get; private set; } = () => { };

    public T? Requires<T>() => (T?)_provider.GetService(typeof(T));
    public object? Requires(Type type) => _provider.GetService(type);

    public void UpdateHandler(Delegate handler) => Handler = handler;

    public abstract bool IsValid(out List<string> messages);
}