// Copyright(c) 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using CommandLine;
using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GoogleCloudSamples
{
    // [START fs_index_field]
    public class IndexField
    {
        public string fieldPath;
        public string mode;
    }
    // [END fs_index_field]

    // [START fs_index_content]
    public class IndexContent
    {
        public string collectionId;
        public IndexField[] fields;

        public IndexContent(string collection, string field1, string order1, string field2, string order2)
        {
            collectionId = collection;
            IndexField indexField1 = new IndexField();
            indexField1.fieldPath = field1;
            indexField1.mode = order1;
            IndexField indexField2 = new IndexField();
            indexField2.fieldPath = field2;
            indexField2.mode = order2;
            IndexField[] fieldsArray = new IndexField[2];
            fieldsArray[0] = indexField1;
            fieldsArray[1] = indexField2;
            fields = fieldsArray;
        }
    }
    // [END fs_index_content]

    public class ManageIndexes
    {
        public static string Usage = @"Usage:
C:\> dotnet run command YOUR_PROJECT_ID

Where command is one of
    count-indexes
    create-indexes
    delete-indexes
";

        // Counts the number of created indexes for a given collection in a project.
        // [START fs_count_indexes]
        private static int CountIndexes(string project, string collectionId)
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefault();
            // Inject the Cloud Platform scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    "https://www.googleapis.com/auth/cloud-platform"
                });
            }
            HttpClient http = new Google.Apis.Http.HttpClientFactory()
                .CreateHttpClient(
                new Google.Apis.Http.CreateHttpClientArgs()
                {
                    ApplicationName = "Google Cloud Platform Firestore Sample",
                    GZipEnabled = true,
                    Initializers = { credential },
                });
            string uriString = "https://firestore.googleapis.com/v1beta1/projects/" + project + "/databases/(default)/indexes";
            UriBuilder uri = new UriBuilder(uriString);
            var resultText = http.GetAsync(uri.Uri).Result.Content
                .ReadAsStringAsync().Result;
            dynamic result = Newtonsoft.Json.JsonConvert
                .DeserializeObject(resultText);

            int numIndexesCreated = 0;
            if (result.indexes != null)
            {
                foreach (var index in result.indexes)
                {
                    Console.WriteLine(index.name);
                    if (index.collectionId == collectionId & index.state == "READY")
                    {
                        numIndexesCreated = numIndexesCreated + 1;
                    }
                }
            }
            return numIndexesCreated;
        }
        // [END fs_count_indexes]

        // Creates an index for a given collection in a project.
        // [START fs_create_index]
        private static async Task CreateIndex(string project, string collectionId, string field1, string order1, string field2, string order2)
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefault();
            // Inject the Cloud Platform scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    "https://www.googleapis.com/auth/cloud-platform"
                });
            }
            HttpClient http = new Google.Apis.Http.HttpClientFactory()
                .CreateHttpClient(
                new Google.Apis.Http.CreateHttpClientArgs()
                {
                    ApplicationName = "Google Cloud Platform Firestore Sample",
                    GZipEnabled = true,
                    Initializers = { credential },
                });

            IndexContent indexContent = new IndexContent(collectionId, field1, order1, field2, order2);
            string jsonRequest = JsonConvert.SerializeObject(indexContent);
            string uriString = "https://firestore.googleapis.com/v1beta1/projects/" + project + "/databases/(default)/indexes";
            UriBuilder uri = new UriBuilder(uriString);
            await http.PostAsync(uri.Uri, new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json"));
        }
        // [END fs_create_index]

        // Retrieves all indexes for a given collection in a project and deletes them.
        // [START fs_delete_indexes]
        private static async Task DeleteIndexes(string project, string collectionId)
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefault();
            // Inject the Cloud Platform scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    "https://www.googleapis.com/auth/cloud-platform"
                });
            }
            HttpClient http = new Google.Apis.Http.HttpClientFactory()
                .CreateHttpClient(
                new Google.Apis.Http.CreateHttpClientArgs()
                {
                    ApplicationName = "Google Cloud Platform Firestore Sample",
                    GZipEnabled = true,
                    Initializers = { credential },
                });

            string uriString = "https://firestore.googleapis.com/v1beta1/projects/" + project + "/databases/(default)/indexes";
            UriBuilder uri = new UriBuilder(uriString);
            var resultText = http.GetAsync(uri.Uri).Result.Content
                .ReadAsStringAsync().Result;
            dynamic result = Newtonsoft.Json.JsonConvert
                .DeserializeObject(resultText);

            List<string> indexesToBeDeleted = new List<string>();

            if (result.indexes != null)
            {
                foreach (var index in result.indexes)
                {
                    if (index.collectionId == collectionId)
                    {
                        string name = index.name;
                        indexesToBeDeleted.Add(name);
                    }
                }
            }

            foreach (string indexToBeDeleted in indexesToBeDeleted)
            {
                uriString = "https://firestore.googleapis.com/v1beta1/" + indexToBeDeleted;
                UriBuilder deleteUri = new UriBuilder(uriString);
                await http.DeleteAsync(deleteUri.Uri);
            }
            Console.WriteLine("Index deletion completed!");
        }
        // [END fs_delete_indexes]

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Write(Usage);
                return;
            }
            string command = args[0].ToLower();
            string project = string.Join(" ",
                new ArraySegment<string>(args, 1, args.Length - 1));
            switch (command)
            {
                case "count-indexes":
                    CountIndexes(project, "YOUR_COLLECTION_NAME");
                    break;

                case "create-indexes":
                    CreateIndex(project, "YOUR_COLLECTION_NAME", "Name", "ASCENDING", "State", "ASCENDING").Wait();
                    CreateIndex(project, "YOUR_COLLECTION_NAME", "State", "ASCENDING", "Population", "ASCENDING").Wait();
                    CreateIndex(project, "YOUR_COLLECTION_NAME", "State", "ASCENDING", "Population", "DESCENDING").Wait();
                    int numIndexesCreated = CountIndexes(project, "YOUR_COLLECTION_NAME");
                    while (numIndexesCreated < 3)
                    {
                        Console.WriteLine("Index creation still in progress...");
                        System.Threading.Thread.Sleep(10000);
                        numIndexesCreated = CountIndexes(project, "YOUR_COLLECTION_NAME");
                    }
                    Console.WriteLine("Index creation completed!");
                    break;

                case "delete-indexes":
                    DeleteIndexes(project, "YOUR_COLLECTION_NAME").Wait();
                    break;

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}
