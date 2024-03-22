//using Beta.TestAdapter;
//using mock_assembly;

//namespace Beta.Tests.TestAdapter;

//public class BetaEngineAdapterTests
//{
//    public class CtorMethod : BetaEngineAdapterTests
//    {
//        [Fact]
//        public void CanInit()
//        {
//            // Arrange.
//            var logger = A.Fake<ITestLogger>();
//            var assemblyPath = typeof(SampleTests).Assembly.Location;

//            // Assert.
//            Should.NotThrow(() => new BetaEngineAdapter(assemblyPath, logger));
//        }
//    }

//    public class MaybeThrowsMethod : BetaEngineAdapterTests
//    {
//        [Fact]
//        public void ThrownExceptionWrappedAsBetaEngineLoadFailedException()
//        {
//            // Arrange.
//            var logger = A.Fake<ITestLogger>();

//            // Act.
//            var ex = Should.Throw<BetaEngineLoadFailedException>(() => BetaEngineAdapter.MaybeThrows(
//                logger,
//                "ignored",
//                () =>
//                {
//                    throw new Exception("expected");
//                    return 0;
//                },
//                "ignored {0}"));

//            // Assert.
//            ex.InnerException.ShouldBeOfType<Exception>();
//            ex.InnerException.Message.ShouldBe("expected");
//        }
//    }

//    public class ExecuteMethod : BetaEngineAdapterTests
//    {
//        [Fact]
//        public void CallsMethodOnControllerInstance()
//        {
//            // Arrange.
//            var logger = A.Fake<ITestLogger>();
//            var controllerInstance = A.Fake<IStub>();
//            var adapter = new BetaEngineAdapter(controllerInstance, logger);

//            // Act.
//            adapter.Execute(nameof(IStub.Method), [1, "two"]);

//            // Assert.
//            A.CallTo(() => controllerInstance.Method(1, "two")).MustHaveHappened();
//        }

//        // ReSharper disable once MemberCanBePrivate.Global
//        public interface IStub
//        {
//            void Method(int i, string s);
//        }
//    }
//}
