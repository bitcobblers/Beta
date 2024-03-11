using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.TestAdapter;

public class BetaTestAdapter
{
    protected ITestLogger Logger = new TestLogger();
    protected IAdapterSettings Settings = new AdapterSettings();

    private List<string> ForbiddenFolders { get; set; }

    protected void Initialize(IDiscoveryContext? context, IMessageLogger? logger)
    {
        Logger = new TestLogger(logger);

        try
        {
            Settings = new AdapterSettings(context);
            ForbiddenFolders = InitializeForbiddenFolders().ToList();
            SetCurrentWorkingDirectory();
        }
        catch (Exception ex)
        {
            Logger.Warning("Error initializing RunSettings.  The default settings will be used.", ex);
        }
    }

    protected void PrintBanner(RunSettings settings)
    {
        Logger.Info($"Target Framework Version: {settings.TargetFrameworkVersion}");
    }

    private static IEnumerable<string> InitializeForbiddenFolders()
    {
        return new[]
               {
                   Environment.GetEnvironmentVariable("ProgramW6432"),
                   Environment.GetEnvironmentVariable("ProgramFiles(x86)"),
                   Environment.GetEnvironmentVariable("windir")
               }
               .Where(folder => folder != null)
               .Where(o => !string.IsNullOrEmpty(o))
               .Select(o => o?.ToLower() + @"\");
    }

    private void SetCurrentWorkingDirectory()
    {
        var dir = Directory.GetCurrentDirectory();
        var foundForbiddenFolder = CheckDirectory(dir);

        if (foundForbiddenFolder)
        {
            Directory.SetCurrentDirectory(Path.GetTempPath());
        }
    }

    /// <summary>
    ///     If a directory matches one of the forbidden folders, then we should reroute, so we return true in that case.
    /// </summary>
    private bool CheckDirectory(string dir)
    {
        var checkDir = dir.EndsWith("\\") ? dir : dir + "\\";
        return ForbiddenFolders.Any(o => checkDir.StartsWith(o, StringComparison.OrdinalIgnoreCase));
    }


    // protected IEnumerable<TestCase> RunDiscovery(
    //     IEnumerable<string>? sources,
    //     RunSettings settings)
    // {
    //     yield break;
    //
    //     var discoverer = new TestDiscoverer(logger);
    //     using var diaSessionManager = new DiaSessionManager();
    //
    //     if (sources == null)
    //     {
    //         yield break;
    //     }
    //
    //     foreach (var betaTest in discoverer.DiscoverTests(sources, settings.TargetFrameworkVersion))
    //     {
    //         var session = diaSessionManager.GetSession(betaTest.Assembly.Location);
    //         var navInfo = session.GetNavigationData(betaTest.Method);
    //         var fullyQualifiedName = betaTest.DeclaringType + "." + betaTest.MethodName;
    //
    //         if (betaTest.Input is not null)
    //         {
    //             foreach (var input in betaTest.Input)
    //             {
    //                 var options = new JsonSerializerSettings
    //                 {
    //                     TypeNameHandling = TypeNameHandling.All
    //                 };
    //
    //                 var testCase = new TestCase
    //                 {
    //                     Id = Guid.NewGuid(),
    //                     CodeFilePath = navInfo?.FileName,
    //                     DisplayName = $"{betaTest.TestName}({input})",
    //                     ExecutorUri = new Uri(BetaTestExecutor.ExecutorUri),
    //                     FullyQualifiedName = fullyQualifiedName,
    //                     Source = betaTest.Assembly.Location,
    //                     LineNumber = navInfo?.MinLineNumber ?? 0
    //                 };
    //
    //                 testCase.SetPropertyValue(TestCaseProperty, JsonConvert.SerializeObject(input, options));
    //                 testCase.SetPropertyValue(TestContainerProperty,
    //                     betaTest.Method?.DeclaringType?.AssemblyQualifiedName);
    //                 testCase.SetPropertyValue(TestMethodProper, betaTest.MethodName);
    //
    //                 yield return testCase;
    //             }
    //         }
    //         else
    //         {
    //             var testCase = new TestCase
    //             {
    //                 Id = Guid.NewGuid(),
    //                 CodeFilePath = navInfo?.FileName,
    //                 DisplayName = betaTest.TestName,
    //                 ExecutorUri = new Uri(BetaTestExecutor.ExecutorUri),
    //                 FullyQualifiedName = fullyQualifiedName,
    //                 Source = betaTest.Assembly.Location,
    //                 LineNumber = navInfo?.MinLineNumber ?? 0
    //             };
    //
    //             yield return testCase;
    //         }
    //     }
    // }
}
