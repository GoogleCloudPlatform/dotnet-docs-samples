using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Google.Cloud.Translate.V3.Samples;
using GoogleCloudSamples;

public class GetSupportedLanguagesWithTargetTest
{
    protected string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GCLOUD_PROJECT");

    readonly CommandLineRunner _sample = new CommandLineRunner()
    {
        VoidMain = TranslateV3GetSupportedLanguagesForTargetMain.Main,
        Command = "Get Supported Languages with Target"
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
    public void TestGetSupportedLanguagesWithTarget()
    {
        var output = Run("--project_id=" + ProjectId, "--language_code=is");
        Assert.Contains("Language Code: sq", output.Stdout);
        Assert.Contains("Display Name: albanska", output.Stdout);
    }
}
