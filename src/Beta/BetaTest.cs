using System.Reflection;

namespace Beta;

[PublicAPI]
public record BetaTest(object Instance, object? Input, string Name, Func<Proof> Proof)
{
    public Guid Id { get; } = Guid.NewGuid();

    public MethodBase? Method { get; set; }
}
