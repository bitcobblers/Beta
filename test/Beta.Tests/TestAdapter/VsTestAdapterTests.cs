using Beta.TestAdapter;
using Beta.TestAdapter.Models;

namespace Beta.Tests.TestAdapter;

public class VsTestAdapterTests
{
    public class ToTestCaseMethod : VsTestAdapterTests
    {
        [Fact]
        public void ConvertsTestToTestCase()
        {
            // Arrange.
            var navigationProvider = A.Fake<INavigationDataProvider>();

            A.CallTo(() => navigationProvider.Get(A<string>._, A<string>._))
             .Returns(new NavigationData("some-file.cs", 10));

            var test = new DiscoveredTest
            {
                ClassName = "SomeClass",
                MethodName = "SomeMethod",
                Input = "my-input",
                InputIndex = 1,
                TestName = "Some Test"
            };

            // Act.
            var testCase = VsTestAdapter.ToTestCase(test, "some-file.dll", navigationProvider);

            // Assert.
            testCase.ExecutorUri.ShouldBe(new Uri(VsTestExecutor.ExecutorUri));
            testCase.FullyQualifiedName.ShouldBe("SomeClass.SomeMethod");
            testCase.DisplayName.ShouldBe("Some Test");
            testCase.Source.ShouldBe("some-file.dll");
            testCase.CodeFilePath.ShouldBe("some-file.cs");
            testCase.LineNumber.ShouldBe(10);
            testCase.GetPropertyValue<string>(VsTestAdapter.TestCaseInputProperty, null).ShouldBe("1");
        }
    }
}