// Copyright(c) 2018 Google Inc.
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

using System;
using System.Collections.Generic;
using System.IO;
using Google.Cloud.BigQuery.V2;
using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;
using GoogleCloudSamples;
using Xunit;

public class BigQueryTest : IDisposable
{
    private readonly string _projectId;
    private readonly BigQueryClient _client;
    List<BigQueryDataset> tempDatasets = new List<BigQueryDataset>();
    readonly StorageClient _storage;
    readonly string _bucketName;

    public BigQueryTest()
    {
        _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        _client = BigQueryClient.Create(_projectId);
        _storage = StorageClient.Create();
        _bucketName = TestUtil.RandomName();
        _storage.CreateBucket(_projectId, _bucketName);
    }

    [Fact]
    public void TestBrowseTable()
    {
        var snippet = new BigQueryBrowseTable();
        int recordCount = snippet.BrowseTable(_projectId);
        Assert.True(recordCount > 0);
    }

    [Fact]
    public void TestCopyTable()
    {
        var snippet = new BigQueryCopyTable();
        string destinationDatasetId = CreateTempDataset();
        BigQueryTable table = snippet.CopyTable(_projectId, destinationDatasetId);
    }

    [Fact]
    public void TestCreateDataset()
    {
        var snippet = new BigQueryCreateDataset();
        BigQueryDataset dataset = snippet.CreateDataset(_projectId);
        dataset.Delete();
    }

    [Fact]
    public void TestCreateTable()
    {
        var snippet = new BigQueryCreateTable();
        string datasetId = CreateTempDataset();
        BigQueryTable table = snippet.CreateTable(_projectId, datasetId);
    }

    [Fact]
    public void TestDeleteDataset()
    {
        var snippet = new BigQueryDeleteDataset();
        string datasetId = TestUtil.RandomName();
        BigQueryDataset tempDataset = _client.CreateDataset(datasetId);
        snippet.DeleteDataset(_projectId, datasetId);
    }

    [Fact]
    public void TestDeleteTable()
    {
        var snippet = new BigQueryDeleteTable();
        string datasetId = CreateTempDataset();
        string tableId = TestUtil.RandomName();
        BigQueryTable table = _client.CreateTable(datasetId, tableId, null);
        snippet.DeleteTable(_projectId, datasetId, tableId);
    }

    [Fact]
    public void TestExtractTable()
    {
        var snippet = new BigQueryExtractTable();
        snippet.ExtractTable(_projectId, _bucketName);
    }

    [Fact]
    public void TestExtractTableJson()
    {
        var snippet = new BigQueryExtractTableJson();
        snippet.ExtractTableJson(_projectId, _bucketName);
    }

    [Fact]
    public void TestListDatasets()
    {
        var snippet = new BigQueryListDatasets();
        var datasetId1 = CreateTempDataset();
        var datasetId2 = CreateTempDataset();
        snippet.ListDatasets(_projectId);
    }

    [Fact]
    public void TestListTables()
    {
        var snippet = new BigQueryListTables();
        var datasetId = CreateTempDataset();
        var tableId1 = CreateTempEmptyTable(datasetId);
        var tableId2 = CreateTempEmptyTable(datasetId);
        snippet.ListTables(_projectId, datasetId);
    }

    [Fact]
    public void TestLoadFromFile()
    {
        var snippet = new BigQueryLoadFromFile();
        var datasetId = CreateTempDataset();
        var tableId = CreateTempEmptyTable(datasetId);
        string filePath = Path.Combine("data", "sample.csv");
        snippet.LoadFromFile(_projectId, datasetId, tableId, filePath);
    }

    [Fact]
    public void TestLoadTableGcsCsv()
    {
        var snippet = new BigQueryLoadTableGcsCsv();
        var datasetId = CreateTempDataset();
        snippet.LoadTableGcsCsv(_projectId, datasetId);
    }

    [Fact]
    public void TestLoadTableGcsJson()
    {
        var snippet = new BigQueryLoadTableGcsJson();
        var datasetId = CreateTempDataset();
        snippet.LoadTableGcsJson(_projectId, datasetId);
    }

    [Fact]
    public void TestLoadTableGcsOrc()
    {
        var snippet = new BigQueryLoadTableGcsOrc();
        var datasetId = CreateTempDataset();
        snippet.LoadTableGcsOrc(_projectId, datasetId);

        var truncateSnippet = new BigQueryLoadTableGcsOrcTruncate();
        truncateSnippet.LoadTableGcsOrcTruncate(_projectId, datasetId, "us_states");
    }

    [Fact]
    public void TestQuery()
    {
        var snippet = new BigQueryQuery();
        snippet.Query(_projectId);
    }

    [Fact]
    public void TestQueryLegacy()
    {
        var snippet = new BigQueryQueryLegacy();
        snippet.QueryLegacy(_projectId);
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
        BigQueryDataset tempDataset = _client.CreateDataset(datasetId);
        tempDatasets.Add(tempDataset);
        return datasetId;
    }

    public string CreateTempEmptyTable(string datasetId)
    {
        string tableId = TestUtil.RandomName();
        BigQueryTable table = _client.CreateTable(datasetId, tableId, null);
        return tableId;
    }

    public string CreateTempUsStatesTable(string datasetId)
    {
        var snippet = new BigQueryLoadTableGcsCsv();
        BigQueryTable table = snippet.LoadTableGcsCsv(_projectId, datasetId);
        return table.Reference.TableId;
    }
    public void Dispose()
    {
        foreach (BigQueryDataset dataset in tempDatasets) {
            var deleteDatasetOptions = new DeleteDatasetOptions()
            {
                DeleteContents = true
            };
            dataset.Delete(deleteDatasetOptions);
        }
        _storage.DeleteBucket(
            _bucketName,
            new DeleteBucketOptions() { DeleteObjects = true }
        );
    }
}
