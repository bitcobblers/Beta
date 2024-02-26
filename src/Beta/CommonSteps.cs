using JetBrains.Annotations;

namespace Beta;

public static class CommonSteps
{
    [PublicAPI]
    public static class UnitTest
    {
        public static StepResult<T> Unit<T>(Func<T> handler) => new("Unit", handler);
        public static StepResult<T> Unit<T>(string description, Func<T> handler) => new("Unit", handler, description);
    }

    // ReSharper disable once InconsistentNaming
    [PublicAPI]
    public static class AAA
    {
        public static StepResult<T> Arrange<T>(Func<T> handler) => new("Arrange", handler);
        public static StepResult<T> Act<T>(Func<T> handler) => new("Act", handler);
        // public static Axiom Assert<T>(Func<T>? handler) => new(() => new Axiom(handler));
    }

    // ReSharper disable once InconsistentNaming
    [PublicAPI]
    public static class GWT
    {
        public static StepResult<T> Given<T>(string description, Func<T> handler) => new("Given", handler, description);
        public static StepResult<T> When<T>(string description, Func<T> handler) => new("When", handler, description);
        // public static StepResult<T> Then<T>(string description, Func<T> handler) => new("Then", handler, description);
    }
}