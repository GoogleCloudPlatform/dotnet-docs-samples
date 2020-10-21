// Copyright 2020 Google Inc.
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

using Google.Cloud.Datastore.V1;
using Google.Cloud.Storage.V1;
using System;
using Xunit;

[CollectionDefinition(nameof(DatastoreAdminFixture))]
public class DatastoreAdminFixture : ICollectionFixture<DatastoreAdminFixture>, IDisposable
{
    public string ProjectId { get; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    public string BucketName { get; } = Guid.NewGuid().ToString();
    public string Namespace { get; } = Guid.NewGuid().ToString();
    public string Kind { get; } = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-Task";

    public DatastoreAdminFixture()
    {
        CreateKeyFactory();
        CreateBucket();
    }

    void CreateKeyFactory()
    {
        DatastoreDb datastoreDb = DatastoreDb.Create(ProjectId, Namespace);
        KeyFactory keyFactory = datastoreDb.CreateKeyFactory(Kind);
        Key key = keyFactory.CreateKey("sampletask");
        var task = new Entity
        {
            Key = key,
            ["description"] = "Buy milk"
        };
        datastoreDb.Insert(task);
    }

    void CreateBucket()
    {
        var storage = StorageClient.Create();
        storage.CreateBucket(ProjectId, BucketName);
    }

    public void Dispose()
    {
        try
        {
            DatastoreDb datastoreDb = DatastoreDb.Create(ProjectId, Namespace);
            var deadEntities = datastoreDb.RunQuery(new Query(Kind));
            datastoreDb.Delete(deadEntities.Entities);
        }
        catch (Exception)
        {
            // Do nothing, we delete on a best effort basis.
        }

        try
        {
            var storage = StorageClient.Create();
            var storageObjects = storage.ListObjects(BucketName);
            foreach (var storageObject in storageObjects)
            {
                storage.DeleteObject(BucketName, storageObject.Name);
            }
            storage.DeleteBucket(BucketName);
        }
        catch (Exception)
        {
            // Do nothing, we delete on a best effort basis.
        }
    }
}
