namespace Beta.TestAdapter.Tests;

public class DiaSessionManagerTests
{
    [Fact]
    public void GettingSessionForFirstTimeAutomaticallyLoadsIt()
    {
        // Arrange.
        var session = A.Fake<DiaSessionWrapper>();
        var manager = new DiaSessionManager(_ => session);

        // Act.
        var result = manager.GetSession("assembly.dll");

        // Assert.
        result.ShouldNotBeNull();
    }

    [Fact]
    public void GettingSessionForSameAssemblyReturnsSameInstance()
    {
        // Arrange.
        var manager = new DiaSessionManager(_ => A.Fake<DiaSessionWrapper>());

        // Act.
        var result1 = manager.GetSession("assembly.dll");
        var result2 = manager.GetSession("assembly.dll");

        // Assert.
        result1.ShouldBeSameAs(result2);
    }

    [Fact]
    public void DisposingManagerDisposesAllSessions()
    {
        // Arrange.
        var session1 = A.Fake<DiaSessionWrapper>();
        var session2 = A.Fake<DiaSessionWrapper>();
        var handler = A.Fake<Func<string, DiaSessionWrapper>>();

        A.CallTo(() => handler.Invoke(A<string>._))
         .ReturnsNextFromSequence(session1, session2);


        var manager = new DiaSessionManager(handler);

        manager.GetSession("assembly1.dll");
        manager.GetSession("assembly2.dll");

        // Act.
        manager.Dispose();

        // Assert.
        A.CallTo(() => session1.Dispose()).MustHaveHappened();
        A.CallTo(() => session2.Dispose()).MustHaveHappened();
    }
}
