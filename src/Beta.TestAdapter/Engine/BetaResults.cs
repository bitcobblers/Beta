using System.Xml.Linq;

namespace Beta.TestAdapter.Engine;

public class BetaResults
{
    public enum SkipReason
    {
        NoUnitTests,
        LoadFailure
    }

    public BetaResults(XDocument? results)
    {
        if (results?.Root == null)
        {
            throw new ArgumentException("The XML document does not contain a root element.", nameof(results));
        }

        var root = results.Root;

        FullResults = results;
        RootTestSuite = root.Name == "test-run" ? root.Elements().First() : results.Root;
        IsRunnable = RootTestSuite.Attribute("runstate")?.Value == "Runnable";
        AsString = results.ToString();
    }

    public XDocument FullResults { get; }

    public XElement RootTestSuite { get; }

    public bool IsRunnable { get; }

    public string AsString { get; }

    public SkipReason WhatSkipReason
    {
        get
        {
            string[] messages = ["contains no tests", "Has no TestFixture"];

            var skipped = from property in RootTestSuite.Descendants("property")
                          where property.Attribute("name")?.Value == "_SKIPREASON"
                          let value = property.Attribute("value")?.Value
                          where value != null
                          where messages.Any(value.Contains)
                          select true;

            return skipped.Any() ? SkipReason.NoUnitTests : SkipReason.LoadFailure;
        }
    }

    public bool HasNoUnitTests => WhatSkipReason == SkipReason.NoUnitTests;

    public IEnumerable<XContainer> TestCases => RootTestSuite.Descendants("test-case");
}
