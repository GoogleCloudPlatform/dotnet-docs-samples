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

using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.IO;

namespace GoogleCloudSamples
{
    public class AuthSample
    {
        const string usage = @"Usage:AuthSample <bucket_name>";
        string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        // [START auth_cloud_implicit]
        void ImplicitAuth()
        {
            // If you don't specify credentials when constructing the client, the
            // client library will look for credentials in the environment.
            var storage = StorageClient.Create();
            // Make an authenticated API request.
            var buckets = storage.ListBuckets(_projectId);
            foreach (var bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }
        }
        // [END auth_cloud_implicit]

        void ExplicitAuth(string jsonPath)
        {
            // Explicitly use service account credentials by specifying the private key
            // file.
            GoogleCredential credential = null;
            using (var jsonStream = new FileStream(jsonPath, FileMode.Open,
                FileAccess.Read, FileShare.Read))
            {
                credential = GoogleCredential.FromStream(jsonStream);
            }
            var storage = StorageClient.Create(credential);
            // Make an authenticated API request.
            var buckets = storage.ListBuckets(_projectId);
            foreach (var bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }
        }

        void ExplicitAuthComputeEngine()
        {
            // Explicitly use Compute Engine credentials. These credentials are
            // available on Compute Engine, App Engine Flexible, and Container Engine.
            var computeCredential = new ComputeCredential();
            // How to instantiate?  This gives me a compile time error.
            var storage = StorageClient.Create(new GoogleCredential(computeCredential));
            // Make an authenticated API request.
            var buckets = storage.ListBuckets(_projectId);
            foreach (var bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }
        }

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
