namespace Beta;

public abstract class BaseTestBuilder
{
    public Delegate Handler { get; private set; } = () => { };

    public void UpdateHandler(Delegate handler)
    {
        Handler = handler;
    }

    public abstract bool IsValid(out List<string> messages);
}