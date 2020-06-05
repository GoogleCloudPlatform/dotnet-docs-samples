// Copyright 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Api.Gax.ResourceNames;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Dlp.V2;
using GoogleCloudSamples;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;

public class DlpTestFixture
{
    public string ProjectId => Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    public string WrappedKey => Environment.GetEnvironmentVariable("DLP_DEID_WRAPPED_KEY");
    public string KeyName => Environment.GetEnvironmentVariable("DLP_DEID_KEY_NAME");
    public readonly string ResourcePath = Path.GetFullPath("../../../resources/");

    public readonly CommandLineRunner CommandLineRunner = new CommandLineRunner
    {
        VoidMain = Dlp.Main,
    };

    public DlpTestFixture() { }
}

// TODO reconcile these with the "simple" tests below
public partial class DlpTest : IClassFixture<DlpTestFixture>, IDisposable
{
    private readonly DlpTestFixture _kmsFixture;
    private string ProjectId { get { return _kmsFixture.ProjectId; } }

    #region anassri_tests;

    private readonly string _resourcePath = Path.GetFullPath("../../../resources/");
    private string CallingProjectId { get { return _kmsFixture.ProjectId; } }
    private string TableProjectId { get { return "nodejs-docs-samples"; } } // TODO make retrieval more idiomatic
    private string KeyName { get { return _kmsFixture.KeyName; } }
    private string WrappedKey { get { return _kmsFixture.WrappedKey; } }

    // TODO change these
    private readonly string _bucketName = "nodejs-docs-samples";

    private readonly string _topicId = "dlp-nyan-2";
    private readonly string _subscriptionId = "nyan-dlp-2";

    // TODO keep these values, but make their retrieval more idiomatic
    private readonly string _datasetId = "integration_tests_dlp";

    private readonly string _tableId = "harmful";

    // FYI these values depend on a BQ table in nodejs-docs-samples; we should verify its publicly accessible
    private readonly string _quasiIds = "Age,Gender";

    private readonly string _quasiIdInfoTypes = "AGE,GENDER";
    private readonly string _sensitiveAttribute = "Name";

    #endregion anassri_tests;

    private readonly RetryRobot _retryRobot = new RetryRobot();

    private readonly CommandLineRunner _dlp = new CommandLineRunner()
    {
        VoidMain = Dlp.Main,
        Command = "Dlp"
    };

    public DlpTest(DlpTestFixture fixture)
    {
        _kmsFixture = fixture;
    }

    public void Dispose()
    {
        // Delete any jobs created by the test.
        DlpServiceClient dlp = DlpServiceClient.Create();
        Google.Api.Gax.PagedEnumerable<ListDlpJobsResponse, DlpJob> result = dlp.ListDlpJobs(new ListDlpJobsRequest
        {
            ParentAsProjectName = new ProjectName(ProjectId),
            Type = DlpJobType.RiskAnalysisJob
        });
        foreach (DlpJob job in result)
        {
            dlp.DeleteDlpJob(new DeleteDlpJobRequest()
            {
                Name = job.Name
            });
        }
    }

    [Fact]
    public void TestListInfoTypes()
    {
        // list all info types
        ConsoleOutput outputA = _dlp.Run("listInfoTypes");
        Assert.Contains("US_DEA_NUMBER", outputA.Stdout);
        Assert.Contains("AMERICAN_BANKERS_CUSIP_ID", outputA.Stdout);

        // list info types with a filter
        ConsoleOutput outputB = _dlp.Run(
            "listInfoTypes",
            "-f", "supported_by=RISK_ANALYSIS"
        );
        Assert.Contains("AGE", outputB.Stdout);
        Assert.DoesNotContain("AMERICAN_BANKERS_CUSIP_ID", outputB.Stdout);
    }

