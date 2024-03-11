// ***********************************************************************
// Copyright (c) 2014-2021 Charlie Poole, 2014-2022 Terje Sandstrom
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System.Xml.Linq;
using Beta.Engine;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using static Beta.TestAdapter.Internal.XContainerExtensions;

namespace Beta.TestAdapter;

public enum VsTestCategoryType
{
    NUnit,
    MsTest
}

public enum DisplayNameOptions
{
    Name,
    FullName,
    FullNameSep
}

public enum DiscoveryMethod
{
    Legacy,
    Current
}

public enum ExplicitModeEnum
{
    Strict,
    Relaxed,
    None
}

public enum OutputXmlFolderMode
{
    UseResultDirectory,
    RelativeToResultDirectory,
    RelativeToWorkFolder,
    AsSpecified
}

public class AdapterSettings : IAdapterSettings
{
    private const string RandomSeedFile = "beta_random_seed.tmp";

    public AdapterSettings(IDiscoveryContext? context)
        : this(context?.RunSettings?.SettingsXml)
    {
    }

    public AdapterSettings(string? settingsXml)
    {
        settingsXml = string.IsNullOrWhiteSpace(settingsXml) ? "<RunSettings />" : settingsXml;

        // Visual Studio already gives a good error message if the .runsettings
        // file is poorly formed, so we don't need to do anything more.
        var doc = XDocument.Parse(settingsXml);

        RunConfiguration = RunConfigurationSettings.Parse(doc.Root?.Element("RunConfiguration"));
        BetaConfiguration = BetaConfigurationSettings.Parse(doc.Root, RunConfiguration);
        BetaDiagnosticConfiguration = BetaDiagnosticConfigurationSettings.Parse(doc.Root, BetaConfiguration);
        TestProperties = LoadTestProperties(doc.Root);
    }

    public BetaDiagnosticConfigurationSettings BetaDiagnosticConfiguration { get; }

    public BetaConfigurationSettings BetaConfiguration { get; }

    public RunConfigurationSettings RunConfiguration { get; }

    public IDictionary<string, string> TestProperties { get; }

    public string? SetTestOutputFolder(string workDirectory)
    {
        if (!BetaConfiguration.UseTestOutputXml)
        {
            return "";
        }

        var resultsDir = RunConfiguration.ResultsDirectory ?? string.Empty;
        var testOutputXml = BetaConfiguration.TestOutputXml ?? string.Empty;

        switch (BetaConfiguration.OutputXmlFolderMode)
        {
            case OutputXmlFolderMode.UseResultDirectory:
                BetaConfiguration.TestOutputFolder = resultsDir;
                break;
            case OutputXmlFolderMode.RelativeToResultDirectory:
                BetaConfiguration.TestOutputFolder = Path.Combine(resultsDir, testOutputXml);
                break;
            case OutputXmlFolderMode.RelativeToWorkFolder:
                BetaConfiguration.TestOutputFolder = Path.Combine(workDirectory, testOutputXml);
                break;
            case OutputXmlFolderMode.AsSpecified:
                BetaConfiguration.TestOutputFolder = testOutputXml;
                break;
            default:
                return "";
        }

        return BetaConfiguration.TestOutputFolder;
    }

    public void SaveRandomSeed(string dirname)
    {
        try
        {
            var path = Path.Combine(dirname, RandomSeedFile);
            File.WriteAllText(path, BetaConfiguration.RandomSeed.ToString());
        }
        catch (Exception)
        {
            // _logger.Warning("Failed to save random seed.", ex);
        }
    }

    public void RestoreRandomSeed(string dirname)
    {
        var fullPath = Path.Combine(dirname, RandomSeedFile);
        if (!File.Exists(fullPath))
        {
            return;
        }

        try
        {
            var value = File.ReadAllText(fullPath);
            BetaConfiguration.RandomSeed = int.Parse(value);
        }
        catch (Exception)
        {
            // _logger.Warning("Unable to restore random seed.", ex);
        }
    }

    private IDictionary<string, string> LoadTestProperties(XContainer? container)
    {
        var testProperties = new Dictionary<string, string>();

        if (container == null)
        {
            return testProperties;
        }

        foreach (var node in container.Element("TestRunParameters")?.Elements("Parameter") ?? [])
        {
            var key = node.Attribute("name")?.Value;
            var value = node.Attribute("value")?.Value;

            if (key != null && value != null)
            {
                testProperties.Add(key, value);
            }
        }

        return testProperties;
    }
}

