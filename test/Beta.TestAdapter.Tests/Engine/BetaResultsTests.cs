using System.Xml.Linq;
using Beta.TestAdapter.Engine;

namespace Beta.TestAdapter.Tests.Engine;

public class BetaResultsTests
{
    [Fact]
    public void NullResultsThrowsArgumentException()
    {
        // Act and assert.
        Should.Throw<ArgumentException>(() => new BetaResults(null));
    }

    [Fact]
    public void NullRootThrowsArgumentException()
    {
        // Arrange.
        var results = new XDocument();

        // Act and assert.
        Should.Throw<ArgumentException>(() => new BetaResults(results));
    }

    [Fact]
    public void CanGetRootTestSuiteFromFirstChild()
    {
        // Arrange.
        var xml =
            new XDocument(
                new XElement("test-run",
                    new XElement("test-suite")));

        // Act.
        var results = new BetaResults(xml);

        // Assert.
        results.RootTestSuite.Name.ShouldBe("test-suite");
    }

    [Fact]
    public void CanGetRootTestSuiteFromRoot()
    {
        // Arrange.
        var xml =
            new XDocument(
                new XElement("test-suite"));

        // Act.
        var results = new BetaResults(xml);

        // Assert.
        results.RootTestSuite.Name.ShouldBe("test-suite");
    }

    [Fact]
    public void ExtractsRunnableStateFromRootSuite()
    {
        // Arrange.
        var xml =
            new XDocument(
                new XElement("test-run",
                    new XElement("test-suite",
                        new XAttribute("runstate", "Runnable"))));

        // Act.
        var results = new BetaResults(xml);

        // Assert.
        results.IsRunnable.ShouldBeTrue();
    }

    [Fact]
    public void ExtractsNestedTestCases()
    {
        // Arrange.
        var xml =
            new XDocument(
                new XElement("test-run",
                    new XElement("test-suite",
                        new XElement("test-case"),
                        new XElement("test-suite",
                            new XElement("test-case")))));

        // Act.
        var results = new BetaResults(xml);

        // Assert.
        results.TestCases.Count().ShouldBe(2);
    }

    [Fact]
    public void SkipReasonSetToNoTestsIfSkipReasonIsSet()
    {
        // Arrange.
        var xml =
            new XDocument(
                new XElement("test-run",
                    new XElement("test-suite",
                        new XElement("property",
                            new XAttribute("name", "_SKIPREASON"),
                            new XAttribute("value", "contains no tests")))));

        // Act.
        var results = new BetaResults(xml);

        // Assert.
        results.WhatSkipReason.ShouldBe(BetaResults.SkipReason.NoUnitTests);
    }

    [Fact]
    public void SkipReasonSetToLoadFailureIfNoSkipReasonIsSet()
    {
        // Arrange.
        var xml =
            new XDocument(
                new XElement("test-run",
                    new XElement("test-suite")));

        // Act.
        var results = new BetaResults(xml);

        // Assert.
        results.WhatSkipReason.ShouldBe(BetaResults.SkipReason.LoadFailure);
    }

    [Fact]
    public void HasNoUnitTestsReturnsTrueIfSkipReasonIsNoUnitTests()
    {
        // Arrange.
        var xml =
            new XDocument(
                new XElement("test-run",
                    new XElement("test-suite",
                        new XElement("property",
                            new XAttribute("name", "_SKIPREASON"),
                            new XAttribute("value", "contains no tests")))));

        // Act.
        var results = new BetaResults(xml);

        // Assert.
        results.HasNoUnitTests.ShouldBeTrue();
    }
}
