using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Google.Cloud.Translate.V3.Samples;
using GoogleCloudSamples;

public class TranslateTextWithModelTest
{
    protected string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    protected string ModelId { get; private set; } = "TRL2188848820815848149";

    readonly CommandLineRunner _quickStart = new CommandLineRunner()
    {
        VoidMain = TranslateV3TranslateTextWithModelMain.Main,
        Command = "Translate Text with Model"
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
    public void TestTranslateTextWithModel()
    {
        var output = Run("--project_id=" + ProjectId,
            "--location=us-central1", "--text=That' il do it",
            "--target_language=ja", "--model_id=" + ModelId);
        Assert.True(output.Stdout.Contains("\u305D\u308C\u306F\u305D\u3046\u3060") || output.Stdout.Contains("\u305D\u308C\u3058\u3083\u3042"));
    }
}



