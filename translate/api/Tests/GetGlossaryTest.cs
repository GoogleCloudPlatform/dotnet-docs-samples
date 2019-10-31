using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using TranslateV3Samples;
using GoogleCloudSamples;

public class GetGlossaryTest : IDisposable
{
    private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    protected string GlossaryId { get; private set; }
    protected string GlossaryInputUri { get; private set; } = "gs://cloud-samples-data/translation/glossary_ja.csv";

    private readonly CommandLineRunner _sample = new CommandLineRunner()
    {
        VoidMain = TranslateV3GetGlossaryMain.Main
    };

    // Setup
    public GetGlossaryTest()
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
        return _sample.Run(arguments);
    }

    [Fact]
    public void GetGlossary()
    {
        var output = Run("--project_id=" + _projectId, "--glossary_id=" + GlossaryId);
        Assert.Contains("gs://cloud-samples-data/translation/glossary_ja.csv", output.Stdout);
    }
}



