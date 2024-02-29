namespace Beta;

public class StepResult<T>(Func<T> handler)
{
    public T Resolve() => handler();
}