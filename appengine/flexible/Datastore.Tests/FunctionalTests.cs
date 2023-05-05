using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

/// <summary>
/// Provides simple tests to demonstrate calling the sample app. This is
/// based on the straightforward examples provided by Microsoft here:
/// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0
/// </summary>
public class FunctionalTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public FunctionalTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task TestGetSucceeds()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");
        response.EnsureSuccessStatusCode();
    }
}

