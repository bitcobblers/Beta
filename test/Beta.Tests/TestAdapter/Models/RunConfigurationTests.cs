using System.Xml.Linq;
using Beta.TestAdapter.Models;

namespace Beta.Tests.TestAdapter.Models;

public class RunConfigurationTests
{
    private const string SampleConfiguration =
        """
        <RunConfiguration>
          <ResultsDirectory>E:\Path\To\TestResults</ResultsDirectory>
          <SolutionDirectory>E:\Path\To\Solution</SolutionDirectory>
          <MaxCpuCount>0</MaxCpuCount>
          <EnvironmentVariables>
            <VSTEST_BACKGROUND_DISCOVERY>1</VSTEST_BACKGROUND_DISCOVERY>
          </EnvironmentVariables>
          <CollectSourceInformation>False</CollectSourceInformation>
          <TargetFrameworkVersion>.NETCoreApp,Version=v8.0</TargetFrameworkVersion>
          <TargetPlatform>X64</TargetPlatform>
          <DesignMode>True</DesignMode>
          <BatchSize>1000</BatchSize>
        </RunConfiguration>
        """;

    [Fact]
    public void CanParseFullConfiguration()
    {
        // Arrange.
        var xml = XElement.Parse(SampleConfiguration);

        // Act.
        var configuration = RunConfiguration.Parse(xml);

        // Assert.
        configuration.ResultsDirectory.ShouldBe(@"E:\Path\To\TestResults");
        configuration.SolutionDirectory.ShouldBe(@"E:\Path\To\Solution");
        configuration.MaxCpuCount.ShouldBe(0);
        configuration.EnvironmentVariables.Count.ShouldBe(1);
        configuration.EnvironmentVariables["VSTEST_BACKGROUND_DISCOVERY"].ShouldBe("1");
        configuration.CollectSourceInformation.ShouldBeFalse();
        configuration.TargetFrameworkVersion.ShouldBe(".NETCoreApp,Version=v8.0");
        configuration.TargetPlatform.ShouldBe("X64");
        configuration.DesignMode.ShouldBeTrue();
        configuration.BatchSize.ShouldBe(1000);
    }
}
