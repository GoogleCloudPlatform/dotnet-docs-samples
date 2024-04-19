// Copyright 2024 Google LLC
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

// [START storage_set_object_retention_policy]

using Google.Cloud.Storage.V1;
using System;
using Object = Google.Apis.Storage.v1.Data.Object;

public class SetObjectRetentionPolicySample
{
    public Google.Apis.Storage.v1.Data.Object SetObjectRetentionPolicy(
        string bucketName = "your-unique-bucket-name", // A bucket that has object retention enabled
        string objectName = "your-object-name") // An object that does not have a retention policy set
    {
        var storage = StorageClient.Create();
        var file = storage.GetObject(bucketName, objectName);


        file.Retention = new Object.RetentionData
        {
            Mode = "Unlocked", RetainUntilTimeDateTimeOffset = DateTimeOffset.UtcNow.AddDays(10)
        };

        file = storage.UpdateObject(file);

        Console.WriteLine($"The retention policy for object {objectName} was set to:");
        Console.WriteLine("Mode: " + file.Retention.Mode);
        Console.WriteLine("RetainUntilTime: " + file.Retention.RetainUntilTimeDateTimeOffset.ToString());

        // To modify an existing policy on an Unlocked object, pass in the override parameter
        file.Retention = new Object.RetentionData
        {
            Mode = "Unlocked", RetainUntilTimeDateTimeOffset = DateTimeOffset.UtcNow.AddDays(9)
        };

        file = storage.UpdateObject(file, new UpdateObjectOptions { OverrideUnlockedRetention = true });

        Console.WriteLine($"The retention policy for object {objectName} was updated to:");
        Console.WriteLine("Mode: " + file.Retention.Mode);
        Console.WriteLine("RetainUntilTime: " + file.Retention.RetainUntilTimeDateTimeOffset.ToString());

        return file;
    }
}
// [END storage_set_object_retention_policy]
