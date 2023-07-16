namespace Beta;

public class BasicTestBuilder : BaseTestBuilder
{
    // ReSharper disable once UnusedParameter.Global
    public new void UpdateHandler(Delegate handler)
    {
        throw new NotSupportedException("Basic test builders cannot update their own handler");
    }
}