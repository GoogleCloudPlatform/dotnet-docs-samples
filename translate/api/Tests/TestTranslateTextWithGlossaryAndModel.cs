using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Google.Cloud.Translate.V3.Samples;
using GoogleCloudSamples;

public class TranslateTextWithGlossaryAndModelTest : IDisposable
{
    protected string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    protected string GlossaryId { get; private set; }
    protected string GlossaryInputUri { get; private set; } = "gs://cloud-samples-data/translation/glossary_ja.csv";
    protected string ModelId { get; private set; } = "TRL8772189639420149760";
    protected string InputUri { get; private set; } = "gs://cloud-samples-data/translation/text_with_custom_model_and_glossary.txt";

    readonly CommandLineRunner _quickStart = new CommandLineRunner()
    {
        VoidMain = TranslateV3TranslateTextWithGlossaryAndModelMain.Main,
        Command = "Translate Text with Glossary and Model"
    };

    //Setup
    public TranslateTextWithGlossaryAndModelTest()
    {
        GlossaryId = "translate-v3" + TestUtil.RandomName();
        TranslateV3CreateGlossary.SampleCreateGlossary(ProjectId, GlossaryId, GlossaryInputUri);
    }


    //TearDown
    public void Dispose()
    {
        TranslateV3DeleteGlossary.SampleDeleteGlossary(ProjectId, GlossaryId);
    }

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
    public void TestTranslateTextWithGlossaryAndModel()
    {
        var output = Run("--project_id=" + ProjectId,
            "--location=us-central1",
            "--text=That' il do it. deception",
            "--target_language=ja",
            "--glossary_id=" + GlossaryId,
            "--model_id=" + ModelId);
        Assert.True(output.Stdout.Contains("\u305D\u308C\u306F\u305D\u3046\u3060") || output.Stdout.Contains("\u305D\u308C\u3058\u3083\u3042")); //custom model
        Assert.Contains("\u6B3A\u304F", output.Stdout); //glossary
    }
}



