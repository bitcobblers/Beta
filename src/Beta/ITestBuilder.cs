namespace Beta;

public interface ITestBuilder
{
    Delegate Handler { get; }

    T? Requires<T>();
    object? Requires(Type type);

    bool IsValid(out List<string> messages);
}