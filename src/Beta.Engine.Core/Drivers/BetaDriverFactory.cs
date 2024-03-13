using System.Reflection;
using Beta.Common;
using Beta.Engine.Api.Extensibility;
using Beta.Engine.Extensibility;
using Beta.Engine.Internal;

namespace Beta.Engine.Drivers;

public class NUnit3DriverFactory : IDriverFactory
{
    private const string NUNIT_FRAMEWORK = "nunit.framework";
    private static readonly ILogger log = InternalTrace.GetLogger(typeof(NUnit3DriverFactory));

    /// <summary>
    ///     Gets a flag indicating whether a given assembly name and version
    ///     represent a test framework supported by this factory.
    /// </summary>
    /// <param name="reference">An AssemblyName referring to the possible test framework.</param>
    public bool IsSupportedTestFramework(AssemblyName reference)
    {
        return NUNIT_FRAMEWORK.Equals(reference.Name, StringComparison.OrdinalIgnoreCase) &&
               reference.Version.Major >= 3;
    }

    /// <summary>
    ///     Gets a driver for a given test assembly and a framework
    ///     which the assembly is already known to reference.
    /// </summary>
    /// <param name="reference">An AssemblyName referring to the test framework.</param>
    /// <returns></returns>
    public IFrameworkDriver GetDriver(AssemblyName reference)
    {
        Guard.ArgumentValid(IsSupportedTestFramework(reference), "Invalid framework", "reference");
#if NETSTANDARD
        log.Info("Using NUnitNetStandardDriver");
        return new NUnitNetStandardDriver();
#else
            log.Info("Using NUnitNetCore31Driver");
            return new NUnitNetCore31Driver();
#endif
    }
}
