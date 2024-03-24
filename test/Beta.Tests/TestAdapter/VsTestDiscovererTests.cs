using Beta.TestAdapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using static Beta.TestAdapter.Factories;

namespace Beta.Tests.TestAdapter;

public class VsTestDiscovererTests
{
    [Fact]
    public void DiscoveryRegistersAnyFoundTests()
    {
        // Arrange.
        var context = A.Fake<IDiscoveryContext>();
        var logger = A.Fake<IMessageLogger>();
        var sink = A.Fake<ITestCaseDiscoverySink>();
        var adapter = A.Fake<IEngineAdapter>();
        var factory = new EngineAdapterFactory(_ => adapter);

        A.CallTo(() => adapter.Query())
         .Returns([A.Dummy<TestCase>()]);

        var discoverer = new VsTestDiscoverer(factory);

        // Act.
        discoverer.DiscoverTests(["test.dll"], context, logger, sink);

        // Assert.
        A.CallTo(() => sink.SendTestCase(A<TestCase>._))
         .MustHaveHappened(1, Times.Exactly);
    }
}
