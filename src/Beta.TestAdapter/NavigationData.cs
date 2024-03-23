using System.Diagnostics.CodeAnalysis;

namespace Beta.TestAdapter;

/// <summary>
///     Defines the navigation data for a single test.
/// </summary>
/// <param name="FileName">The file where the test is defined.</param>
/// <param name="LineNumber">The line number where the test method is.</param>
[ExcludeFromCodeCoverage]
public record NavigationData(string FileName, int LineNumber);
