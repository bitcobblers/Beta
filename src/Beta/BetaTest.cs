using System.Reflection;

namespace Beta;

/// <summary>
///     Defines a single test.
/// </summary>
/// <param name="Container">The instance of the type holding the test.</param>
/// <param name="Input">The optional input for the test.</param>
/// <param name="TestName">The name of the test.</param>
/// <param name="Apply">The method, that when executed will apply input to the test and create a proof.</param>
[PublicAPI]
public record BetaTest(object Container, IEnumerable<object>? Input, string TestName, Func<object, Proof> Apply)
{
    /// <summary>
    ///     Gets the traits for the test.
    /// </summary>
    public Dictionary<string, string> Traits = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Initializes a new instance of the <see cref="BetaTest" /> class.
    /// </summary>
    /// <param name="container">The instance of the type holding the test.</param>
    /// <param name="testName">The name of the test.</param>
    /// <param name="apply">The apply method for the test.</param>
    public BetaTest(object container, string testName, Func<Proof> apply)
        : this(container, null, testName, _ => apply())
    {
    }

    /// <summary>
    ///     Gets the unique identifier for the test.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the inputs as a key-value pair.
    /// </summary>
    public Dictionary<Guid, object> Inputs { get; } =
        (Input ?? Array.Empty<object>()).ToDictionary(_ => Guid.NewGuid());

    /// <summary>
    ///     Gets the FQN of the container for the test.
    /// </summary>
    public string FullyQualifiedTypeName { get; } = Container.GetType().FullName!;

    /// <summary>
    ///     Gets or sets the skip reason for this test.
    /// </summary>
    public string? SkipReason { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the method of the test.
    /// </summary>
    public MethodBase Method { get; set; } = null!;

    /// <summary>
    ///     Sets a trait for this test.
    /// </summary>
    /// <param name="name">The name of the trait.</param>
    /// <param name="value">The value of the trait</param>
    /// <returns>The current test.</returns>
    public BetaTest SetTrait(string name, string value)
    {
        Traits[name] = value;
        return this;
    }

    /// <summary>
    ///     Marks the test as skipped.
    /// </summary>
    /// <param name="reason">The reason for skipping the test.</param>
    /// <returns>The current test.</returns>
    public BetaTest Skip(string reason)
    {
        SkipReason = reason;
        return this;
    }

    #region Probably Unneeded

    // /// <summary>
    // ///     Gets the assembly that contains the test.
    // /// </summary>
    // public Assembly Assembly => Container.GetType().Assembly;
    //
    // /// <summary>
    // ///     Gets the declaring type of the test.
    // /// </summary>
    // public string DeclaringType => Container.GetType().FullName!;
    //
    // /// <summary>
    // ///     Gets the method name of the test.
    // /// </summary>
    // public string MethodName => Method.Name;

    #endregion
}

/// <summary>
///     Defines a single test.
/// </summary>
/// <typeparam name="TInput">The type to use as input for the test.</typeparam>
public record BetaTest<TInput> : BetaTest
{
    /// <summary>
    /// </summary>
    /// <param name="container">The instance of the type holding the test.</param>
    /// <param name="input">The input for the test cases.</param>
    /// <param name="testName">The name of the test.</param>
    /// <param name="apply">The apply method for the test.</param>
    public BetaTest(TestContainer container, IEnumerable<TInput> input, string testName, Func<TInput, Proof> apply)
        : base(container, (IEnumerable<object>)input, testName, o => apply((TInput)o))
    {
    }
}
