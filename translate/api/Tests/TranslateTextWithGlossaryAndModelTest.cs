using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using TranslateV3Samples;
using GoogleCloudSamples;

public class TranslateTextWithGlossaryAndModelTest : IDisposable
{
    private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    protected string GlossaryId { get; private set; }
    protected string GlossaryInputUri { get; private set; } = "gs://cloud-samples-data/translation/glossary_ja.csv";
    protected string ModelId { get; private set; } = "TRL8772189639420149760";
    protected string InputUri { get; private set; } = "gs://cloud-samples-data/translation/text_with_custom_model_and_glossary.txt";

    private readonly CommandLineRunner _quickStart = new CommandLineRunner()
    {
        VoidMain = TranslateV3TranslateTextWithGlossaryAndModelMain.Main
    };

    // Setup
    public TranslateTextWithGlossaryAndModelTest()
    {
        GlossaryId = "translate-v3" + TestUtil.RandomName();
        TranslateV3CreateGlossary.CreateGlossarySample(_projectId, GlossaryId, GlossaryInputUri);
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
    public void TranslateTextWithGlossaryAndModel()
    {
        var output = Run("--project_id=" + _projectId,
            "--location=us-central1",
            "--text=That' il do it. deception",
            "--target_language=ja",
            "--glossary_id=" + GlossaryId,
            "--model_id=" + ModelId);
        Assert.True(output.Stdout.Contains("\u3084\u308B\u6B3A\u304F")
            || output.Stdout.Contains("\u305D\u308C\u3058\u3083\u3042")); // custom model
        Assert.Contains("\u6B3A\u304F", output.Stdout); //glossary
    }
}



