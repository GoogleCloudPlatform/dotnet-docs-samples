using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using TranslateV3Samples;
using GoogleCloudSamples;

public class GetSupportedLanguagesWithTargetTests
{
    private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

    private readonly CommandLineRunner _sample = new CommandLineRunner()
    {
        VoidMain = TranslateV3GetSupportedLanguagesForTargetMain.Main
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
    public void GetSupportedLanguagesWithTargetTest()
    {
        var output = Run("--project_id=" + _projectId, "--language_code=is");
        Assert.Contains("Language Code: sq", output.Stdout);
        Assert.Contains("Display Name: albanska", output.Stdout);
    }
}
