using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Google.Cloud.Translate.V3.Samples;
using GoogleCloudSamples;

public class TranslateTextTest
{
    protected string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

    readonly CommandLineRunner _quickStart = new CommandLineRunner()
    {
        VoidMain = TranslateV3TranslateTextMain.Main,
        Command = "Translate Text"
    };

    /// <summary>
    ///  Run the command and track all cloud assets that were created.
    /// </summary>
    /// <param name="arguments">The command arguments.</param>
    public ConsoleOutput Run(params string[] arguments)
    {
        Console.WriteLine(arguments);
        return _quickStart.Run(arguments);
    }


    [Fact]
    public void TestTranslateText()
    {
        var output = Run("--project_id=" + ProjectId, "--text=Hello world", "--target_language=sr-Latn");
        Assert.True(output.Stdout.Contains("Zdravo svet") || output.Stdout.Contains("Pozdrav svijetu"));
    }
}



