using FakeItEasy;

namespace Beta.FakeItEasy;

[PublicAPI]
public static class FakeSteps
{
    public static Step<T> Fake<T>(Action<T>? configure = null) where T : class
    {
        return new Step<T>(() =>
        {
            var fake = A.Fake<T>();
            configure?.Invoke(fake);
            return fake;
        });
    }
}