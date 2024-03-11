using Beta.TestAdapter.Tests.Fakes;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Beta.TestAdapter.Tests;

public class AdapterSettingsTests
{
    private readonly IAdapterSettings _settings;

    public AdapterSettingsTests()
    {
        var testLogger = new TestLogger(new MessageLoggerStub());

        _settings = new AdapterSettings(testLogger);
    }

    [Fact]
    public void NullContextThrowsException()
    {
        Should.Throw<ArgumentNullException>(() => _settings.Load((IDiscoveryContext)null));
    }

    [InlineData(null)]
    [InlineData("")]
    [InlineData("<RunSettings />")]
    [Theory]
    public void DefaultSettings(string xml)
    {
        _settings.Load(xml);
        Assert.Multiple(() =>
        {
            _settings.RunConfiguration.MaxCpuCount.ShouldBe(-1);
            _settings.RunConfiguration.ResultsDirectory.ShouldBeNull();
            _settings.RunConfiguration.TargetFrameworkVersion.ShouldBeNull();
            _settings.RunConfiguration.TargetPlatform.ShouldBeNull();
            _settings.RunConfiguration.TestAdapterPaths.ShouldBeNull();
            _settings.RunConfiguration.CollectSourceInformation.ShouldBeTrue();
            _settings.TestProperties.ShouldBeEmpty();
            _settings.BetaConfiguration.TraceLevel.ShouldBe(Engine.InternalTraceLevel.Off);
            _settings.BetaConfiguration.WorkDirectory.ShouldBeNull();
            _settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(-1);
            _settings.BetaConfiguration.DefaultTimeout.ShouldBe(0);
            _settings.BetaConfiguration.Verbosity.ShouldBe(0);
            _settings.BetaConfiguration.ShadowCopyFiles.ShouldBeFalse();
            _settings.BetaConfiguration.UseVsKeepEngineRunning.ShouldBeFalse();
            _settings.BetaConfiguration.BasePath.ShouldBeNull();
            _settings.BetaConfiguration.PrivateBinPath.ShouldBeNull();
            _settings.BetaConfiguration.RandomSeed.ShouldNotBeNull();
            _settings.BetaConfiguration.SynchronousEvents.ShouldBeFalse();
            _settings.BetaConfiguration.DomainUsage.ShouldBeNull();
            _settings.BetaConfiguration.InProcDataCollectorsAvailable.ShouldBeFalse();
            _settings.RunConfiguration.DisableAppDomain.ShouldBeFalse();
            _settings.RunConfiguration.DisableParallelization.ShouldBeFalse();
            _settings.RunConfiguration.DesignMode.ShouldBeFalse();
            _settings.BetaConfiguration.UseTestOutputXml.ShouldBeFalse();
            _settings.BetaConfiguration.NewOutputXmlFileForEachRun.ShouldBeFalse();
            _settings.BetaConfiguration.PreFilter.ShouldBeFalse();
        });
    }

    [Fact]
    public void ResultsDirectorySetting()
    {
        _settings.Load(
            "<RunSettings><RunConfiguration><ResultsDirectory>./myresults</ResultsDirectory></RunConfiguration></RunSettings>");

        _settings.RunConfiguration.ResultsDirectory.ShouldBe("./myresults");
    }

    [Fact]
    public void MaxCpuCountSetting()
    {
        _settings.Load("<RunSettings><RunConfiguration><MaxCpuCount>42</MaxCpuCount></RunConfiguration></RunSettings>");

        _settings.RunConfiguration.MaxCpuCount.ShouldBe(42);
    }

    [Fact]
    public void TargetFrameworkVersionSetting()
    {
        _settings.Load(
            "<RunSettings><RunConfiguration><TargetFrameworkVersion>Framework45</TargetFrameworkVersion></RunConfiguration></RunSettings>");

        _settings.RunConfiguration.TargetFrameworkVersion.ShouldBe("Framework45");
    }

    [Fact]
    public void TargetPlatformSetting()
    {
        _settings.Load(
            "<RunSettings><RunConfiguration><TargetPlatform>x86</TargetPlatform></RunConfiguration></RunSettings>");

        _settings.RunConfiguration.TargetPlatform.ShouldBe("x86");
    }

