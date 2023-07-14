namespace Beta;

public static class CollectionExtensions
{
    public static void ToTest<T>(this IEnumerable<T> collection, BetaTest test, string name, BetaTest.TestConfigurator<T> configure)
    {
        foreach (var input in collection)
        {
            test.AddTest(name, input, configure);
        }
    }
}