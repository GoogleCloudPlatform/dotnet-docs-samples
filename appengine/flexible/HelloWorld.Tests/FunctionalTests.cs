using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

// Provides simple tests to demonstrate calling the sample app. This is
// based on the straightforward examples provided by Microsoft here:
// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0
public class FunctionalTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public FunctionalTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/")]
    public async Task TestGetSucceeds(string url)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
	var body = await response.Content.ReadAsStringAsync();
	Assert.True(string.Equals("Hello World!", body), $"Unexpected response: {body}");
    }
}