    [Fact]
    public void TestAdapterPathsSetting()
    {
        _settings.Load(
            "<RunSettings><RunConfiguration><TestAdapterPaths>/first/path;/second/path</TestAdapterPaths></RunConfiguration></RunSettings>");

        _settings.RunConfiguration.TestAdapterPaths.ShouldBe("/first/path;/second/path");
    }

    [Fact]
    public void CollectSourceInformationSetting()
    {
        _settings.Load(
            "<RunSettings><RunConfiguration><CollectSourceInformation>False</CollectSourceInformation></RunConfiguration></RunSettings>");

        _settings.RunConfiguration.CollectSourceInformation.ShouldBeFalse();
    }

    [Fact]
    public void DisableAppDomainSetting()
    {
        _settings.Load(
            "<RunSettings><RunConfiguration><DisableAppDomain>true</DisableAppDomain></RunConfiguration></RunSettings>");

        _settings.RunConfiguration.DisableAppDomain.ShouldBeTrue();
    }

    [Fact]
    public void DisableParallelizationSetting()
    {
        _settings.Load(
            "<RunSettings><RunConfiguration><DisableParallelization>true</DisableParallelization></RunConfiguration></RunSettings>");

        _settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(0);
    }

    [Fact]
    public void UpdateNumberOfTestWorkersWhenConflictingSettings()
    {
        _settings.Load(
            "<RunSettings><RunConfiguration><DisableParallelization>true</DisableParallelization></RunConfiguration><NUnit><NumberOfTestWorkers>12</NumberOfTestWorkers></NUnit></RunSettings>");

        // When there's a conflicting values in DisableParallelization and NumberOfTestWorkers. Override the NumberOfTestWorkers.
        _settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(0);

        // Do not override the NumberOfTestWorkers when DisableParallelization is False
        _settings.Load(
            "<RunSettings><RunConfiguration><DisableParallelization>false</DisableParallelization></RunConfiguration><NUnit><NumberOfTestWorkers>0</NumberOfTestWorkers></NUnit></RunSettings>");
        _settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(0);

        // Do not override the NumberOfTestWorkers when DisableParallelization is not defined
        _settings.Load(
            "<RunSettings><RunConfiguration></RunConfiguration><NUnit><NumberOfTestWorkers>12</NumberOfTestWorkers></NUnit></RunSettings>");
        _settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(12);
    }

    [Fact]
    public void DesignModeSetting()
    {
        _settings.Load("<RunSettings><RunConfiguration><DesignMode>true</DesignMode></RunConfiguration></RunSettings>");

        _settings.RunConfiguration.DesignMode.ShouldBeTrue();
    }

    [Fact]
    public void TestRunParameterSettings()
    {
        _settings.Load(
            "<RunSettings><TestRunParameters><Parameter name='Answer' value='42'/><Parameter name='Question' value='Why?'/></TestRunParameters></RunSettings>");

        _settings.TestProperties.Count.ShouldBe(2);
        _settings.TestProperties["Answer"].ShouldBe("42");
        _settings.TestProperties["Question"].ShouldBe("Why?");
    }

    [Fact]
    public void InternalTraceLevel()
    {
        _settings.Load("<RunSettings><NUnit><InternalTraceLevel>Debug</InternalTraceLevel></NUnit></RunSettings>");

        _settings.BetaConfiguration.TraceLevel.ShouldBe(Engine.InternalTraceLevel.Debug);
    }

    [Fact]
    public void InternalTraceLevelEmpty()
    {
        _settings.Load("");

        _settings.BetaConfiguration.TraceLevel.ShouldBe(Engine.InternalTraceLevel.Off);
    }


    [Fact]
    public void WorkDirectorySetting()
    {
        _settings.Load("<RunSettings><NUnit><WorkDirectory>/my/work/dir</WorkDirectory></NUnit></RunSettings>");

        _settings.BetaConfiguration.WorkDirectory.ShouldBe("/my/work/dir");
    }

