using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beta.Runner.TestAdapter;

namespace Beta.Tests.TestAdapter;

public class RunSettingsTests
{
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [Theory]
    public void NullOrEmptyXmlContentReturnsEmptyRunSettings(string input)
    {
        // Act.
        var settings = RunSettings.Parse(input);

        // Assert.
        settings.ShouldBeSameAs(RunSettings.Empty);
    }

    [Fact]
    public void MalformedContentReturnsEmptyRunSettings()
    {
        // Arrange.
        var input = "</invalid xml>";

        // Act.
        var settings = RunSettings.Parse(input);

        // Assert.
        settings.ShouldBeSameAs(RunSettings.Empty);
    }

    [Fact]
    public void MissingRunConfigurationElementReturnsEmptyRunSettings()
    {
        // Arrange.
        var input = "<RunSettings></RunSettings>";

        // Act.
        var settings = RunSettings.Parse(input);

        // Assert.
        settings.ShouldBeSameAs(RunSettings.Empty);
    }

    [Fact]
    public void ParsesSolutionDirectory()
    {
        // Arrange.
        var input =
            """
            <RunSettings>
                <RunConfiguration>
                    <SolutionDirectory>C:\</SolutionDirectory>
                    <ResultsDirectory>C:\Results</ResultsDirectory>
                    <CollectSourceInformation>true</CollectSourceInformation>
                    <TargetFrameworkVersion>net5.0</TargetFrameworkVersion>
                    <TargetPlatform>x64</TargetPlatform>
                    <BatchSize>100</BatchSize>
                    <DesignMode>true</DesignMode>
                    <MaxCpuCount>8</MaxCpuCount>
                </RunConfiguration>
            </RunSettings>
            """;

        // Act.
        var settings = RunSettings.Parse(input);

        // Assert.
        settings.SolutionDirectory.ShouldBe(@"C:\");
        settings.ResultsDirectory.ShouldBe(@"C:\Results");
        settings.CollectSourceInformation.ShouldBeTrue();
        settings.TargetFrameworkVersion.ShouldBe("net5.0");
        settings.TargetPlatform.ShouldBe("x64");
        settings.BatchSize.ShouldBe(100);
        settings.DesignMode.ShouldBeTrue();
        settings.MaxCpuCount.ShouldBe(8);
    }
}