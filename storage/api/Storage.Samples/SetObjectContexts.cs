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

// [START storage_set_object_contexts]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;

public class SetObjectContextsSample
{
    /// <summary>
    /// Sets the context data associated with the object.
    /// </summary>
    /// <param name="bucketName">The name of the bucket containing the object.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="customContexts">The new user-defined contexts to set context data associated with the object.</param>
    public Google.Apis.Storage.v1.Data.Object.ContextsData SetObjectContexts(
        string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name",
        IDictionary<string, ObjectCustomContextPayload> customContexts = null)
    {
        var storage = StorageClient.Create();
        customContexts ??= new Dictionary<string, ObjectCustomContextPayload>();
        var contextsData = new Google.Apis.Storage.v1.Data.Object.ContextsData { Custom = customContexts };
        var obj = new Google.Apis.Storage.v1.Data.Object
        {
            Bucket = bucketName,
            Name = objectName,
            Contexts = contextsData
        };
        var updatedObject = storage.UpdateObject(obj);
        Console.WriteLine($"Updated Context Data for the Object {objectName} in the Bucket {bucketName}");
        return updatedObject.Contexts;
    }
}
// [END storage_set_object_contexts]
