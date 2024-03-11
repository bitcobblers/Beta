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
    private const string RANDOM_SEED_FILE = "beta_random_seed.tmp";
    private readonly ITestLogger _logger;

    #region Constructor

    public AdapterSettings(ITestLogger logger)
    {
        _logger = logger;
    }

    #endregion

    public BetaDiagnosticConfigurationSettings BetaDiagnosticConfiguration { get; }

    public BetaConfigurationSettings BetaConfiguration { get; private set; }

    public RunConfigurationSettings RunConfiguration { get; private set; }

    public IDictionary<string, string> TestProperties { get; private set; }

    #region Public Methods

    public void Load(IDiscoveryContext context, TestLogger testLogger)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context), "Load called with null context");
        }

        Load(context.RunSettings?.SettingsXml);
    }

    public void Load(string settingsXml)
    {
        if (string.IsNullOrEmpty(settingsXml))
        {
            settingsXml = "<RunSettings />";
        }

        // Visual Studio already gives a good error message if the .runsettings
        // file is poorly formed, so we don't need to do anything more.
        var doc = XDocument.Parse(settingsXml);

        RunConfiguration = RunConfigurationSettings.Parse(doc.Root?.Element("RunConfiguration"));
        BetaConfiguration = BetaConfigurationSettings.Parse(doc.Root, RunConfiguration);
        TestProperties = UpdateTestProperties(doc.Root);

        _logger.Verbosity = BetaConfiguration.Verbosity;
    }

    public string SetTestOutputFolder(string workDirectory)
    {
        if (!BetaConfiguration.UseTestOutputXml)
        {
            return "";
        }

        switch (BetaConfiguration.OutputXmlFolderMode)
        {
            case OutputXmlFolderMode.UseResultDirectory:
                BetaConfiguration.TestOutputFolder = RunConfiguration?.ResultsDirectory;
                return BetaConfiguration.TestOutputFolder;
            case OutputXmlFolderMode.RelativeToResultDirectory:
                BetaConfiguration.TestOutputFolder =
                    Path.Combine(RunConfiguration?.ResultsDirectory, BetaConfiguration.TestOutputXml);
                return BetaConfiguration.TestOutputFolder;
            case OutputXmlFolderMode.RelativeToWorkFolder:
                BetaConfiguration.TestOutputFolder = Path.Combine(workDirectory, BetaConfiguration.TestOutputXml);
                return BetaConfiguration.TestOutputFolder;
            case OutputXmlFolderMode.AsSpecified:
                BetaConfiguration.TestOutputFolder = BetaConfiguration.TestOutputXml;
                return BetaConfiguration.TestOutputFolder;
            default:
                return "";
        }
    }

    private IDictionary<string, string> UpdateTestProperties(XContainer? container)
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

    public void SaveRandomSeed(string dirname)
    {
        try
        {
            var path = Path.Combine(dirname, RANDOM_SEED_FILE);
            File.WriteAllText(path, BetaConfiguration.RandomSeed.Value.ToString());
        }
        catch (Exception ex)
        {
            _logger.Warning("Failed to save random seed.", ex);
        }
    }

    public void RestoreRandomSeed(string dirname)
    {
        var fullPath = Path.Combine(dirname, RANDOM_SEED_FILE);
        if (!File.Exists(fullPath))
        {
            return;
        }

        try
        {
            var value = File.ReadAllText(fullPath);
            BetaConfiguration.RandomSeed = int.Parse(value);
        }
        catch (Exception ex)
        {
            _logger.Warning("Unable to restore random seed.", ex);
        }
    }

    #endregion
}

public record BetaConfigurationSettings
{
    public InternalTraceLevel TraceLevel { get; set; }

    public string WorkDirectory { get; init; }

    public string Where { get; init; }
    public string TestOutputXml { get; init; }
    public string TestOutputXmlFileName { get; init; }

    public bool UseTestOutputXml => !string.IsNullOrEmpty(TestOutputXml) ||
                                    OutputXmlFolderMode == OutputXmlFolderMode.UseResultDirectory;

    public OutputXmlFolderMode OutputXmlFolderMode { get; init; } = OutputXmlFolderMode.RelativeToWorkFolder;

    public string TestOutputFolder { get; set; } = "";

    public bool NewOutputXmlFileForEachRun { get; init; }
    public int DefaultTimeout { get; init; }