    [Fact]
    public void TestDeidMask()
    {
        ConsoleOutput output = _dlp.Run(
            "deidMask",
            ProjectId,
            "'My SSN is 372819127.'",
            "-n", "5",
            "-m", "*",
            "-i", "US_SOCIAL_SECURITY_NUMBER"
        );
        Assert.Contains("My SSN is *****9127", output.Stdout);
    }

    [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/510")]
    public void TestDeidentifyDates()
    {
        string InputPath = _resourcePath + "dates-input.csv";
        string OutputPath = _resourcePath + "resources/dates-shifted.csv";
        string CorrectPath = _resourcePath + "resources/dates-correct.csv";
        _ = _dlp.Run(
            "deidDateShift",
            ProjectId,
            InputPath,
            OutputPath,
            "50",
            "50",
            "birth_date,register_date",
            "name",
            WrappedKey,
            KeyName);

        Assert.Equal(
            File.ReadAllText(OutputPath),
            File.ReadAllText(CorrectPath));
    }

    [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/510")]
    public void TestDeidReidFpe()
    {
        string data = "'My SSN is 372819127'";
        string alphabet = "Numeric";

        // Deid
        ConsoleOutput deidOutput = _dlp.Run(
            "deidFpe",
            CallingProjectId,
            data,
            KeyName,
            WrappedKey,
            alphabet,
            "-i", "US_SOCIAL_SECURITY_NUMBER");
        Assert.Matches(new Regex("My SSN is TOKEN\\(9\\):\\d+"), deidOutput.Stdout);

        // Reid
        ConsoleOutput reidOutput = _dlp.Run("reidFpe", CallingProjectId, data, KeyName, WrappedKey, alphabet);
        Assert.Contains(data, reidOutput.Stdout);
    }

    [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/1006")]
    public void TestTriggers()
    {
        string triggerId = $"my-csharp-test-trigger-{Guid.NewGuid()}";
        string fullTriggerId = $"projects/{CallingProjectId}/jobTriggers/{triggerId}";
        string displayName = $"My trigger display name {Guid.NewGuid()}";
        string description = $"My trigger description {Guid.NewGuid()}";

        // Create
        ConsoleOutput createOutput = _dlp.Run(
            "createJobTrigger",
            CallingProjectId,
            "-i", "PERSON_NAME,US_ZIP",
            _bucketName,
            "1",
            "--autoPopulateTimespan",
            "-l", "Unlikely",
            "-m", "0",
            "-t", triggerId,
            "-n", displayName,
            "-d", description);
        Assert.Contains($"Successfully created trigger {fullTriggerId}", createOutput.Stdout);

        // List
        ConsoleOutput listOutput = _dlp.Run("listJobTriggers", CallingProjectId);
        Assert.Contains($"Name: {fullTriggerId}", listOutput.Stdout);
        Assert.Contains($"Display Name: {displayName}", listOutput.Stdout);
        Assert.Contains($"Description: {description}", listOutput.Stdout);

        // Delete
        ConsoleOutput deleteOutput = _dlp.Run("deleteJobTrigger", fullTriggerId);
        Assert.Contains($"Successfully deleted trigger {fullTriggerId}", deleteOutput.Stdout);
    }

    [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/510")]
    public void TestNumericalStats()
    {
        ConsoleOutput output = _dlp.Run(
            "numericalStats",
            CallingProjectId,
            TableProjectId,
            _datasetId,
            _tableId,
            _topicId,
            _subscriptionId,
            "Age"
        );

        Assert.Matches(new Regex("Value Range: \\[\\d+, \\d+\\]"), output.Stdout);
        Assert.Matches(new Regex("Value at \\d+% quantile: \\d+"), output.Stdout);
    }

    [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/510")]
    public void TestCategoricalStats()
    {
        ConsoleOutput output = _dlp.Run(
            "categoricalStats",
            CallingProjectId,
            TableProjectId,
            _datasetId,
            _tableId,
            _topicId,
            _subscriptionId,
            "Gender"
        );

        Assert.Matches(new Regex("Least common value occurs \\d+ time\\(s\\)"), output.Stdout);
        Assert.Matches(new Regex("Most common value occurs \\d+ time\\(s\\)"), output.Stdout);
        Assert.Matches(new Regex("\\d+ unique value\\(s\\) total"), output.Stdout);
    }

    [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/510")]
    public void TestKAnonymity()
    {
        ConsoleOutput output = _dlp.Run(
            "kAnonymity",
            CallingProjectId,
            TableProjectId,
            _datasetId,
            _tableId,
            _topicId,
            _subscriptionId,
            _quasiIds
        );

        Assert.Matches(new Regex("Quasi-ID values: \\[\\d{2},Female\\]"), output.Stdout);
        Assert.Matches(new Regex("Class size: \\d"), output.Stdout);
        Assert.Matches(new Regex("\\d+ unique value\\(s\\) total"), output.Stdout);
    }

    [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/510")]
    public void TestLDiversity()
    {
        ConsoleOutput output = _dlp.Run(
            "lDiversity",
            CallingProjectId,
            TableProjectId,
            _datasetId,
            _tableId,
            _topicId,
            _subscriptionId,
            _quasiIds,
            _sensitiveAttribute
        );

        Assert.Matches(new Regex("Quasi-ID values: \\[\\d{2},Female\\]"), output.Stdout);
        Assert.Matches(new Regex("Class size: \\d"), output.Stdout);
        Assert.Matches(new Regex("Sensitive value James occurs \\d time\\(s\\)"), output.Stdout);
        Assert.Matches(new Regex("\\d+ unique value\\(s\\) total"), output.Stdout);
    }

    [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/510")]
    public void TestKMap()
    {
        ConsoleOutput output = _dlp.Run(
            "kMap",
            CallingProjectId,
            TableProjectId,
            _datasetId,
            _tableId,
            _topicId,
            _subscriptionId,
            _quasiIds,
            _quasiIdInfoTypes,
            "US"
        );

        Assert.Matches(new Regex("Anonymity range: \\[\\d, \\d\\]"), output.Stdout);
        Assert.Matches(new Regex("Size: \\d"), output.Stdout);
        Assert.Matches(new Regex("Values: \\[\\d{2},Female,US\\]"), output.Stdout);
    }

    [Fact]
    public void TestJobs()
    {
        // Create job.
        DlpServiceClient dlp = DlpServiceClient.Create();
        DlpJob dlpJob = dlp.CreateDlpJob(new CreateDlpJobRequest()
        {
            ParentAsProjectName = new ProjectName(ProjectId),
            RiskJob = new RiskAnalysisJobConfig()
            {
                PrivacyMetric = new PrivacyMetric()
                {
                    CategoricalStatsConfig = new PrivacyMetric.Types.CategoricalStatsConfig()
                    {
                        Field = new FieldId()
                        {
                            Name = "zip_code"
                        }
                    }
                },
                SourceTable = new BigQueryTable()
                {
                    ProjectId = "bigquery-public-data",
                    DatasetId = "san_francisco",
                    TableId = "bikeshare_trips"
                }
            }
        });
        Regex dlpJobRegex = new Regex("projects/.*/dlpJobs/r-\\d+");

        _retryRobot.ShouldRetry = ex => true;
        _retryRobot.Eventually(() =>
        {
            // List jobs.
            ConsoleOutput listOutput = _dlp.Run("listJobs", CallingProjectId, "state=DONE", "RiskAnalysisJob");
            Assert.Matches(dlpJobRegex, listOutput.Stdout);

            // Delete created job.
            string jobName = dlpJobRegex.Match(listOutput.Stdout).Value;
            ConsoleOutput deleteOutput = _dlp.Run("deleteJob", jobName);
            Assert.Contains($"Successfully deleted job {jobName}", deleteOutput.Stdout);
        });
    }
}
