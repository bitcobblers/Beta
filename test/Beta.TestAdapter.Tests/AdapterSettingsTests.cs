using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Beta.TestAdapter.Tests;

public class AdapterSettingsTests
{
    [InlineData(null)]
    [InlineData("")]
    [InlineData("<RunSettings />")]
    [Theory]
    public void DefaultSettings(string xml)
    {
        var settings = new AdapterSettings(xml);

        Assert.Multiple(() =>
        {
            settings.RunConfiguration.MaxCpuCount.ShouldBe(-1);
            settings.RunConfiguration.ResultsDirectory.ShouldBeNull();
            settings.RunConfiguration.TargetFrameworkVersion.ShouldBeNull();
            settings.RunConfiguration.TargetPlatform.ShouldBeNull();
            settings.RunConfiguration.TestAdapterPaths.ShouldBeNull();
            settings.RunConfiguration.CollectSourceInformation.ShouldBeTrue();
            settings.RunConfiguration.DisableAppDomain.ShouldBeFalse();
            settings.RunConfiguration.DisableParallelization.ShouldBeFalse();
            settings.RunConfiguration.DesignMode.ShouldBeFalse();

            settings.TestProperties.ShouldBeEmpty();

            settings.BetaConfiguration.TraceLevel.ShouldBe(Engine.InternalTraceLevel.Off);
            settings.BetaConfiguration.WorkDirectory.ShouldBeNull();
            settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(-1);
            settings.BetaConfiguration.DefaultTimeout.ShouldBe(0);
            settings.BetaConfiguration.Verbosity.ShouldBe(0);
            settings.BetaConfiguration.ShadowCopyFiles.ShouldBeFalse();
            settings.BetaConfiguration.UseVsKeepEngineRunning.ShouldBeFalse();
            settings.BetaConfiguration.BasePath.ShouldBeNull();
            settings.BetaConfiguration.PrivateBinPath.ShouldBeNull();
            settings.BetaConfiguration.RandomSeed.ShouldNotBeNull();
            settings.BetaConfiguration.SynchronousEvents.ShouldBeFalse();
            settings.BetaConfiguration.DomainUsage.ShouldBeNullOrWhiteSpace();
            settings.BetaConfiguration.InProcDataCollectorsAvailable.ShouldBeFalse();
            settings.BetaConfiguration.UseTestOutputXml.ShouldBeFalse();
            settings.BetaConfiguration.NewOutputXmlFileForEachRun.ShouldBeFalse();
            settings.BetaConfiguration.PreFilter.ShouldBeFalse();
            settings.BetaConfiguration.ShadowCopyFiles.ShouldBeFalse();
            settings.BetaConfiguration.FullnameSeparator.ShouldBe(':');
            settings.BetaConfiguration.DisplayName.ShouldBe(DisplayNameOptions.Name);
            settings.BetaConfiguration.MapWarningTo.ShouldBe(TestOutcome.Skipped);
        });
    }

