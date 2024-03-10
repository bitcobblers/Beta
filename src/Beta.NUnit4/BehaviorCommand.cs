using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace Beta.NUnit4;

public class BehaviorCommand(Test test) : TestCommand(test)
{
    /// <inheritdoc />
    public override TestResult Execute(TestExecutionContext context)
    {
        throw new NotImplementedException();
    }
}
