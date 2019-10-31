using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using TranslateV3Samples;
using GoogleCloudSamples;

public class TranslateTextWithGlossaryTest : IDisposable
{
    private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    protected string GlossaryId { get; private set; }
    protected string GlossaryInputUri { get; private set; } = "gs://cloud-samples-data/translation/glossary_ja.csv";

    private readonly CommandLineRunner _quickStart = new CommandLineRunner()
    {
        VoidMain = TranslateV3TranslateTextWithGlossaryMain.Main
    };

    // Setup
    public TranslateTextWithGlossaryTest()
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
    public void TranslateTextWithGlossary()
    {
        var output = Run("--project_id=" + _projectId, "--text=account", "--target_language=ja", "--glossary_id=" + GlossaryId);
        Assert.True(output.Stdout.Contains("\u30A2\u30AB\u30A6\u30F3\u30C8") || output.Stdout.Contains("\u53E3\u5EA7"));
    }
}



