using Beta.TestAdapter.Models;

namespace Beta.Tests.TestAdapter.Models;

public class RunSettingsTests
{
    public class SpaceParseMethod : RunSettingsTests
    {
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [Theory]
        public void ReturnsDefaultSettingsWhenXmlIsNullOrWhite(string? inputXml)
        {
            // Act.
            var settings = RunSettings.Parse(inputXml);

            // Assert.
            Assert.NotNull(settings);
            Assert.NotNull(settings.Configuration);
        }

        [Fact]
        public void ReturnsDefaultRunConfigurationWhenMissingFromConfiguration()
        {
            // Arrange.
            const string xml = "<RunSettings></RunSettings>";

            // Act.
            var settings = RunSettings.Parse(xml);

            // Assert.
            Assert.NotNull(settings.Configuration);
        }

        [Fact]
        public void ReturnsDefaultRunConfigurationWhenGivenMalformedXml()
        {
            // Arrange.
            const string xml = "<RunSettings></ invalid-xml<>></RunSettings>";

            // Act.
            var settings = RunSettings.Parse(xml);

            // Assert.
            Assert.NotNull(settings.Configuration);
        }
    }
}
