// Copyright(c) 2018 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.
//

using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using Google.Cloud.Storage.V1;
using GoogleCloudSamples;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

public class BigQueryTest : IDisposable, IClassFixture<RandomBucketFixture>
{
    private readonly string _projectId;
    private readonly string _jsonPath;
    private readonly BigQueryClient _client;
    readonly List<BigQueryDataset> _tempDatasets = new List<BigQueryDataset>();
    readonly StorageClient _storage;
    readonly string _bucketName;
    readonly TextWriter _consoleOut = Console.Out;
    readonly StringWriter _stringOut;

    public BigQueryTest(RandomBucketFixture randomBucketFixture)
    {
        _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        _jsonPath = System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
        _client = BigQueryClient.Create(_projectId);
        _storage = StorageClient.Create();
        _bucketName = randomBucketFixture.BucketName;
        _stringOut = new StringWriter();
        Console.SetOut(_stringOut);
    }

    [Fact]
    public void TestAuthenticateServiceAccount()
    {
        var snippet = new BigQueryAuthenticateServiceAccount();
        snippet.AuthenticateWithServiceAccount(_projectId, _jsonPath);
        var outputLines = _stringOut.ToString().Trim().Split(Environment.NewLine);
        Assert.Equal(100, outputLines.Count());
    }

    [Fact]
    public void TestBrowseTable()
    {
        var snippet = new BigQueryBrowseTable();
        snippet.BrowseTable(_projectId);
        var outputLines = _stringOut.ToString().Trim().Split(Environment.NewLine);
        Assert.Equal(10, outputLines.Count());
    }

    [Fact]
    public void TestCopyTable()
    {
        var snippet = new BigQueryCopyTable();
        string destinationDatasetId = CreateTempDataset();
        snippet.CopyTable(_projectId, destinationDatasetId);
        var output = _stringOut.ToString();
        var numRows = Int64.Parse(Regex.Match(output, @"\d+").Value);
        Assert.True(numRows > 0);
    }

    [Fact]
    public void TestCreateDataset()
    {
        var snippet = new BigQueryCreateDataset();
        BigQueryDataset dataset = snippet.CreateDataset(_projectId);
        Assert.Equal("US", dataset.Resource.Location);
        dataset.Delete();
    }

    [Fact]
    public void TestCreateJob()
    {
        var snippet = new BigQueryCreateJob();
        BigQueryJob job = snippet.CreateJob(_projectId);
        Assert.NotNull(job);
    }

    [Fact]
    public void TestCreateTable()
    {
        var snippet = new BigQueryCreateTable();
        string datasetId = CreateTempDataset();
        BigQueryTable table = snippet.CreateTable(_projectId, datasetId);
        Assert.Equal("your_table_id", table.Resource.TableReference.TableId);
    }

    [Fact]
    public void TestCreateTableRangePartitioned()
    {
        var snippet = new BigQueryCreateTableRangePartitioned();
        string datasetId = CreateTempDataset();
        BigQueryTable table = snippet.CreateTable(_projectId, datasetId, "partitioned-table");
        Assert.NotNull(table.Resource.RangePartitioning);
    }

    [Fact]
    public void TestDeleteDataset()
    {
        var snippet = new BigQueryDeleteDataset();
        string datasetId = TestUtil.RandomName();
        _client.CreateDataset(datasetId);
        snippet.DeleteDataset(_projectId, datasetId);
        var output = _stringOut.ToString();
        Assert.Contains($"{datasetId} deleted", output);
    }

    [Fact]
    public void TestDeleteDatasetAndContents()
    {
        var snippet = new BigQueryDeleteDatasetAndContents();
        string datasetId = TestUtil.RandomName();
        _client.CreateDataset(datasetId);
        CreateTempEmptyTable(datasetId);
        snippet.DeleteDatasetAndContents(_projectId, datasetId);
        var output = _stringOut.ToString();
        Assert.Contains($"{datasetId} and contents deleted", output);
    }

