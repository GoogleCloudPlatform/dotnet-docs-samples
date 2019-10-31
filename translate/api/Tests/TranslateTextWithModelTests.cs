using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using TranslateV3Samples;
using GoogleCloudSamples;

public class TranslateTextWithModelTests
{
    private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    private readonly string _modelId = "TRL8772189639420149760";

    private readonly CommandLineRunner _quickStart = new CommandLineRunner()
    {
        VoidMain = TranslateV3TranslateTextWithModelMain.Main
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
    public void TranslateTextWithModelTest()
    {
        var output = Run("--project_id=" + _projectId,
            "--location=us-central1", "--text=That' il do it",
            "--target_language=ja", "--model_id=" + _modelId);
        Assert.True(output.Stdout.Contains("\u305D\u308C\u306F\u305D\u3046\u3060") || output.Stdout.Contains("\u3084\u308B"));
    }
}



