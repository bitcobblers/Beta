using Beta.StepBuilders;
using JetBrains.Annotations;

namespace Beta;

public class GuidedTestBuilder : BaseTestBuilder
{
    public GuidedTestBuilder(IServiceProvider serviceProvider) 
        : base(serviceProvider)
    {
    }

    public bool IsArrangeDefined { get; set; }
    public bool IsActDefined { get; set; }
    public bool IsAssertDefined { get; set; }

    [PublicAPI]
    public ArrangeStepBuilder<TContext> Arrange<TContext>(Func<TContext> handler) => new(this, handler);

    public override bool IsValid(out List<string> messages)
    {
        messages = new List<string>();

        if (IsArrangeDefined == false)
        {
            messages.Add("No arrange step defined.");
        }

        if(IsActDefined == false)
        {
            messages.Add("No act step defined.");
        }

        if(IsAssertDefined == false)
        {
            messages.Add("No assert step defined.");
        }

        return messages.Count == 0;
    }
}