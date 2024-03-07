using static Beta.CommonSteps;

namespace Beta.Tests;

public class CommonStepsTests
{
    public class GatherMethod : TestContainerTests
    {
        [Fact]
        public void CanGatherFromValue()
        {
            // Act
            var step = Gather(42);

            // Assert
            step.Evaluate().ShouldBe(42);
        }

        [Fact]
        public void CanGatherFromHandler()
        {
            // Act
            var step = Gather(() => 42);

            // Assert
            step.Evaluate().ShouldBe(42);
        }
    }

    public class ApplyMethod : TestContainerTests
    {
        [Fact]
        public async Task CanApplyFromHandler()
        {
            // Act
            var proof = Apply(() => 42);

            // Assert
            (await proof.Actual).ShouldBe(42);
        }

        [Fact]
        public async Task CanApplyFromAsyncHandler()
        {
            // Act
            var proof = Apply(() => Task.FromResult(42));

            // Assert
            (await proof.Actual).ShouldBe(42);
        }
    }
}