    /// <summary>
    ///     Workdir not set, TestOutput is relative.
    /// </summary>
    [Fact]
    public void TestOutputSetting()
    {
        _settings.Load(
            @"<RunSettings><NUnit><WorkDirectory>C:\Whatever</WorkDirectory><TestOutputXml>/my/work/dir</TestOutputXml></NUnit></RunSettings>");


        _settings.SetTestOutputFolder(_settings.BetaConfiguration.WorkDirectory);

        _settings.BetaConfiguration.TestOutputFolder.ShouldContain("/my/work/dir");
    }

    /// <summary>
    ///     NewOutputXmlFileForEachRun setting.
    /// </summary>
    [Fact]
    public void TestNewOutputXmlFileForEachRunSetting()
    {
        _settings.Load(
            "<RunSettings><NUnit><NewOutputXmlFileForEachRun>true</NewOutputXmlFileForEachRun></NUnit></RunSettings>");

        _settings.BetaConfiguration.NewOutputXmlFileForEachRun.ShouldBeTrue();
    }

    [Fact]
    public void TestOutputSettingWithWorkDir()
    {
        _settings.Load(
            @"<RunSettings><NUnit><WorkDirectory>C:\Whatever</WorkDirectory><TestOutputXml>my/testoutput/dir</TestOutputXml><OutputXmlFolderMode>RelativeToWorkFolder</OutputXmlFolderMode></NUnit></RunSettings>");

        _settings.SetTestOutputFolder(_settings.BetaConfiguration.WorkDirectory);

        _settings.BetaConfiguration.TestOutputFolder.ShouldContain(@"\my/testoutput/dir");
        _settings.BetaConfiguration.TestOutputFolder.ShouldStartWith(@"C:\");
        Path.IsPathRooted(_settings.BetaConfiguration.TestOutputFolder).ShouldBeTrue();
    }

    /// <summary>
    ///     Test should set output folder to same as resultdirectory, and ignore workdirectory and testoutputxml.
    /// </summary>
    [Fact]
    public void TestOutputSettingWithUseResultDirectory()
    {
        _settings.Load("""
                       <RunSettings>
                           <RunConfiguration>
                               <ResultsDirectory>c:\whatever\results</ResultsDirectory>
                           </RunConfiguration>
                           <NUnit>
                               <WorkDirectory>C:\AnotherWhatever</WorkDirectory>
                               <TestOutputXml>my/testoutput/dir</TestOutputXml>
                               <OutputXmlFolderMode>UseResultDirectory</OutputXmlFolderMode>
                           </NUnit>
                       </RunSettings>
                       """);
        _settings.SetTestOutputFolder(_settings.BetaConfiguration.WorkDirectory);

        _settings.BetaConfiguration.TestOutputFolder.ShouldBe(@"c:\whatever\results");
    }

    /// <summary>
    ///     Test should set output folder to same as resultdirectory, and ignore workdirectory and testoutputxml.
    /// </summary>
    [Fact]
    public void TestOutputSettingAsSpecified()
    {
        _settings.Load(@"<RunSettings><NUnit><TestOutputXml>c:\whatever</TestOutputXml></NUnit></RunSettings>");
        _settings.SetTestOutputFolder(_settings.BetaConfiguration.WorkDirectory);

        _settings.BetaConfiguration.TestOutputFolder.ShouldBe(@"c:\whatever");
        _settings.BetaConfiguration.OutputXmlFolderMode.ShouldBe(OutputXmlFolderMode.AsSpecified);
    }

    [Fact]
    public void NumberOfTestWorkersSetting()
    {
        _settings.Load("<RunSettings><NUnit><NumberOfTestWorkers>12</NumberOfTestWorkers></NUnit></RunSettings>");

        _settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(12);
    }

    [Fact]
    public void DefaultTimeoutSetting()
    {
        _settings.Load("<RunSettings><NUnit><DefaultTimeout>5000</DefaultTimeout></NUnit></RunSettings>");

        _settings.BetaConfiguration.DefaultTimeout.ShouldBe(5000);
    }

    [Fact]
    public void ShadowCopySetting()
    {
        _settings.Load("<RunSettings><NUnit><ShadowCopyFiles>true</ShadowCopyFiles></NUnit></RunSettings>");

        _settings.BetaConfiguration.ShadowCopyFiles.ShouldBeTrue();
    }


    [Fact]
    public void ShadowCopySettingDefault()
    {
        _settings.Load("");

        _settings.BetaConfiguration.ShadowCopyFiles.ShouldBeFalse();
    }

    [Fact]
    public void VerbositySetting()
    {
        _settings.Load("<RunSettings><NUnit><Verbosity>1</Verbosity></NUnit></RunSettings>");

        _settings.BetaConfiguration.Verbosity.ShouldBe(1);
    }

    [Fact]
    public void UseVsKeepEngineRunningSetting()
    {
        _settings.Load(
            "<RunSettings><NUnit><UseVsKeepEngineRunning>true</UseVsKeepEngineRunning></NUnit></RunSettings>");

        _settings.BetaConfiguration.UseVsKeepEngineRunning.ShouldBeTrue();
    }

    [Fact]
    public void PreFilterCanBeSet()
    {
        _settings.Load("<RunSettings><NUnit><PreFilter>true</PreFilter></NUnit></RunSettings>");

        _settings.BetaConfiguration.PreFilter.ShouldBeTrue();
    }


    [Fact]
    public void BasePathSetting()
    {
        _settings.Load("<RunSettings><NUnit><BasePath>..</BasePath></NUnit></RunSettings>");

        _settings.BetaConfiguration.BasePath.ShouldBe("..");
    }


    [Fact]
    public void VsTestCategoryTypeSetting()
    {
        _settings.Load("<RunSettings><NUnit><VsTestCategoryType>mstest</VsTestCategoryType></NUnit></RunSettings>");

        _settings.BetaConfiguration.VsTestCategoryType.ShouldBe(VsTestCategoryType.MsTest);
    }

    [Fact]
    public void VsTestCategoryTypeSettingWithGarbage()
    {
        _settings.Load("<RunSettings><NUnit><VsTestCategoryType>garbage</VsTestCategoryType></NUnit></RunSettings>");

        _settings.BetaConfiguration.VsTestCategoryType.ShouldBe(VsTestCategoryType.NUnit);
    }


    [Fact]
    public void PrivateBinPathSetting()
    {
        _settings.Load("<RunSettings><NUnit><PrivateBinPath>dir1;dir2</PrivateBinPath></NUnit></RunSettings>");

        _settings.BetaConfiguration.PrivateBinPath.ShouldBe("dir1;dir2");
    }

    [Fact]
    public void RandomSeedSetting()
    {
        _settings.Load("<RunSettings><NUnit><RandomSeed>12345</RandomSeed></NUnit></RunSettings>");

        _settings.BetaConfiguration.RandomSeed.ShouldBe(12345);
    }

    [Fact]
    public void DefaultTestNamePattern()
    {
        _settings.Load(
            "<RunSettings><NUnit><DefaultTestNamePattern>{m}{a:1000}</DefaultTestNamePattern></NUnit></RunSettings>");

        _settings.BetaConfiguration.DefaultTestNamePattern.ShouldBe("{m}{a:1000}");
    }

    [Fact]
    public void CollectDataForEachTestSeparately()
    {
        _settings.Load(@"
<RunSettings>
    <RunConfiguration>
        <CollectDataForEachTestSeparately>true</CollectDataForEachTestSeparately>
    </RunConfiguration>
    <InProcDataCollectionRunSettings>
        <InProcDataCollectors>
            <InProcDataCollector friendlyName='DummyCollectorName' uri='InProcDataCollector://NUnit/DummyCollectorName' />
        </InProcDataCollectors>
    </InProcDataCollectionRunSettings>
</RunSettings>");

        _settings.BetaConfiguration.DomainUsage.ShouldBeNull();
        _settings.BetaConfiguration.SynchronousEvents.ShouldBeTrue();
        _settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(0);
        _settings.BetaConfiguration.InProcDataCollectorsAvailable.ShouldBeTrue();
    }

    [Fact]
    public void InProcDataCollector()
    {
        _settings.Load(@"
<RunSettings>
    <InProcDataCollectionRunSettings>
        <InProcDataCollectors>
            <InProcDataCollector friendlyName='DummyCollectorName' uri='InProcDataCollector://NUnit/DummyCollectorName' />
        </InProcDataCollectors>
    </InProcDataCollectionRunSettings>
</RunSettings>");

        _settings.BetaConfiguration.DomainUsage.ShouldBeNull();
        _settings.BetaConfiguration.SynchronousEvents.ShouldBeFalse();
        _settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(-1);
        _settings.BetaConfiguration.InProcDataCollectorsAvailable.ShouldBeTrue();
    }

    [Fact]
    public void LiveUnitTestingDataCollector()
    {
        _settings.Load(@"
<RunSettings>
    <InProcDataCollectionRunSettings>
        <InProcDataCollectors>
            <InProcDataCollector friendlyName='DummyCollectorName' uri='InProcDataCollector://Microsoft/LiveUnitTesting/1.0' />
        </InProcDataCollectors>
    </InProcDataCollectionRunSettings>
</RunSettings>");

        _settings.BetaConfiguration.DomainUsage.ShouldBeNull();
        _settings.BetaConfiguration.SynchronousEvents.ShouldBeTrue();
        _settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(0);
        _settings.BetaConfiguration.InProcDataCollectorsAvailable.ShouldBeTrue();
    }

    [Fact]
    public void WhereCanBeSet()
    {
        _settings.Load(
            "<RunSettings><NUnit><Where>cat == SomeCategory and namespace == SomeNamespace or cat != SomeOtherCategory</Where></NUnit></RunSettings>");

        _settings.BetaConfiguration.Where.ShouldBe(
            "cat == SomeCategory and namespace == SomeNamespace or cat != SomeOtherCategory");
    }

    [InlineData("None", TestOutcome.None)]
    [InlineData("Passed", TestOutcome.Passed)]
    [InlineData("Failed", TestOutcome.Failed)]
    [InlineData("Skipped", TestOutcome.Skipped)]
    [Theory]
    public void MapWarningToTests(string setting, TestOutcome outcome)
    {
        var runsettings = $"<RunSettings><NUnit><MapWarningTo>{setting}</MapWarningTo></NUnit></RunSettings>";
        _settings.Load(runsettings);

        _settings.BetaConfiguration.MapWarningTo.ShouldBe(outcome);
    }

    [InlineData("garbage")]
    [Theory]
    public void MapWarningToTestsFailing(string setting)
    {
        var runsettings = $"<RunSettings><NUnit><MapWarningTo>{setting}</MapWarningTo></NUnit></RunSettings>";
        _settings.Load(runsettings);

        _settings.BetaConfiguration.MapWarningTo.ShouldBe(TestOutcome.Skipped);
    }

    [Fact]
    public void MapWarningToTestsDefault()
    {
        _settings.Load("");
    }

    [InlineData("Name", DisplayNameOptions.Name)]
    [InlineData("Fullname", DisplayNameOptions.FullName)]
    [InlineData("FullnameSep", DisplayNameOptions.FullNameSep)]
    [InlineData("invalid", DisplayNameOptions.Name)]
    [Theory]
    public void MapDisplayNameTests(string setting, DisplayNameOptions outcome)
    {
        var runsettings = $"<RunSettings><NUnit><DisplayName>{setting}</DisplayName></NUnit></RunSettings>";
        _settings.Load(runsettings);

        _settings.BetaConfiguration.DisplayName.ShouldBe(outcome);
    }

    [InlineData(":")]
    [InlineData("-")]
    [InlineData(".")]
    [Theory]
    public void FullNameSeparatorTest(string setting)
    {
        var expected = setting[0];
        var runsettings = $"<RunSettings><NUnit><FullnameSeparator>{setting}</FullnameSeparator></NUnit></RunSettings>";
        _settings.Load(runsettings);

        _settings.BetaConfiguration.FullnameSeparator.ShouldBe(expected);
    }

    [Fact]
    public void TestDefaults()
    {
        _settings.Load("");

        _settings.BetaConfiguration.FullnameSeparator.ShouldBe(':');
        _settings.BetaConfiguration.DisplayName.ShouldBe(DisplayNameOptions.Name);
        _settings.BetaConfiguration.MapWarningTo.ShouldBe(TestOutcome.Skipped);
    }
}
