namespace Beta;

public class BasicTestBuilder : BaseTestBuilder
{
    public BasicTestBuilder(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    // ReSharper disable once UnusedParameter.Global
    public new void UpdateHandler(Delegate handler)
    {
        throw new NotSupportedException("Basic test builders cannot update their own handler");
    }

    public override bool IsValid(out List<string> messages)
    {
        messages = new List<string>();
        return true;
    }
}
