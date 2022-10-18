﻿// Copyright 2020 Google Inc.
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

//TODO: remove the storage_set_bucket_public_iam tag later as it is already declared in MakeBucketPublic.cs

// [START storage_make_public]
// [START storage_set_bucket_public_iam]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;

public class MakePublicSample
{
    public string MakePublic(
        string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name")
    {
        var storage = StorageClient.Create();
        var storageObject = storage.GetObject(bucketName, objectName);
        storageObject.Acl ??= new List<ObjectAccessControl>();
        storage.UpdateObject(storageObject, new UpdateObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead });
        Console.WriteLine(objectName + " is now public and can be fetched from " + storageObject.MediaLink);

        return storageObject.MediaLink;
    }
}

// [END storage_set_bucket_public_iam]
// [END storage_make_public]
