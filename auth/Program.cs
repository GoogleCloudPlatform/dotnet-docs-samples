/*
 * Copyright (c) 2015 Google Inc.
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
// [START all]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

namespace GoogleCloudSamples
{
    public class AuthSample
    {
        const string usage = @"Usage:AuthSample <bucket_name>";

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(usage);
                return;
            }
            AuthSample sample = new AuthSample();
            string bucketName = args[0];
            // [START build_service]
            // Create a new Cloud Storage client authorized via Application 
            // Default Credentials
            var storage = StorageClient.Create();
            // [END build_service]
            Console.WriteLine(
                "======= Listing Cloud Storage Bucket's contents =======");
            Console.WriteLine();
            // [START list_storage_bucket_contents]
            // Use the Cloud Storage client to get a list of objects for the
            // given bucket name
            foreach (var objectName in storage.ListObjects(bucketName, ""))
            {
                Console.WriteLine(objectName.Name);
            }
            // [END list_storage_bucket_contents]
        }
    }
}
// [END all]