    [Fact]
    public void ResultsDirectorySetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <RunConfiguration>
                    <ResultsDirectory>./myresults</ResultsDirectory>
                </RunConfiguration>
            </RunSettings>
            """);

        settings.RunConfiguration.ResultsDirectory.ShouldBe("./myresults");
    }

    [Fact]
    public void MaxCpuCountSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                 <RunConfiguration>
                     <MaxCpuCount>42</MaxCpuCount>
                 </RunConfiguration>
             </RunSettings>
            """);

        settings.RunConfiguration.MaxCpuCount.ShouldBe(42);
    }

    [Fact]
    public void TargetFrameworkVersionSetting()
    {
        var settings = new AdapterSettings(
            """
             <RunSettings>
                 <RunConfiguration>
                     <TargetFrameworkVersion>Framework45</TargetFrameworkVersion>
                 </RunConfiguration>
             </RunSettings>
            """);

        settings.RunConfiguration.TargetFrameworkVersion.ShouldBe("Framework45");
    }

    [Fact]
    public void TargetPlatformSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <RunConfiguration>
                    <TargetPlatform>x86</TargetPlatform>
                </RunConfiguration>
            </RunSettings>
            """);

        settings.RunConfiguration.TargetPlatform.ShouldBe("x86");
    }

    [Fact]
    public void TestAdapterPathsSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <RunConfiguration>
                    <TestAdapterPaths>/first/path;/second/path</TestAdapterPaths>
                </RunConfiguration>
            </RunSettings>
            """);

        settings.RunConfiguration.TestAdapterPaths.ShouldBe("/first/path;/second/path");
    }

    [Fact]
    public void CollectSourceInformationSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <RunConfiguration>
                    <CollectSourceInformation>False</CollectSourceInformation>
                </RunConfiguration>
            </RunSettings>
            """);

        settings.RunConfiguration.CollectSourceInformation.ShouldBeFalse();
    }

    [Fact]
    public void DisableAppDomainSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <RunConfiguration>
                    <DisableAppDomain>true</DisableAppDomain>
                </RunConfiguration>
            </RunSettings>
            """);

        settings.RunConfiguration.DisableAppDomain.ShouldBeTrue();
    }

    [Fact]
    public void DisableParallelizationSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <RunConfiguration>
                    <DisableParallelization>true</DisableParallelization>
                </RunConfiguration>
            </RunSettings>
            """);

        settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(0);
    }

    [Fact]
    public void UpdateNumberOfTestWorkersWhenConflictingSettings()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <RunConfiguration>
                    <DisableParallelization>true</DisableParallelization>
                </RunConfiguration>
                <NUnit>
                    <NumberOfTestWorkers>12</NumberOfTestWorkers>
                </NUnit>
            </RunSettings>
            """);

        // When there's a conflicting values in DisableParallelization and NumberOfTestWorkers. Override the NumberOfTestWorkers.
        settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(0);

        // Do not override the NumberOfTestWorkers when DisableParallelization is False
        var settings2 = new AdapterSettings(
            """
            <RunSettings>
                <RunConfiguration>
                    <DisableParallelization>false</DisableParallelization>
                </RunConfiguration>
                <NUnit>
                    <NumberOfTestWorkers>0</NumberOfTestWorkers>
                </NUnit>
            </RunSettings>
            """);
        settings2.BetaConfiguration.NumberOfTestWorkers.ShouldBe(0);

        // Do not override the NumberOfTestWorkers when DisableParallelization is not defined
        var settings3 = new AdapterSettings(
            """
            <RunSettings>
                <RunConfiguration>
                </RunConfiguration>
                <NUnit>
                    <NumberOfTestWorkers>12</NumberOfTestWorkers>
                </NUnit>
            </RunSettings>
            """);
        settings3.BetaConfiguration.NumberOfTestWorkers.ShouldBe(12);
    }

    [Fact]
    public void DesignModeSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <RunConfiguration>
                    <DesignMode>true</DesignMode>
                </RunConfiguration>
            </RunSettings>
            """);

        settings.RunConfiguration.DesignMode.ShouldBeTrue();
    }

    [Fact]
    public void TestRunParameterSettings()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <TestRunParameters>
                    <Parameter name='Answer' value='42'/>
                    <Parameter name='Question' value='Why?'/>
                </TestRunParameters>
            </RunSettings>
            """);

        settings.TestProperties.Count.ShouldBe(2);
        settings.TestProperties["Answer"].ShouldBe("42");
        settings.TestProperties["Question"].ShouldBe("Why?");
    }

    [Fact]
    public void InternalTraceLevel()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <InternalTraceLevel>Debug</InternalTraceLevel>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.TraceLevel.ShouldBe(Engine.InternalTraceLevel.Debug);
    }

    [Fact]
    public void WorkDirectorySetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <WorkDirectory>/my/work/dir</WorkDirectory>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.WorkDirectory.ShouldBe("/my/work/dir");
    }

    /// <summary>
    ///     Workdir not set, TestOutput is relative.
    /// </summary>
    [Fact]
    public void TestOutputSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <WorkDirectory>C:\Whatever</WorkDirectory>
                    <TestOutputXml>/my/work/dir</TestOutputXml>
                </NUnit>
            </RunSettings>
            """);


        settings.SetTestOutputFolder(settings.BetaConfiguration.WorkDirectory);

        settings.BetaConfiguration.TestOutputFolder.ShouldContain("/my/work/dir");
    }

    /// <summary>
    ///     NewOutputXmlFileForEachRun setting.
    /// </summary>
    [Fact]
    public void TestNewOutputXmlFileForEachRunSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <NewOutputXmlFileForEachRun>true</NewOutputXmlFileForEachRun>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.NewOutputXmlFileForEachRun.ShouldBeTrue();
    }

    [Fact]
    public void TestOutputSettingWithWorkDir()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <WorkDirectory>C:\Whatever</WorkDirectory>
                    <TestOutputXml>my/testoutput/dir</TestOutputXml>
                    <OutputXmlFolderMode>RelativeToWorkFolder</OutputXmlFolderMode>
                </NUnit>
            </RunSettings>
            """);

        settings.SetTestOutputFolder(settings.BetaConfiguration.WorkDirectory);

        settings.BetaConfiguration.TestOutputFolder.ShouldContain(@"\my/testoutput/dir");
        settings.BetaConfiguration.TestOutputFolder.ShouldStartWith(@"C:\");
        Path.IsPathRooted(settings.BetaConfiguration.TestOutputFolder).ShouldBeTrue();
    }

    /// <summary>
    ///     Test should set output folder to same as resultdirectory, and ignore workdirectory and testoutputxml.
    /// </summary>
    [Fact]
    public void TestOutputSettingWithUseResultDirectory()
    {
        var settings = new AdapterSettings("""
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
        settings.SetTestOutputFolder(settings.BetaConfiguration.WorkDirectory);

        settings.BetaConfiguration.TestOutputFolder.ShouldBe(@"c:\whatever\results");
    }

    /// <summary>
    ///     Test should set output folder to same as resultdirectory, and ignore workdirectory and testoutputxml.
    /// </summary>
    [Fact]
    public void TestOutputSettingAsSpecified()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <TestOutputXml>c:\whatever</TestOutputXml>
                </NUnit>
            </RunSettings>
            """);

        settings.SetTestOutputFolder(settings.BetaConfiguration.WorkDirectory);

        settings.BetaConfiguration.TestOutputFolder.ShouldBe(@"c:\whatever");
        settings.BetaConfiguration.OutputXmlFolderMode.ShouldBe(OutputXmlFolderMode.AsSpecified);
    }

    [Fact]
    public void NumberOfTestWorkersSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <NumberOfTestWorkers>12</NumberOfTestWorkers>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(12);
    }

    [Fact]
    public void DefaultTimeoutSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <DefaultTimeout>5000</DefaultTimeout>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.DefaultTimeout.ShouldBe(5000);
    }

    [Fact]
    public void ShadowCopySetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <ShadowCopyFiles>true</ShadowCopyFiles>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.ShadowCopyFiles.ShouldBeTrue();
    }

    [Fact]
    public void VerbositySetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <Verbosity>1</Verbosity>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.Verbosity.ShouldBe(1);
    }

    [Fact]
    public void UseVsKeepEngineRunningSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <UseVsKeepEngineRunning>true</UseVsKeepEngineRunning>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.UseVsKeepEngineRunning.ShouldBeTrue();
    }

    [Fact]
    public void PreFilterCanBeSet()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <PreFilter>true</PreFilter>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.PreFilter.ShouldBeTrue();
    }


    [Fact]
    public void BasePathSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <BasePath>..</BasePath>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.BasePath.ShouldBe("..");
    }


    [Fact]
    public void VsTestCategoryTypeSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <VsTestCategoryType>mstest</VsTestCategoryType>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.VsTestCategoryType.ShouldBe(VsTestCategoryType.MsTest);
    }

    [Fact]
    public void VsTestCategoryTypeSettingWithGarbage()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <VsTestCategoryType>garbage</VsTestCategoryType>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.VsTestCategoryType.ShouldBe(VsTestCategoryType.NUnit);
    }


    [Fact]
    public void PrivateBinPathSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <PrivateBinPath>dir1;dir2</PrivateBinPath>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.PrivateBinPath.ShouldBe("dir1;dir2");
    }

    [Fact]
    public void RandomSeedSetting()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <RandomSeed>12345</RandomSeed>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.RandomSeed.ShouldBe(12345);
    }

    [Fact]
    public void DefaultTestNamePattern()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <DefaultTestNamePattern>{m}{a:1000}</DefaultTestNamePattern>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.DefaultTestNamePattern.ShouldBe("{m}{a:1000}");
    }

    [Fact]
    public void CollectDataForEachTestSeparately()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <RunConfiguration>
                    <CollectDataForEachTestSeparately>true</CollectDataForEachTestSeparately>
                </RunConfiguration>
                <InProcDataCollectionRunSettings>
                    <InProcDataCollectors>
                        <InProcDataCollector friendlyName='DummyCollectorName' uri='InProcDataCollector://NUnit/DummyCollectorName' />
                    </InProcDataCollectors>
                </InProcDataCollectionRunSettings>
            </RunSettings>
            """);

        settings.BetaConfiguration.DomainUsage.ShouldBeNullOrWhiteSpace();
        settings.BetaConfiguration.SynchronousEvents.ShouldBeTrue();
        settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(0);
        settings.BetaConfiguration.InProcDataCollectorsAvailable.ShouldBeTrue();
    }

    [Fact]
    public void InProcDataCollector()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <InProcDataCollectionRunSettings>
                    <InProcDataCollectors>
                        <InProcDataCollector friendlyName='DummyCollectorName' uri='InProcDataCollector://NUnit/DummyCollectorName' />
                    </InProcDataCollectors>
                </InProcDataCollectionRunSettings>
            </RunSettings>
            """);

        settings.BetaConfiguration.DomainUsage.ShouldBeNullOrWhiteSpace();
        settings.BetaConfiguration.SynchronousEvents.ShouldBeFalse();
        settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(-1);
        settings.BetaConfiguration.InProcDataCollectorsAvailable.ShouldBeTrue();
    }

    [Fact]
    public void LiveUnitTestingDataCollector()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <InProcDataCollectionRunSettings>
                    <InProcDataCollectors>
                        <InProcDataCollector friendlyName='DummyCollectorName' uri='InProcDataCollector://Microsoft/LiveUnitTesting/1.0' />
                    </InProcDataCollectors>
                </InProcDataCollectionRunSettings>
            </RunSettings>
            """);

        settings.BetaConfiguration.DomainUsage.ShouldBeNullOrWhiteSpace();
        settings.BetaConfiguration.SynchronousEvents.ShouldBeTrue();
        settings.BetaConfiguration.NumberOfTestWorkers.ShouldBe(0);
        settings.BetaConfiguration.InProcDataCollectorsAvailable.ShouldBeTrue();
    }

    [Fact]
    public void WhereCanBeSet()
    {
        var settings = new AdapterSettings(
            """
            <RunSettings>
                <NUnit>
                    <Where>cat == SomeCategory and namespace == SomeNamespace or cat != SomeOtherCategory</Where>
                </NUnit>
            </RunSettings>
            """);

        settings.BetaConfiguration.Where.ShouldBe(
            "cat == SomeCategory and namespace == SomeNamespace or cat != SomeOtherCategory");
    }

    [InlineData("None", TestOutcome.None)]
    [InlineData("Passed", TestOutcome.Passed)]
    [InlineData("Failed", TestOutcome.Failed)]
    [InlineData("Skipped", TestOutcome.Skipped)]
    [Theory]
    public void MapWarningToTests(string setting, TestOutcome outcome)
    {
        var settings = new AdapterSettings(
            $"""
             <RunSettings>
                <NUnit>
                    <MapWarningTo>{setting}</MapWarningTo>
                </NUnit>
             </RunSettings>
             """);

        settings.BetaConfiguration.MapWarningTo.ShouldBe(outcome);
    }

    [InlineData("garbage")]
    [Theory]
    public void MapWarningToTestsFailing(string setting)
    {
        var settings = new AdapterSettings(
            $"""
             <RunSettings>
                <NUnit>
                    <MapWarningTo>{setting}</MapWarningTo>
                </NUnit>
             </RunSettings>
             """);

        settings.BetaConfiguration.MapWarningTo.ShouldBe(TestOutcome.Skipped);
    }

    [InlineData("Name", DisplayNameOptions.Name)]
    [InlineData("Fullname", DisplayNameOptions.FullName)]
    [InlineData("FullnameSep", DisplayNameOptions.FullNameSep)]
    [InlineData("invalid", DisplayNameOptions.Name)]
    [Theory]
    public void MapDisplayNameTests(string setting, DisplayNameOptions outcome)
    {
        var settings = new AdapterSettings(
            $"""
              <RunSettings>
                 <NUnit>
                     <DisplayName>{setting}</DisplayName>
                 </NUnit>
             </RunSettings>
             """);

        settings.BetaConfiguration.DisplayName.ShouldBe(outcome);
    }

    [InlineData(":")]
    [InlineData("-")]
    [InlineData(".")]
    [Theory]
    public void FullNameSeparatorTest(string setting)
    {
        var expected = setting[0];

        var settings = new AdapterSettings(
            $"""
             <RunSettings>
                <NUnit>
                    <FullnameSeparator>{setting}</FullnameSeparator>
                </NUnit>
             </RunSettings>
             """);

        settings.BetaConfiguration.FullnameSeparator.ShouldBe(expected);
    }
}
