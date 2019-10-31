using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using TranslateV3Samples;
using GoogleCloudSamples;

public class GetSupportedLanguagesTest
{
    private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

    private readonly CommandLineRunner _sample = new CommandLineRunner()
    {
        VoidMain = TranslateV3GetSupportedLanguagesMain.Main
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
    public void GetSupportedLanguages()
    {
        var output = Run("--project_id=" + _projectId);
        Assert.Contains("zh-CN", output.Stdout);
    }
}
