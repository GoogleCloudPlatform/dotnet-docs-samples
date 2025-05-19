using Google.Cloud.ModelArmor.V1;
using Xunit;
using Xunit.Abstractions;

[Collection(nameof(ModelArmorFixture))]
public class QuickstartTests
{
    private readonly ModelArmorFixture _fixture;
    private readonly QuickstartSample _sample;
    private readonly ITestOutputHelper _output;

    public QuickstartTests(ModelArmorFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _sample = new QuickstartSample();
        _output = output;
    }

    [Fact]
    public void Runs()
    {
        // Get template name from fixture
        TemplateName templateName = _fixture.TemplateForQuickstartName;

        // Run the quickstart sample
        _sample.Quickstart(
            projectId: templateName.ProjectId,
            locationId: templateName.LocationId,
            templateId: templateName.TemplateId
        );

        // You can add assertions here if needed
        _output.WriteLine($"Successfully ran quickstart with template {templateName}");
    }
}
