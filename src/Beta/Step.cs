namespace Beta;

public class Step<T>(Func<T> handler)
{
    public T Evaluate() => handler();
}