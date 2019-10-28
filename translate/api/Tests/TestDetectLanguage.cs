using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Google.Cloud.Translate.V3.Samples;
using GoogleCloudSamples;

public class DetectLanguageTest
{
    protected string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

    readonly CommandLineRunner _sample = new CommandLineRunner()
    {
        VoidMain = TranslateV3DetectLanguageMain.Main
    };


    /// <summary>
    ///  Run the command and track all cloud assets that were created.
    /// </summary>
    /// <param name="arguments">The command arguments.</param>
    public ConsoleOutput Run(params string[] arguments)
    {
        return _sample.Run(arguments);
    }

    [Fact]
    public void TestDetectLanguage()
    {
        var output = Run("--project_id=" + ProjectId, "--text=H\u00E6 s\u00E6ta");
        Assert.Contains("is", output.Stdout);
    }
}
