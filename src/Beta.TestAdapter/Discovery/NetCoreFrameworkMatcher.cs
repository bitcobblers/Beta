using System.Text.RegularExpressions;

namespace Beta.TestAdapter.Discovery;

public class NetCoreFrameworkMatcher : IFrameworkMatcher
{
    private static readonly Regex Net5X = new(@"^net\d+\.\d+$", RegexOptions.IgnoreCase);

    public bool IsMatch(string? framework) =>
        string.IsNullOrWhiteSpace(framework) ||
        Net5X.IsMatch(framework) ||
        framework.StartsWith(".NETCoreApp", StringComparison.OrdinalIgnoreCase) ||
        framework.StartsWith("FrameworkCore10", StringComparison.OrdinalIgnoreCase);
}