    [Fact]
    public void TestDeleteTable()
    {
        var snippet = new BigQueryDeleteTable();
        string datasetId = CreateTempDataset();
        string tableId = TestUtil.RandomName();
        _client.CreateTable(datasetId, tableId, new Table());
        snippet.DeleteTable(_projectId, datasetId, tableId);
        var output = _stringOut.ToString();
        Assert.Contains($"{tableId} deleted", output);
    }

    [Fact]
    public void TestExtractModel()
    {
        string datasetId = CreateTempDataset();
        string modelId = TestUtil.RandomName();
        string createModelSql = $@"
CREATE MODEL {datasetId}.{modelId}
OPTIONS
  (model_type='linear_reg',
    input_label_cols=['label'],
    max_iterations = 1,
    learn_rate=0.4,
    learn_rate_strategy='constant') AS
SELECT 'a' AS f1, 2.0 AS label
UNION ALL
SELECT 'b' AS f1, 3.8 AS label";

        var createModelJob = _client.CreateQueryJob(createModelSql, null);
        createModelJob.PollUntilCompleted().ThrowOnAnyError();
        Assert.NotNull(_client.GetModel(datasetId, modelId));

        var snippet = new BigQueryExtractModel();
        snippet.ExtractModel(_projectId, datasetId, modelId, $"gs://{_bucketName}/model");
        var modelFiles = _storage.ListObjects(_bucketName, "model/").ToList();
        // We end up with multiple files for the model.
        Assert.True(modelFiles.Count > 4);
    }

    [Fact]
    public void TestExtractTable()
    {
        var snippet = new BigQueryExtractTable();
        snippet.ExtractTable(_projectId, _bucketName);
        var uploadedFile = _storage.ListObjects(_bucketName, "shakespeare").First();
        Assert.True(uploadedFile.Size > 0);
    }

    [Fact]
    public void TestExtractTableJson()
    {
        var snippet = new BigQueryExtractTableJson();
        snippet.ExtractTableJson(_projectId, _bucketName);
        var uploadedFile = _storage.GetObject(_bucketName, "shakespeare.json");
        Assert.True(uploadedFile.Size > 0);
    }

    [Fact]
    public void TestListDatasets()
    {
        var snippet = new BigQueryListDatasets();
        var datasetId1 = CreateTempDataset();
        var datasetId2 = CreateTempDataset();
        snippet.ListDatasets(_projectId);
        var output = _stringOut.ToString();
        Assert.Contains($"{datasetId1}", output);
        Assert.Contains($"{datasetId2}", output);
    }

    [Fact]
    public void TestListTables()
    {
        var snippet = new BigQueryListTables();
        var datasetId = CreateTempDataset();
        var tableId1 = CreateTempEmptyTable(datasetId);
        var tableId2 = CreateTempEmptyTable(datasetId);
        snippet.ListTables(_projectId, datasetId);
        var output = _stringOut.ToString();
        Assert.Contains($"{tableId1}", output);
        Assert.Contains($"{tableId2}", output);
    }

    [Fact]
    public void TestLoadFromFile()
    {
        var snippet = new BigQueryLoadFromFile();
        var datasetId = CreateTempDataset();
        var tableId = CreateTempEmptyTable(datasetId);
        string filePath = Path.Combine("data", "sample.csv");
        snippet.LoadFromFile(_projectId, datasetId, tableId, filePath);
        var output = _stringOut.ToString();
        long numRows = Convert.ToInt64(Regex.Match(output, @"\d+").Value);
        Assert.Equal(4, numRows);
    }

    [Fact]
    public void TestLoadTableGcsCsv()
    {
        var snippet = new BigQueryLoadTableGcsCsv();
        var datasetId = CreateTempDataset();
        snippet.LoadTableGcsCsv(_projectId, datasetId);
        var output = _stringOut.ToString();
        long numRows = Convert.ToInt64(Regex.Match(output, @"\d+").Value);
        Assert.Equal(50, numRows);
    }

    [Fact]
    public void TestLoadTableGcsJson()
    {
        var snippet = new BigQueryLoadTableGcsJson();
        var datasetId = CreateTempDataset();
        snippet.LoadTableGcsJson(_projectId, datasetId);
        var output = _stringOut.ToString();
        long numRows = Convert.ToInt64(Regex.Match(output, @"\d+").Value);
        Assert.Equal(50, numRows);
    }

