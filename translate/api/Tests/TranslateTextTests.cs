using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using TranslateV3Samples;
using GoogleCloudSamples;

public class TranslateTextTests
{
    private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

    private readonly CommandLineRunner _quickStart = new CommandLineRunner()
    {
        VoidMain = TranslateV3TranslateTextMain.Main
    };

    /// <summary>
    ///  Run the command and track all cloud assets that were created.
    /// </summary>
    /// <param name="arguments">The command arguments.</param>
    public ConsoleOutput Run(params string[] arguments)
    {
        return _quickStart.Run(arguments);
    }

    [Fact]
    public void TranslateTextTest()
    {
        var output = Run("--project_id=" + _projectId, "--text=Hello world", "--target_language=sr-Latn");
        Assert.True(output.Stdout.Contains("Zdravo svet") || output.Stdout.Contains("Pozdrav svijetu"));
    }
}



