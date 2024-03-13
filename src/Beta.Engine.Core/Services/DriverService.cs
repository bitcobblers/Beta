using System.Reflection;
using Beta.Common;
using Beta.Engine.Api.Extensibility;
using Beta.Engine.Core;
using Beta.Engine.Drivers;
using Beta.Engine.Extensibility;
using Beta.Engine.Internal;
using TestCentric.Metadata;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Services;

public class DriverService : Service, IDriverService
{
    private static readonly ILogger log = InternalTrace.GetLogger("DriverService");

    private readonly IList<IDriverFactory> _factories = new List<IDriverFactory>();

    /// <summary>
    ///     Get a driver suitable for use with a particular test assembly.
    /// </summary>
    /// <param name="domain">The application domain to use for the tests</param>
    /// <param name="assemblyPath">The full path to the test assembly</param>
    /// <param name="targetFramework">The value of any TargetFrameworkAttribute on the assembly, or null</param>
    /// <param name="skipNonTestAssemblies">True if non-test assemblies should simply be skipped rather than reporting an error</param>
    /// <returns></returns>
    public IFrameworkDriver GetDriver(AppDomain domain, string assemblyPath, string targetFramework,
                                      bool skipNonTestAssemblies)
    {
        if (!File.Exists(assemblyPath))
        {
            return new InvalidAssemblyFrameworkDriver(assemblyPath, "File not found: " + assemblyPath);
        }

        if (!PathUtils.IsAssemblyFileType(assemblyPath))
        {
            return new InvalidAssemblyFrameworkDriver(assemblyPath, "File type is not supported");
        }

        if (targetFramework != null)
        {
            // This takes care of an issue with Roslyn. It may get fixed, but we still
            // have to deal with assemblies having this setting. I'm assuming that
            // any true Portable assembly would have a Profile as part of its name.
            var platform = targetFramework == ".NETPortable,Version=v5.0"
                ? ".NETStandard"
                : targetFramework.Split(',')[0];

            if (platform == "Silverlight" || platform == ".NETPortable" || platform == ".NETStandard" ||
                platform == ".NETCompactFramework")
            {
                return new InvalidAssemblyFrameworkDriver(assemblyPath,
                    platform + " test assemblies are not supported by this version of the engine");
            }
        }

        try
        {
            using (var assemblyDef = AssemblyDefinition.ReadAssembly(assemblyPath))
            {
                if (skipNonTestAssemblies)
                {
                    foreach (var attr in assemblyDef.CustomAttributes)
                    {
                        if (attr.AttributeType.FullName == "NUnit.Framework.NonTestAssemblyAttribute")
                        {
                            return new SkippedAssemblyFrameworkDriver(assemblyPath);
                        }
                    }
                }

                var references = new List<AssemblyName>();
                foreach (var cecilRef in assemblyDef.MainModule.AssemblyReferences)
                {
                    references.Add(new AssemblyName(cecilRef.FullName));
                }

                foreach (var factory in _factories)
                {
                    log.Debug($"Trying {factory.GetType().Name}");

                    foreach (var reference in references)
                    {
                        if (factory.IsSupportedTestFramework(reference))
                        {
                            return factory.GetDriver(reference);
                        }
                    }
                }
            }
        }
        catch (BadImageFormatException ex)
        {
            return new InvalidAssemblyFrameworkDriver(assemblyPath, ex.Message);
        }

        if (skipNonTestAssemblies)
        {
            return new SkippedAssemblyFrameworkDriver(assemblyPath);
        }

        return new InvalidAssemblyFrameworkDriver(assemblyPath, string.Format("No suitable tests found in '{0}'.\n" +
                                                                              "Either assembly contains no tests or proper test driver has not been found.",
            assemblyPath));
    }

    public override void StartService()
    {
        Guard.OperationValid(ServiceContext != null, "Can't start DriverService outside of a ServiceContext");

        try
        {
            var extensionService = ServiceContext.GetService<ExtensionService>();
            if (extensionService != null)
            {
                foreach (var factory in extensionService.GetExtensions<IDriverFactory>())
                {
                    _factories.Add(factory);
                }
            }

            _factories.Add(new NUnit3DriverFactory());

            Status = ServiceStatus.Started;
        }
        catch (Exception)
        {
            Status = ServiceStatus.Error;
            throw;
        }
    }
}
