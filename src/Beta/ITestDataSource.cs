namespace Beta;

public interface ITestDataSource<out T> : IEnumerable<T>
{
    bool SupportsEagerLoading { get; }
}