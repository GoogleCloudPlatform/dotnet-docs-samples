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

// [START storage_print_file_acl_for_user]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Linq;

public class PrintFileAclForUserSample
{
    public IEnumerable<ObjectAccessControl> PrintFileAclForUser(
        string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name",
        string userEmail = "user@iam.gserviceaccount.com")
    {
        var storage = StorageClient.Create();
        var storageObject = storage.GetObject(bucketName, objectName, new GetObjectOptions
        {
            Projection = Projection.Full
        });

        var fileAclForUser = storageObject.Acl.Where((acl) => acl.Entity == $"user-{userEmail}");
        foreach (var acl in fileAclForUser)
        {
            Console.WriteLine($"{acl.Role}:{acl.Entity}");
        }

        return fileAclForUser;
    }
}
// [END storage_print_file_acl_for_user]
