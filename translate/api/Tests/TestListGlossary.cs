using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Google.Cloud.Translate.V3.Samples;
using GoogleCloudSamples;

public class ListGlossaryTest : IDisposable
{
    protected string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    protected string GlossaryId { get; private set; }
    protected string GlossaryInputUri { get; private set; } = "gs://cloud-samples-data/translation/glossary_ja.csv";

    readonly CommandLineRunner _quickStart = new CommandLineRunner()
    {
        VoidMain = TranslateV3ListGlossaryMain.Main,
        Command = "TranslateV3ListGlossary.SampleListGlossaries"
    };

    //Setup
    public ListGlossaryTest()
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
    public void TestListGlossaries()
    {
        var output = Run("--project_id=" + ProjectId);
        Assert.Contains("gs://cloud-samples-data/translation/glossary_ja.csv", output.Stdout);
    }
}