    [Fact]
    public void TestLoadTableGcsOrc()
    {
        var snippet = new BigQueryLoadTableGcsOrc();
        var datasetId = CreateTempDataset();
        snippet.LoadTableGcsOrc(_projectId, datasetId);

        var truncateSnippet = new BigQueryLoadTableGcsOrcTruncate();
        truncateSnippet.LoadTableGcsOrcTruncate(
            _projectId, datasetId, "us_states");

        var output = _stringOut.ToString();
        // Snippet runs should report 50 output rows twice
        Assert.Equal(2, Regex.Matches(output, "50").Count);
    }

    [Fact]
    public void TestQuery()
    {
        var snippet = new BigQueryQuery();
        snippet.Query(_projectId);
        var outputLines = _stringOut.ToString().Trim().Split(Environment.NewLine);
        Assert.Equal(100, outputLines.Count());
    }

    [Fact]
    public void TestQueryLegacy()
    {
        var snippet = new BigQueryQueryLegacy();
        snippet.QueryLegacy(_projectId);
        var outputLines = _stringOut.ToString().Trim().Split(Environment.NewLine);
        Assert.Equal(100, outputLines.Count());
    }

    [Fact]
    public void TestQueryWithArrayParameters()
    {
        var snippet = new BigQueryQueryWithArrayParameters();
        snippet.QueryWithArrayParameters(_projectId);
        var outputLines = _stringOut.ToString().Trim().Split(Environment.NewLine);
        Assert.Equal(10, outputLines.Count());
    }

    [Fact]
    public void TestQueryWithNamedParameters()
    {
        var snippet = new BigQueryQueryWithNamedParameters();
        snippet.QueryWithNamedParameters(_projectId);
        var outputLines = _stringOut.ToString().Trim().Split(Environment.NewLine);
        Assert.Equal(12, outputLines.Count());
    }

    [Fact]
    public void TestQueryWithPositionalParameters()
    {
        var snippet = new BigQueryQueryWithPositionalParameters();
        snippet.QueryWithPositionalParameters(_projectId);
        var outputLines = _stringOut.ToString().Trim().Split(Environment.NewLine);
        Assert.Equal(12, outputLines.Length);
    }

    [Fact]
    public void TestQueryWithTimestampParameters()
    {
        var snippet = new BigQueryQueryWithTimestampParameters();
        snippet.QueryWithTimestampParameters(_projectId);
        var outputLines = _stringOut.ToString().Trim().Split(Environment.NewLine);
        Assert.Single(outputLines);
    }

    [Fact]
    public void TestTableInsertRows()
    {
        var snippet = new BigQueryTableInsertRows();
        var datasetId = CreateTempDataset();
        var tableId = CreateTempUsStatesTable(datasetId);
        snippet.TableInsertRows(_projectId, datasetId, tableId);
    }

    public string CreateTempDataset()
    {
        string datasetId = TestUtil.RandomName();
        BigQueryDataset tempDataset = _client.CreateDataset(
            datasetId, new Dataset { Location = "US" });
        _tempDatasets.Add(tempDataset);
        return datasetId;
    }

    public string CreateTempEmptyTable(string datasetId)
    {
        string tableId = TestUtil.RandomName();
        _client.CreateTable(datasetId, tableId, new Table());
        return tableId;
    }

    public string CreateTempUsStatesTable(string datasetId)
    {
        var snippet = new BigQueryLoadTableGcsCsv();
        snippet.LoadTableGcsCsv(_projectId, datasetId);
        return "us_states";  // The table ID defined in the snippet
    }

    public void Dispose()
    {
        foreach (BigQueryDataset dataset in _tempDatasets)
        {
            var deleteDatasetOptions = new DeleteDatasetOptions()
            {
                DeleteContents = true
            };
            dataset.Delete(deleteDatasetOptions);
        }
        Console.SetOut(_consoleOut);
    }
}