public record BetaConfigurationSettings
{
    public InternalTraceLevel TraceLevel { get; private init; }

    public string? WorkDirectory { get; init; }

    public string? Where { get; init; }
    public string? TestOutputXml { get; init; }
    public string? TestOutputXmlFileName { get; init; }

    public bool UseTestOutputXml => !string.IsNullOrEmpty(TestOutputXml) ||
                                    OutputXmlFolderMode == OutputXmlFolderMode.UseResultDirectory;

    public OutputXmlFolderMode OutputXmlFolderMode { get; init; } = OutputXmlFolderMode.RelativeToWorkFolder;

    public string? TestOutputFolder { get; set; }

    public bool NewOutputXmlFileForEachRun { get; init; }
    public int DefaultTimeout { get; init; }

    public int NumberOfTestWorkers { get; init; }

    public bool ShadowCopyFiles { get; init; }

    public int Verbosity { get; init; }

    public bool UseVsKeepEngineRunning { get; init; }

    public string? BasePath { get; init; }

    public string? PrivateBinPath { get; init; }

    public int? RandomSeed { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool RandomSeedSpecified { get; init; }

    public bool InProcDataCollectorsAvailable { get; init; }

    public bool SynchronousEvents { get; init; }

    public string? DomainUsage { get; init; }

    public bool ShowInternalProperties { get; init; }

    // ReSharper disable once InconsistentNaming
    public bool UseParentFQNForParametrizedTests { get; init; }

    public bool UseNUnitIdforTestCaseId { get; init; }
    public int ConsoleOut { get; init; }
    public bool StopOnError { get; init; }

    public DiscoveryMethod DiscoveryMethod { get; init; }
    public bool SkipNonTestAssemblies { get; init; }
    public int AssemblySelectLimit { get; init; }
    public bool UseNUnitFilter { get; init; }
    public bool IncludeStackTraceForSuites { get; init; }

    public VsTestCategoryType VsTestCategoryType { get; init; }

    public string? DefaultTestNamePattern { get; init; }

    public bool PreFilter { get; init; }

    public TestOutcome MapWarningTo { get; init; }

    public bool UseTestNameInConsoleOutput { get; init; }

    public DisplayNameOptions DisplayName { get; init; }

    public char FullnameSeparator { get; init; }

    public ExplicitModeEnum ExplicitMode { get; init; }
    public bool SkipExecutionWhenNoTests { get; init; }
    public bool EnsureAttachmentFileScheme { get; private set; }

    public static BetaConfigurationSettings Parse(XContainer? container, RunConfigurationSettings runSettings)
    {
        if (container == null)
        {
            return new BetaConfigurationSettings
            {
                OutputXmlFolderMode = OutputXmlFolderMode.RelativeToWorkFolder,
                TestOutputFolder = string.Empty,
                NumberOfTestWorkers = -1,
                RandomSeed = new Random().Next(),
                ConsoleOut = 2,
                DiscoveryMethod = DiscoveryMethod.Current,
                SkipNonTestAssemblies = true,
                AssemblySelectLimit = 2000,
                IncludeStackTraceForSuites = true,
                VsTestCategoryType = VsTestCategoryType.NUnit,
                DisplayName = DisplayNameOptions.Name,
                FullnameSeparator = ':',
                ExplicitMode = ExplicitModeEnum.Strict
            };
        }

        var nunitElement = container.Element("NUnit");
        var inProcDataCollectorElement =
            container.Element("InProcDataCollectionRunSettings")?.Element("InProcDataCollectors");

        var settings = new BetaConfigurationSettings
        {
            TraceLevel = ParseEnum(nunitElement, "InternalTraceLevel", InternalTraceLevel.Off),

            Verbosity = ParseInt(nunitElement, nameof(Verbosity), 0),
            WorkDirectory = ParseString(nunitElement, nameof(WorkDirectory)),
            Where = ParseString(nunitElement, nameof(Where)),
            DefaultTimeout = ParseInt(nunitElement, nameof(DefaultTimeout), 0),
            ShadowCopyFiles = ParseBool(nunitElement, nameof(ShadowCopyFiles), false),
            UseVsKeepEngineRunning = ParseBool(nunitElement, nameof(UseVsKeepEngineRunning), false),
            BasePath = ParseString(nunitElement, nameof(BasePath)),
            PrivateBinPath = ParseString(nunitElement, nameof(PrivateBinPath)),
            TestOutputXml = ParseString(nunitElement, nameof(TestOutputXml)),
            TestOutputXmlFileName = ParseString(nunitElement, nameof(TestOutputXmlFileName)),
            NewOutputXmlFileForEachRun = ParseBool(nunitElement, nameof(NewOutputXmlFileForEachRun), false),

            RandomSeed = ParseNullableInt(nunitElement, nameof(RandomSeed)) ?? new Random().Next(),
            RandomSeedSpecified = ParseNullableInt(nunitElement, nameof(RandomSeed)).HasValue,

            DefaultTestNamePattern = ParseString(nunitElement, nameof(DefaultTestNamePattern)),
            ShowInternalProperties = ParseBool(nunitElement, nameof(ShowInternalProperties), false),
            UseParentFQNForParametrizedTests = ParseBool(nunitElement, nameof(UseParentFQNForParametrizedTests), false),
            UseNUnitIdforTestCaseId = ParseBool(nunitElement, nameof(UseNUnitIdforTestCaseId), false),
            ConsoleOut = ParseInt(nunitElement, nameof(ConsoleOut), 2),
            StopOnError = ParseBool(nunitElement, nameof(StopOnError), false),
            UseNUnitFilter = ParseBool(nunitElement, nameof(UseNUnitFilter), true),
            IncludeStackTraceForSuites = ParseBool(nunitElement, nameof(IncludeStackTraceForSuites), true),
            EnsureAttachmentFileScheme = ParseBool(nunitElement, nameof(EnsureAttachmentFileScheme), false),
            SkipExecutionWhenNoTests = ParseBool(nunitElement, nameof(SkipExecutionWhenNoTests), false),

            DiscoveryMethod = ParseEnum(nunitElement, nameof(DiscoveryMethod), DiscoveryMethod.Current),
            SkipNonTestAssemblies = ParseBool(nunitElement, nameof(SkipNonTestAssemblies), true),
            AssemblySelectLimit = ParseInt(nunitElement, nameof(AssemblySelectLimit), 2000),
            ExplicitMode = ParseEnum(nunitElement, nameof(ExplicitMode), ExplicitModeEnum.Strict),

            DisplayName = ParseEnum(nunitElement, nameof(DisplayName), DisplayNameOptions.Name),
            FullnameSeparator = ParseChar(nunitElement, nameof(FullnameSeparator), ':'),
            PreFilter = ParseBool(nunitElement, nameof(PreFilter), false),
            VsTestCategoryType = ParseEnum(nunitElement, nameof(VsTestCategoryType), VsTestCategoryType.NUnit),
            MapWarningTo = ParseEnum(nunitElement, nameof(MapWarningTo), TestOutcome.Skipped),
            UseTestNameInConsoleOutput = ParseBool(nunitElement, nameof(UseTestNameInConsoleOutput), false),

            InProcDataCollectorsAvailable = CountElements(inProcDataCollectorElement, "InProcDataCollector") > 0,

            DomainUsage = runSettings.DisableAppDomain ? "None" : string.Empty
        };

        return settings with
        {
            OutputXmlFolderMode = ParseOutputXmlFolderMode(nunitElement, settings),
            NumberOfTestWorkers = ParseNumTestWorkers(container, runSettings),
            SynchronousEvents = ParseSynchronousEvents(inProcDataCollectorElement, runSettings)
        };
    }

    private static int ParseNumTestWorkers(XContainer? container, RunConfigurationSettings runSettings)
    {
        var numTestWorkers = ParseInt(container?.Element("NUnit"), nameof(NumberOfTestWorkers), -1);
        var hasLiveUnitTestingDataCollector =
            container
                ?.Element("InProcDataCollectionRunSettings")
                ?.Element("InProcDataCollectors")
                ?.Elements("InProcDataCollector")
                .Any(e => e.Attribute("uri")?.Value == "InProcDataCollector://Microsoft/LiveUnitTesting/1.0") ?? false;

        if (runSettings.CollectDataForEachTestSeparately || hasLiveUnitTestingDataCollector)
        {
            numTestWorkers = 0;
        }

        switch (runSettings.DisableParallelization)
        {
            case true when numTestWorkers < 0:
            case true when numTestWorkers > 0:
                return 0;
            default:
                return numTestWorkers;
        }
    }

    private static bool ParseSynchronousEvents(XContainer? container, RunConfigurationSettings runSettings)
    {
        if (container == null)
        {
            return false;
        }

        var hasLiveUnitTestingDataCollector =
            container
                .Elements("InProcDataCollector")
                .Any(e => e.Attribute("uri")?.Value == "InProcDataCollector://Microsoft/LiveUnitTesting/1.0");

        return runSettings.CollectDataForEachTestSeparately || hasLiveUnitTestingDataCollector;
    }

    private static OutputXmlFolderMode ParseOutputXmlFolderMode(XContainer? container,
                                                                BetaConfigurationSettings betaSettings)
    {
        if (Path.IsPathRooted(betaSettings.TestOutputXml))
        {
            return OutputXmlFolderMode.AsSpecified;
        }

        var outputMode = ParseString(container, nameof(OutputXmlFolderMode));

        return Enum.TryParse<OutputXmlFolderMode>(outputMode, true, out var result)
            ? result
            : OutputXmlFolderMode.RelativeToWorkFolder;
    }
}

public record BetaDiagnosticConfigurationSettings
{
    public bool DumpXmlTestDiscovery { get; init; }
    public bool DumpXmlTestResults { get; init; }
    public bool DumpVsInput { get; init; }
    public bool FreakMode { get; init; }
    public bool Debug { get; init; }
    public bool DebugExecution { get; init; }
    public bool DebugDiscovery { get; init; }

