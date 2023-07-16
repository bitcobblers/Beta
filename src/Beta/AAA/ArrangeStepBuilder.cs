using JetBrains.Annotations;

namespace Beta.AAA;

//public class ArrangeStepBuilder : StepBuilder
//{
//    public ArrangeStepBuilder(Action handler)
//        : base(handler)
//    {
//    }

//    [PublicAPI]
//    public ArrangeStepBuilder Arrange(Action handler) => new(Compile(handler));

//    [PublicAPI]
//    public ArrangeStepBuilder<T> Arrange<T>(Func<T> handler) => new(Compile(handler));

//    [PublicAPI]
//    public ActStepBuilder Act(Action handler) => new(Compile(handler));

//    [PublicAPI]
//    public ActStepBuilder<T> Act<T>(Func<T> handler) => new(Compile(handler));
//}

public class ArrangeStepBuilder<TInput> : StepBuilder<TInput>
{
    public ArrangeStepBuilder(Func<TInput> handler)
        : base(handler)
    {
    }

    [PublicAPI]
    public ActStepBuilder<TOut> Act<TOut>(Func<TInput, TOut> handler) => new(Compile(handler));
}