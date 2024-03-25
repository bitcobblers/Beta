using System.Reflection;
using System.Runtime.Loader;
using Beta.TestAdapter.Exceptions;
using static Beta.TestAdapter.Factories;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Beta.TestAdapter;

/// <summary>
///     Defines an adapter for calling the beta engine from the test assembly's referenced engine.
/// </summary>
/// <param name="logger">The internal logger to use.</param>
public class BetaEngineAdapter(ITestLogger logger) : IEngineAdapter
{
    private const string ControllerName = "Beta.Internal.BetaEngineController";

    private readonly string _assemblyPath;
    private readonly AssemblyLoadContext _loadContext;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BetaEngineAdapter" /> class.
    /// </summary>
    /// <param name="assemblyPath">The path to the test assembly.</param>
    /// <param name="logger">The internal logger to use.</param>
    public BetaEngineAdapter(string assemblyPath, ITestLogger logger)
        : this(
            assemblyPath,
            logger,
            p => new CustomAssemblyLoadContext(p))
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BetaEngineAdapter" /> class.
    /// </summary>
    /// <param name="assemblyPath">The path to the test assembly.</param>
    /// <param name="logger">The internal logger to use.</param>
    /// <param name="loadContextFactory">The factory method to create a new loader context.</param>
    protected BetaEngineAdapter(string assemblyPath,
                                ITestLogger logger,
                                GetLoadContextFactory loadContextFactory)
        : this(logger)
    {
        _assemblyPath = Path.GetFullPath(assemblyPath);
        _loadContext = loadContextFactory(_assemblyPath);
    }

    /// <inheritdoc />
    public IEngineController? GetController()
    {
        var testAssembly = LoadTestAssembly();

        if (testAssembly == null)
        {
            return null;
        }

        var betaAssembly = LoadBetaAssembly(testAssembly);

        if (betaAssembly == null)
        {
            return null;
        }

        var controllerInstance = CreateController(ControllerName, betaAssembly, [testAssembly]);

        return controllerInstance == null
            ? null
            : new WrappedEngineController(controllerInstance);
    }

    /// <summary>
    ///     Uses the context to load the test assembly.
    /// </summary>
    /// <returns>
    ///     The loaded test assembly.
    /// </returns>
    protected virtual Assembly? LoadTestAssembly() =>
        MaybeThrows(
            logger,
            $"Loading test assembly: {_assemblyPath}",
            () => _loadContext.LoadFromAssemblyPath(_assemblyPath),
            "Unable to load test assembly: {0}");

    /// <summary>
    ///     Gets the beta assembly as a reference from the test assembly.
    /// </summary>
    /// <param name="assembly">The test assembly to get the beta reference from.</param>
    /// <returns>
    ///     A reference to the beta assembly.
    /// </returns>
    protected virtual Assembly? LoadBetaAssembly(Assembly assembly) =>
        MaybeThrows(
            logger,
            "Loading beta assembly.",
            () =>
            {
                var betaRef =
                    (from reference in assembly.GetReferencedAssemblies()
                     where reference.Name == "Beta"
                     select reference).FirstOrDefault();

                if (betaRef != null)
                {
                    return _loadContext.LoadFromAssemblyName(betaRef);
                }

                logger.Error($"Could not find reference to Beta assembly in {assembly.FullName}");
                return null;
            },
            "Unable to load beta assembly: {0}");

    /// <summary>
    ///     Creates a new instance of the controller.
    /// </summary>
    /// <param name="typeName">The fully qualified name of the controller.</param>
    /// <param name="betaAssembly">The beta assembly to load the controller from.</param>
    /// <param name="args">The arguments to pass to the controller.</param>
    /// <returns>An instance to the controller.</returns>
    /// <exception cref="BetaEngineLoadFailedException">Thrown if unable to create the controller.</exception>
    protected virtual object? CreateController(string typeName, Assembly betaAssembly, object[] args) =>
        MaybeThrows(
            logger,
            "Creating controller instance.",
            () =>
            {
                var type = betaAssembly.DefinedTypes.FirstOrDefault(t => t.FullName == typeName);

                if (type == null)
                {
                    logger.Error($"Unable to find type {typeName} in Beta assembly.");
                    return null;
                }

                var instance = Activator.CreateInstance(type, args);

                if (instance != null)
                {
                    return instance;
                }

                logger.Error("Instantiation of controller returned null.");
                return null;
            },
            "Unable to create controller instance: {0}");

    /// <summary>
    ///     Utility method to execute code and trap/log exceptions.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="message">The debug message to log before performing the action.</param>
    /// <param name="getValue">The action to perform.</param>
    /// <param name="formattedMessage">A formatted error message if an exception is thrown.</param>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <returns>The result of the action.</returns>
    /// <exception cref="BetaEngineLoadFailedException">Wraps any exception that is thrown by the block.</exception>
    internal static T? MaybeThrows<T>(ITestLogger logger, string message, Func<T> getValue, string formattedMessage)
        where T : class?
    {
        logger.Debug(message);

        try
        {
            return getValue();
        }
        catch (Exception ex)
        {
            var errorMessage = string.Format(formattedMessage, ex.Message);
            logger.Error(errorMessage);
            return null;
        }
    }
}
