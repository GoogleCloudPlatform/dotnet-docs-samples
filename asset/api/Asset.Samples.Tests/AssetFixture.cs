/*
 * Copyright (c) 2020 Google LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using GoogleCloudSamples;
using System;
using System.Threading;
using Xunit;

[CollectionDefinition(nameof(AssetFixture))]
public class AssetFixture : IDisposable, ICollectionFixture<AssetFixture>
{
    public string ProjectId { get; }
    public string BucketName { get; }
    public string DatasetId { get; }
    private readonly RandomBucketFixture _bucketFixture;
    private readonly BigQueryClient _bigQueryClient;

    public AssetFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        _bucketFixture = new RandomBucketFixture();
        BucketName = _bucketFixture.BucketName;

        _bigQueryClient = BigQueryClient.Create(ProjectId);
        DatasetId = RandomDatasetId();
        var dataset = new Dataset
        {
            Location = "US"
        };
        _bigQueryClient.CreateDataset(datasetId: DatasetId, dataset);

        // Wait 10 seconds to let resource creation events go to CAI.
        Thread.Sleep(10000);
    }

    public void Dispose()
    {
        _bucketFixture.Dispose();
        _bigQueryClient.DeleteDataset(datasetId: DatasetId);
    }

    public string RandomDatasetId()
    {
        return $"asset-csharp-{System.Guid.NewGuid()}".Replace('-', '_');
    }
}
