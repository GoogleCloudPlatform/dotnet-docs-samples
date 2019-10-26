using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Google.Cloud.Translate.V3.Samples;
using GoogleCloudSamples;

public class GetSupportedLanguagesTest
{
    protected string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GCLOUD_PROJECT");

    readonly CommandLineRunner _sample = new CommandLineRunner()
    {
        VoidMain = TranslateV3GetSupportedLanguagesMain.Main,
        Command = "Get Supported Languages"
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
    public void TestGetSupportedLanguages()
    {
        var output = Run("--project_id=" + ProjectId);
        Assert.Contains("zh-CN", output.Stdout);
    }
}
