namespace Beta;

[PublicAPI]
public static class CommonSteps
{
    public static Step<T> Gather<T>(T value)
    {
        return new Step<T>(() => value);
    }

    public static Step<T> Gather<T>(Func<T> handler)
    {
        return new Step<T>(handler);
    }

    public static Proof<T> Apply<T>(Func<T> handler)
    {
        return new Proof<T>(handler());
    }

    public static Proof<T> Apply<T>(Func<Task<T>> handler)
    {
        return new Proof<T>(handler());
    }
}
