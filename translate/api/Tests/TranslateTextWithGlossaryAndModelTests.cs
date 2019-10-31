using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using TranslateV3Samples;
using GoogleCloudSamples;

public class TranslateTextWithGlossaryAndModelTests : IDisposable
{
    private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    private readonly string _glossaryInputUri = "gs://cloud-samples-data/translation/glossary_ja.csv";
    private readonly string _modelId = "TRL8772189639420149760";
    protected string GlossaryId { get; private set; }

    private readonly CommandLineRunner _quickStart = new CommandLineRunner()
    {
        VoidMain = TranslateV3TranslateTextWithGlossaryAndModelMain.Main
    };

    // Setup
    public TranslateTextWithGlossaryAndModelTests()
    {
        GlossaryId = "translate-v3" + TestUtil.RandomName();
        TranslateV3CreateGlossary.CreateGlossarySample(_projectId, GlossaryId, _glossaryInputUri);
    }

    // TearDown
    public void Dispose()
    {
        TranslateV3DeleteGlossary.DeleteGlossarySample(_projectId, GlossaryId);
    }

    /// <summary>
    ///  Run the command and track all cloud assets that were created.
    /// </summary>
    /// <param name="arguments">The command arguments.</param>
    public ConsoleOutput Run(params string[] arguments)
    {
        return _quickStart.Run(arguments);
    }

    [Fact]
    public void TranslateTextWithGlossaryAndModelTest()
    {
        var output = Run("--project_id=" + _projectId,
            "--location=us-central1",
            "--text=That' il do it. deception",
            "--target_language=ja",
            "--glossary_id=" + GlossaryId,
            "--model_id=" + _modelId);
        Assert.True(output.Stdout.Contains("\u3084\u308B\u6B3A\u304F")
            || output.Stdout.Contains("\u305D\u308C\u3058\u3083\u3042")); // custom model
        Assert.Contains("\u6B3A\u304F", output.Stdout); //glossary
    }
}



