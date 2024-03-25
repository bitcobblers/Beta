namespace Beta.Internal;

public record DiscoveredTest
{
    public string ClassName { get; init; }
    public string MethodName { get; init; }
    public string Input { get; init; }
    public string TestName { get; init; }
}