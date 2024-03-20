using System.Reflection;
using System.Runtime.Loader;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Beta.TestAdapter;

public class BetaEngineLoadFailedException(string message, Exception innerException)
    : Exception(message, innerException);

public class BetaReferenceNotFoundException(string message)
    : Exception(message);

public class BetaControllerInstantiationFailed(string message)
    : Exception(message);

public class BetaControllerNotFoundException(string message)
    : Exception(message);

/// <summary>
///     Defines an adapter for calling the beta engine from the test assembly's referenced engine.
/// </summary>
/// <param name="logger">The internal logger to use.</param>
public class BetaEngineAdapter(ITestLogger logger)
{
    private readonly Assembly _betaAssembly;
    private readonly object _controllerInstance;
    private readonly Type _controllerType;
    private readonly AssemblyLoadContext _loadContext;
    private readonly Assembly _testAssembly;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BetaEngineAdapter" /> class.
    /// </summary>
    /// <param name="assemblyPath">The path to the test assembly.</param>
    /// <param name="logger">The internal logger to use.</param>
    public BetaEngineAdapter(string assemblyPath, ITestLogger logger)
        : this(logger)
    {
        assemblyPath = Path.GetFullPath(assemblyPath);
        _loadContext = new CustomAssemblyLoadContext(assemblyPath);

        _loadContext.Resolving += (context, assemblyName) =>
        {
            var customLoadContext = context as CustomAssemblyLoadContext;
            return customLoadContext?.LoadFallback(assemblyName);
        };

        _testAssembly = LoadTestAssembly(assemblyPath);
        _betaAssembly = LoadBetaAssembly(_testAssembly);
        _controllerInstance = CreateController("Beta.Engine.BetaEngine", [_testAssembly]);
        _controllerType = _controllerInstance.GetType();
    }

    public IEnumerable<TestCase> Discover()
    {
        yield break;
    }

    public void Run()
    {
    }

    public void Stop()
    {
    }

    private Assembly LoadTestAssembly(string assemblyPath) =>
        MaybeThrows(
            logger,
            $"Loading test assembly: {assemblyPath}",
            () => _loadContext.LoadFromAssemblyPath(assemblyPath),
            "Unable to load test assembly: {0}");

    private Assembly LoadBetaAssembly(Assembly assembly) =>
        MaybeThrows(
            logger,
            "Loading beta assembly.",
            () =>
            {
                var betaRef =
                    (from reference in assembly.GetReferencedAssemblies()
                     where reference.Name == "Beta"
                     select reference).FirstOrDefault();

                if (betaRef == null)
                {
                    throw new BetaReferenceNotFoundException(
                        $"Could not find reference to Beta assembly in {assembly.FullName}");
                }

                return _loadContext.LoadFromAssemblyName(betaRef);
            },
            "Unable to load beta assembly: {0}");

    /// <summary>
    ///     Creates a new instance of the controller.
    /// </summary>
    /// <param name="typeName">The fully qualified name of the controller.</param>
    /// <param name="args">The arguments to pass to the controller.</param>
    /// <returns>An instance to the controller.</returns>
    /// <exception cref="BetaEngineLoadFailedException">Thrown if unable to create the controller.</exception>
    private object CreateController(string typeName, object[] args)
    {
        return MaybeThrows(
            logger,
            "Creating controller instance.",
            () =>
            {
                var type = _betaAssembly.DefinedTypes.FirstOrDefault(t => t.FullName == typeName);

                if (type == null)
                {
                    throw new BetaControllerNotFoundException("Unable to find type {typeName} in Beta assembly.");
                }

                var instance = Activator.CreateInstance(type, args);

                if (instance == null)
                {
                    throw new BetaControllerInstantiationFailed("Instantiation of controller returned null.");
                }

                return instance;
            },
            "Unable to create controller instance: {0}");
    }

    /// <summary>
    ///     Executes a method against the controller.
    /// </summary>
    /// <param name="methodName">The name of the method to execute.</param>
    /// <param name="args">The arguments to pass into the method.</param>
    /// <returns>The result of the method (if any).</returns>
    private object? Execute(string methodName, object[] args) =>
        MaybeThrows(
            logger,
            $"Executing method {methodName}.",
            () => _controllerType.GetMethod(methodName)?.Invoke(_controllerInstance, args),
            "Unable to execute method {methodName}: {0}");

    /// <summary>
    ///     Utility method to execute code and trap/wrap exceptions.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="message">The debug message to log before performing the action.</param>
    /// <param name="getValue">The action to perform.</param>
    /// <param name="formattedMessage">A formatted error message if an exception is thrown.</param>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <returns>The result of the action.</returns>
    /// <exception cref="BetaEngineLoadFailedException">Wraps any exception that is thrown by the block.</exception>
    internal static T MaybeThrows<T>(ITestLogger logger, string message, Func<T> getValue, string formattedMessage)
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
            throw new BetaEngineLoadFailedException(errorMessage, ex);
        }
    }

    /// <summary>
    ///     Defines the methods that can be called on the controller.
    /// </summary>
    private static class ControllerMethods
    {
        /// <summary>
        ///     Performs discovery of tests in the assembly.
        /// </summary>
        public const string Discover = "Discover";

        /// <summary>
        ///     Runs the tests in the assembly.
        /// </summary>
        public const string Run = "Run";

        /// <summary>
        ///     Stops the current test execution.
        /// </summary>
        public const string Stop = "Stop";
    }
}
