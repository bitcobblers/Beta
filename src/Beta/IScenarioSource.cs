namespace Beta;

public interface IScenarioSource<out T> : IEnumerable<T>
{
    bool SupportsEagerLoading { get; }
}