    public static BetaDiagnosticConfigurationSettings Parse(XContainer? container,
                                                            BetaConfigurationSettings betaSettings)
    {
        if (container == null)
        {
            return new BetaDiagnosticConfigurationSettings();
        }

        var isDebug = ParseBool(container, nameof(Debug), false);

        return new BetaDiagnosticConfigurationSettings
        {
            DumpXmlTestDiscovery = ParseBool(container, nameof(DumpXmlTestDiscovery), false),
            DumpXmlTestResults = ParseBool(container, nameof(DumpXmlTestResults), false),
            DumpVsInput = ParseBool(container, nameof(DumpVsInput), false),
            FreakMode = ParseBool(container, nameof(FreakMode), false),
            Debug = isDebug,
            DebugExecution = isDebug || ParseBool(container, nameof(DebugExecution), false),
            DebugDiscovery = isDebug || ParseBool(container, nameof(DebugDiscovery), false)
        };
    }
}

public record RunConfigurationSettings
{
    public int MaxCpuCount { get; init; }

    public string? ResultsDirectory { get; init; }

    public string? TargetPlatform { get; init; }

    public string? TargetFrameworkVersion { get; init; }

    public string? TestAdapterPaths { get; init; }

    public bool CollectSourceInformation { get; init; }

    public bool DisableAppDomain { get; init; }

