﻿using System.Reflection;

namespace Beta;

[PublicAPI]
public record BetaTest(object Instance, string Name, Proof Proof)
{
    public Guid Id { get; } = Guid.NewGuid();

    public MethodBase? Method { get; set; }

    public Task Prove()
    {
        return Task.CompletedTask;
    }
}
