// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

// [START storage_get_object_contexts]

using Google.Cloud.Storage.V1;
using System;

public class GetObjectContextsSample
{
    /// <summary>
    /// Gets the context data associated with the object.
    /// </summary>
    /// <param name="bucketName">The name of the bucket containing the object.</param>
    /// <param name="objectName">The name of the object.</param>
    public Google.Apis.Storage.v1.Data.Object.ContextsData GetObjectContexts(
        string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name")
    {
        var storage = StorageClient.Create();
        var obj = storage.GetObject(bucketName, objectName);
        Google.Apis.Storage.v1.Data.Object.ContextsData contextsData = obj.Contexts;

        if (contextsData?.Custom == null || contextsData.Custom.Count == 0)
        {
            Console.WriteLine($"No Context Data found for the Object {objectName}");
            return contextsData;
        }

        Console.WriteLine($"Context Data for the Object {objectName} is as follows:");
        foreach (var (key, contextPayload) in contextsData.Custom)
        {
            Console.WriteLine($"Context Key: {key}, Context Value: {contextPayload.Value}");
        }
        return contextsData;
    }
}
// [END storage_get_object_contexts]
