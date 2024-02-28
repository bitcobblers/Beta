using JetBrains.Annotations;

namespace Beta;

[PublicAPI]
public static class CommonSteps
{
    public static StepResult<T> Gather<T>(T value) => new(() => value);
    public static StepResult<T> Gather<T>(Func<T> handler) => new(handler);
    public static Axiom<T> Apply<T>(Func<T> handler) => new(handler());
    public static Axiom<T> Apply<T>(Func<Task<T>> handler) => new(handler());
}