    public bool DisableParallelization { get; init; }

    public bool AllowParallelWithDebugger { get; init; }

    public bool DesignMode { get; init; }

    public bool CollectDataForEachTestSeparately { get; init; }

    /// <summary>
    ///     Parses the general settings from an XML fragment.
    /// </summary>
    /// <param name="container">The XML fragment to parse.</param>
    /// <returns>The parsed general settings.</returns>
    public static RunConfigurationSettings Parse(XContainer? container)
    {
        if (container == null)
        {
            return new RunConfigurationSettings
            {
                MaxCpuCount = -1,
                CollectSourceInformation = true
            };
        }

        return new RunConfigurationSettings
        {
            MaxCpuCount = ParseInt(container, nameof(MaxCpuCount), -1),
            ResultsDirectory = ParseString(container, nameof(ResultsDirectory)),
            TargetPlatform = ParseString(container, nameof(TargetPlatform)),
            TargetFrameworkVersion = ParseString(container, nameof(TargetFrameworkVersion)),
            TestAdapterPaths = ParseString(container, nameof(TestAdapterPaths)),
            CollectSourceInformation = ParseBool(container, nameof(CollectSourceInformation), true),
            DisableAppDomain = ParseBool(container, nameof(DisableAppDomain), false),
            DisableParallelization = ParseBool(container, nameof(DisableParallelization), false),
            AllowParallelWithDebugger = ParseBool(container, nameof(AllowParallelWithDebugger), false),
            DesignMode = ParseBool(container, nameof(DesignMode), false),
            CollectDataForEachTestSeparately = ParseBool(container, nameof(CollectDataForEachTestSeparately), false)
        };
    }
}
