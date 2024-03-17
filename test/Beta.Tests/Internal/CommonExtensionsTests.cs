using Beta.Internal;

namespace Beta.Tests.Internal;

public class CommonExtensionsTests
{
    public class ForEachMethod : CommonExtensionsTests
    {
        [Fact]
        public void WhenCollectionIsNull_ThenNoActionIsPerformed()
        {
            // Arrange
            IEnumerable<object>? collection = null;
            var action = A.Fake<Action<object>>();

            // Act
            collection.ForEach(action);

            // Assert
            A.CallTo(() => action(A<object>._)).MustNotHaveHappened();
        }

        [Fact]
        public void WhenCollectionIsEmpty_ThenNoActionIsPerformed()
        {
            // Arrange
            var collection = Enumerable.Empty<object>();
            var action = A.Fake<Action<object>>();

            // Act
            collection.ForEach(action);

            // Assert
            A.CallTo(() => action(A<object>._)).MustNotHaveHappened();
        }

        [Fact]
        public void WhenCollectionHasItems_ThenActionIsPerformed()
        {
            // Arrange
            object[] collection = [new object(), new object()];
            var action = A.Fake<Action<object>>();

            // Act
            collection.ForEach(action);

            // Assert
            A.CallTo(() => action(A<object>._)).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public void WhenCollectionHasNullItems_ThenActionIsNotPerformed()
        {
            // Arrange
            object?[] collection = [new object(), null, new object()];
            var action = A.Fake<Action<object?>>();

            // Act
            collection.ForEach(action);

            // Assert
            A.CallTo(() => action(A<object>._)).MustHaveHappenedTwiceExactly();
        }
    }
}
