using System.Reflection;
using System.Runtime.Loader;
using Beta.TestAdapter;
using Beta.TestAdapter.Exceptions;
using mock_assembly;

namespace Beta.Tests.TestAdapter;

public class EngineAdapterTests
{
    public class CtorMethod : EngineAdapterTests
    {
        [Fact]
        public void CanInit()
        {
            // Arrange.
            var logger = A.Fake<ITestLogger>();
            var assemblyPath = typeof(SampleTests).Assembly.Location;

            // Assert.
            Should.NotThrow(() => new BetaEngineAdapter(assemblyPath, logger));
        }
    }

    public class MaybeThrowsMethod : EngineAdapterTests
    {
        [Fact]
        public void ThrownExceptionWrappedAsBetaEngineLoadFailedException()
        {
            // Arrange.
            var logger = A.Fake<ITestLogger>();

            // Act.
            var ex = Should.Throw<BetaEngineLoadFailedException>(() => BetaEngineAdapter.MaybeThrows(
                logger,
                "ignored",
                () =>
                {
                    throw new Exception("expected");
#pragma warning disable CS0162 // Unreachable code detected
                    return 0;
#pragma warning restore CS0162 // Unreachable code detected
                },
                "ignored {0}"));

            // Assert.
            ex.InnerException.ShouldBeOfType<Exception>();
            ex.InnerException.Message.ShouldBe("expected");
        }
    }

    public class ExecuteMethod : EngineAdapterTests
    {
        [Fact]
        public void CallsMethodOnControllerInstance()
        {
            // Arrange.
            var logger = A.Fake<ITestLogger>();
            var controllerInstance = A.Fake<IStub>();

            // using var adapter = new BetaEngineAdapter(controllerInstance, logger);
            using var adapter = new FakeBetaEngineAdapter(controllerInstance);

            // Act.
            adapter.Execute(nameof(IStub.Method), [1, "two"]);

            // Assert.
            A.CallTo(() => controllerInstance.Method(1, "two")).MustHaveHappened();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public interface IStub
        {
            void Method(int i, string s);
        }

        public class FakeBetaEngineAdapter(object controllerInstance) :
            BetaEngineAdapter(
                "assembly-path",
                A.Fake<ITestLogger>(),
                _ => A.Fake<AssemblyLoadContext>(),
                _ => A.Fake<INavigationDataProvider>())
        {
            /// <inheritdoc />
            protected override Assembly LoadTestAssembly(string assemblyPath) =>
                A.Dummy<Assembly>();

            /// <inheritdoc />
            protected override Assembly LoadBetaAssembly(Assembly assembly) =>
                A.Dummy<Assembly>();

            /// <inheritdoc />
            protected override object CreateController(string typeName, object[] args) =>
                controllerInstance;
        }
    }
}
