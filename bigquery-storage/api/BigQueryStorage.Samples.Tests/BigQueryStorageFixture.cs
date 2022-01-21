/*
 * Copyright 2022 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using Grpc.Core;
using System;
using System.Collections.Generic;
using Xunit;

[CollectionDefinition(nameof(BigQueryStorageFixture))]
public class BigQueryStorageFixture : IDisposable, ICollectionFixture<BigQueryStorageFixture>
{
    internal BigQueryClient Client { get; }

    internal List<string> TempDatasetIds { get; } = new List<string>();

    internal string ProjectId { get; }

    public BigQueryStorageFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrWhiteSpace(ProjectId))
        {
            throw new Exception("Missing GOOGLE_PROJECT_ID. You need to set the Environment variable 'GOOGLE_PROJECT_ID' with your Google Cloud Project's project id.");
        }

        Client = BigQueryClient.Create(ProjectId);
    }

    /// <summary>
    /// Ensures the dataset and table with given name and schema exists
    /// </summary>
    /// <param name="datasetId">The BigQuery Dataset Id</param>
    /// <param name="tableName">The BigQuery table name</param>
    /// <param name="tableSchema">The schema for table</param>
    internal void EnsureDataSetAndTableExists(string datasetId, string tableName, TableSchema tableSchema)
    {
        BigQueryDataset dataSet = Client.GetOrCreateDataset(datasetId);
        if (!TempDatasetIds.Contains(datasetId))
        {
            TempDatasetIds.Add(datasetId);
        }

        dataSet.GetOrCreateTable(tableName, tableSchema);
    }

    /// <summary>
    ///  Disposes the instance of BigQueryStorageFixture
    /// </summary>
    public void Dispose()
    {
        try
        {
            foreach (string datasetId in TempDatasetIds)
            {
                DeleteDataset(datasetId);
            }            
        }
        catch (RpcException)
        {
            // Do nothing, we are deleting on a best effort basis.
        }
    }

    /// <summary>
    /// Deletes the dataset 
    /// </summary>
    internal void DeleteDataset(string datasetId) =>
        Client.DeleteDataset(datasetId, new DeleteDatasetOptions { DeleteContents = true });
}
