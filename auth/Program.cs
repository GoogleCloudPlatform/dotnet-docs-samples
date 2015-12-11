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
using Google.Apis.Storage.v1;
using Google.Apis.Storage.v1.Data;
using Google.Apis.Services;
using System;
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    public class AuthSample
    {
        const string usage = @"Usage:AuthSample <bucket_name>";
        // [START build_service]
        /// <summary>
        /// Creates an authorized Cloud Storage client service using Application 
        /// Default Credentials.
        /// </summary>
        /// <returns>an authorized Cloud Storage client.</returns>
        public StorageService CreateAuthorizedClient()
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefaultAsync().Result;
            // Inject the Cloud Storage scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    StorageService.Scope.DevstorageReadOnly
                });
            }
            return new StorageService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DotNet Google Cloud Platform Auth Sample",
            });
        }
        // [END build_service]

        // [START list_storage_bucket_contents]
        /// <summary>
        /// List the contents of a Cloud Storage bucket.
        /// </summary>
        /// <param name="bucket">the name of the Cloud Storage bucket.</param>
        ///<returns>a list of the contents of the specified bucket.</returns>
        public Objects ListBucketContents(
            StorageService storage, string bucket)
        {
            var request = new
                Google.Apis.Storage.v1.ObjectsResource.ListRequest(storage,
                bucket);
            var requestResult = request.Execute();
            return requestResult;
        }
        // [END list_storage_bucket_contents]

        private static void Main(string[] args)
        {
            SamplesUtil.InvokeMain(() =>
            {
                var sample = new AuthSample();
                sample.MainFunction(args);
            });
        }

        void MainFunction(string[] args)
        {
            AuthSample sample = new AuthSample();
            string bucket = null;
            if (args.Length == 0)
            {
                Console.WriteLine(usage);
                return;
            }
            bucket = args[0];
            // Create a new Cloud Storage client authorized via Application 
            // Default Credentials
            StorageService storage = CreateAuthorizedClient();

            try
            {
                // Use the Cloud Storage client to get a list of objects for the
                // given bucket name
                Objects result = ListBucketContents(storage, bucket);

                // Get enumerator to loop through list of objects
                var resultsList = result.Items.GetEnumerator();

                Console.WriteLine(
                    "======= Listing Cloud Storage Bucket's contents =======");
                Console.WriteLine();

                // Loop through objects list, output object name and timestamp
                while (resultsList.MoveNext())
                {
                    if (!resultsList.Current.Equals(null))
                    {
                        // Output object name and creation timestamp
                        Console.WriteLine(resultsList.Current.Name.ToString());
                        Console.WriteLine(
                            resultsList.Current.TimeCreated.ToString());
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException ||
                    ex is Google.GoogleApiException)
                {
                    // No contents found for given bucket
                    Console.WriteLine("No contents found for given bucket. "
                        + "Sign in to the Google Developers Console");
                    Console.WriteLine(
                        "at: https://console.developers.google.com/storage ");
                    Console.WriteLine("to confirm your bucket name is valid "
                        + "and to upload some files to your bucket.");
                }
            }
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
// [END all]
