﻿using JetBrains.Annotations;

namespace Beta.AAA;

public class AssertStepBuilder : StepBuilder
{
    public AssertStepBuilder(Action handler)
        : base(handler)
    {
    }

    [PublicAPI]
    public AssertStepBuilder Assert(Action handler) => new(Compile(handler));
}

public class AssertStepBuilder<TIn> : StepBuilder<TIn>
{
    public AssertStepBuilder(Func<TIn> handler)
        : base(handler)
    {
    }

    [PublicAPI]
    public AssertStepBuilder<TIn> Assert(Action<TIn> handler) => new(Compile<TIn>(handler));
}