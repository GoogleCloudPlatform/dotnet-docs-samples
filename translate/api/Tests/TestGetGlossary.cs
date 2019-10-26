using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Google.Cloud.Translate.V3.Samples;
using GoogleCloudSamples;

public class GetGlossaryTest : IDisposable
{
    protected string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GCLOUD_PROJECT");
    protected string GlossaryId { get; private set; }
    protected string GlossaryInputUri { get; private set; } = "gs://cloud-samples-data/translation/glossary_ja.csv";

    readonly CommandLineRunner _sample = new CommandLineRunner()
    {
        VoidMain = TranslateV3GetGlossaryMain.Main,
        Command = "Get Glossary"
    };

    //Setup
    public GetGlossaryTest()
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
        return _sample.Run(arguments);
    }


    [Fact]
    public void TestRun()
    {
        var output = Run("--project_id=" + ProjectId, "--glossary_id=" + GlossaryId);
        Assert.Contains("gs://cloud-samples-data/translation/glossary_ja.csv", output.Stdout);
    }
}



