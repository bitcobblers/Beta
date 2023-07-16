namespace Beta;

public class BaseTestBuilder
{
    public Delegate Handler { get; private set; } = () => { };

    public void UpdateHandler(Delegate handler)
    {
        Handler = handler;
    }
}