    public int NumberOfTestWorkers { get; init; } = -1;

    public bool ShadowCopyFiles { get; init; }

    public int Verbosity { get; init; }

    public bool UseVsKeepEngineRunning { get; init; }

    public string BasePath { get; init; }

    public string PrivateBinPath { get; init; }

    public int? RandomSeed { get; set; } = new Random().Next();
    public bool RandomSeedSpecified { get; init; }

    public bool InProcDataCollectorsAvailable { get; init; }

    public bool SynchronousEvents { get; init; }

    public string DomainUsage { get; init; }

    public bool ShowInternalProperties { get; init; }

    public bool UseParentFQNForParametrizedTests { get; init; }

    public bool UseNUnitIdforTestCaseId { get; init; } // default is false.
    public int ConsoleOut { get; init; } = 2;
    public bool StopOnError { get; init; }

    public DiscoveryMethod DiscoveryMethod { get; init; } = DiscoveryMethod.Current;
    public bool SkipNonTestAssemblies { get; init; } = true;
    public int AssemblySelectLimit { get; init; } = 2000;
    public bool UseNUnitFilter { get; init; } = true;
    public bool IncludeStackTraceForSuites { get; init; } = true;

    public VsTestCategoryType VsTestCategoryType { get; init; } = VsTestCategoryType.NUnit;

    public string DefaultTestNamePattern { get; set; }

    public bool PreFilter { get; init; }

    public TestOutcome MapWarningTo { get; init; }

    public bool UseTestNameInConsoleOutput { get; init; }

    public DisplayNameOptions DisplayName { get; init; } = DisplayNameOptions.Name;

    public char FullnameSeparator { get; init; } = ':';

    public ExplicitModeEnum ExplicitMode { get; init; } = ExplicitModeEnum.Strict;
    public bool SkipExecutionWhenNoTests { get; init; }
    public bool EnsureAttachmentFileScheme { get; private set; }

    public static BetaConfigurationSettings Parse(XContainer? container, RunConfigurationSettings runSettings)
    {
        if (container == null)
        {
            return new BetaConfigurationSettings();
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
            //NumberOfTestWorkers = ParseInt(container, nameof(NumberOfTestWorkers), -1),
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
            FullnameSeparator = ParseString(nunitElement, nameof(FullnameSeparator), ":")[0],
            PreFilter = ParseBool(nunitElement, nameof(PreFilter), false),
            VsTestCategoryType = ParseEnum(nunitElement, nameof(VsTestCategoryType), VsTestCategoryType.NUnit),
            MapWarningTo = ParseEnum(nunitElement, nameof(MapWarningTo), TestOutcome.Skipped),
            UseTestNameInConsoleOutput = ParseBool(nunitElement, nameof(UseTestNameInConsoleOutput), false),

            InProcDataCollectorsAvailable =
                CountElements(inProcDataCollectorElement, "InProcDataCollector") > 0,

            DomainUsage = runSettings.DisableAppDomain ? "None" : null
        };

        return settings with
        {
            OutputXmlFolderMode = ParseOutputXmlFolderMode(nunitElement, settings),
            NumberOfTestWorkers = ParseNumTestWorkers(container, settings, runSettings),
            SynchronousEvents = ParseSynchronousEvents(inProcDataCollectorElement, runSettings)
        };
    }

    private static int ParseNumTestWorkers(XContainer? container, BetaConfigurationSettings betaSettings,
                                           RunConfigurationSettings runSettings)
    {
        var numTestWorkers = ParseInt(container?.Element("NUnit"), nameof(NumberOfTestWorkers), -1);
        var hasLiveUnitTestingDataCollector =
            container
                ?.Element("InProcDataCollectionRunSettings")
                ?.Element("InProcDataCollectors")
                ?.Elements("InProcDataCollector")
                ?.Any(e => e.Attribute("uri")?.Value == "InProcDataCollector://Microsoft/LiveUnitTesting/1.0") ?? false;

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
    public int MaxCpuCount { get; init; } = -1;

    public string? ResultsDirectory { get; init; }

    public string? TargetPlatform { get; init; }

    public string? TargetFrameworkVersion { get; init; }

    public string? TestAdapterPaths { get; init; }

    public bool CollectSourceInformation { get; init; } = true;

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
            return new RunConfigurationSettings();
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
