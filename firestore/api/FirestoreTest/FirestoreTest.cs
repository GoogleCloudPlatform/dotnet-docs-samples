/*
 * Copyright (c) 2018 Google Inc.
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

using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GoogleCloudSamples
{
    public class FirestoreFixture : IDisposable
    {
        // Clean-up function to delete all documents in a collection
        private static async Task DeleteCollection(string collection)
        {
            FirestoreDb db = FirestoreDb.Create(Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            CollectionReference collectionReference = db.Collection(collection);
            QuerySnapshot snapshot = await collectionReference.GetSnapshotAsync();
            IReadOnlyList<DocumentSnapshot> documents = snapshot.Documents;
            foreach (DocumentSnapshot document in documents)
            {
                await document.Reference.DeleteAsync();
            }
        }

        // Clean-up function to delete all indexes in a collection
        private static async Task DeleteIndexes(string collection)
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefault();
            //Inject the Cloud Platform scope if required.
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
            string uriString = "https://firestore.googleapis.com/v1beta1/projects/"
            + Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID")
            + "/databases/(default)/indexes";
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
                    if (index.collection == collection)
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
        }

        // Clean up function to delete all collections and indexes after testing is complete
        public void Dispose()
        {
            DeleteCollection("users").Wait();
            DeleteCollection("cities/SF/neighborhoods").Wait();
            DeleteCollection("cities").Wait();
            DeleteCollection("data").Wait();
            DeleteIndexes("cities").Wait();
        }
    }

    public class FirestoreTests : IClassFixture<FirestoreFixture>
    {
        readonly CommandLineRunner _quickstart = new CommandLineRunner()
        {
            VoidMain = Quickstart.Main,
            Command = "dotnet run"
        };

        protected ConsoleOutput RunQuickstart(params string[] args)
        {
            return _quickstart.Run(args);
        }

        readonly CommandLineRunner _addData = new CommandLineRunner()
        {
            VoidMain = AddData.Main,
            Command = "dotnet run"
        };

        protected ConsoleOutput RunAddData(params string[] args)
        {
            return _addData.Run(args);
        }

        readonly CommandLineRunner _deleteData = new CommandLineRunner()
        {
            VoidMain = DeleteData.Main,
            Command = "dotnet run"
        };

        protected ConsoleOutput RunDeleteData(params string[] args)
        {
            return _deleteData.Run(args);
        }

        readonly CommandLineRunner _getData = new CommandLineRunner()
        {
            VoidMain = GetData.Main,
            Command = "dotnet run"
        };

        protected ConsoleOutput RunGetData(params string[] args)
        {
            return _getData.Run(args);
        }

        readonly CommandLineRunner _listenData = new CommandLineRunner()
        {
            VoidMain = ListenData.Main,
            Command = "dotnet run"
        };

        protected ConsoleOutput RunListenData(params string[] args)
        {
            return _listenData.Run(args);
        }

        readonly CommandLineRunner _queryData = new CommandLineRunner()
        {
            VoidMain = QueryData.Main,
            Command = "dotnet run"
        };

        protected ConsoleOutput RunQueryData(params string[] args)
        {
            return _queryData.Run(args);
        }

        readonly CommandLineRunner _orderLimitData = new CommandLineRunner()
        {
            VoidMain = OrderLimitData.Main,
            Command = "dotnet run"
        };

        protected ConsoleOutput RunOrderLimitData(params string[] args)
        {
            return _orderLimitData.Run(args);
        }

        readonly CommandLineRunner _dataModel = new CommandLineRunner()
        {
            VoidMain = DataModel.Main,
            Command = "dotnet run"
        };

        protected ConsoleOutput RunDataModel(params string[] args)
        {
            return _dataModel.Run(args);
        }

        readonly CommandLineRunner _transactionsAndBatchedWrites = new CommandLineRunner()
        {
            VoidMain = TransactionsAndBatchedWrites.Main,
            Command = "dotnet run"
        };

        protected ConsoleOutput RunTransactionsAndBatchedWrites(params string[] args)
        {
            return _transactionsAndBatchedWrites.Run(args);
        }

        readonly CommandLineRunner _paginateData = new CommandLineRunner()
        {
            VoidMain = PaginateData.Main,
            Command = "dotnet run"
        };

        protected ConsoleOutput RunPaginateData(params string[] args)
        {
            return _paginateData.Run(args);
        }

        readonly CommandLineRunner _manageIndexes = new CommandLineRunner()
        {
            VoidMain = ManageIndexes.Main,
            Command = "dotnet run"
        };

        protected ConsoleOutput RunManageIndexes(params string[] args)
        {
            return _manageIndexes.Run(args);
        }

        readonly CommandLineRunner _distrubutedCounter = new CommandLineRunner()
        {
            VoidMain = DistributedCounter.Main,
            Command = "dotnet run"
        };

        protected ConsoleOutput RunDistributedCounter(params string[] args)
        {
            return _distrubutedCounter.Run(args);
        }

        // QUICKSTART TESTS
        [Fact]
        public void InitializeProjectIdTest()
        {
            var output = RunQuickstart("initialize-project-id", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Created Cloud Firestore client with project ID:", output.Stdout);
        }

        [Fact]
        public void AddData1Test()
        {
            var output = RunQuickstart("add-data-1", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Added data to the alovelace document in the users collection.", output.Stdout);
        }

        [Fact]
        public void AddData2Test()
        {
            var output = RunQuickstart("add-data-2", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Added data to the aturing document in the users collection.", output.Stdout);
        }

        [Fact]
        public void RetrieveAllDocumentsTest()
        {
            RunQuickstart("add-data-1", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            RunQuickstart("add-data-2", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunQuickstart("retrieve-all-documents", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("User: alovelace", output.Stdout);
            Assert.Contains("First: Ada", output.Stdout);
            Assert.Contains("Last: Lovelace", output.Stdout);
            Assert.Contains("Born: 1815", output.Stdout);
            Assert.Contains("User: aturing", output.Stdout);
            Assert.Contains("First: Alan", output.Stdout);
            Assert.Contains("Middle: Mathison", output.Stdout);
            Assert.Contains("Last: Turing", output.Stdout);
            Assert.Contains("Born: 1912", output.Stdout);
        }

        // ADD DATA TESTS
        [Fact]
        public void AddDocAsMapTest()
        {
            var output = RunAddData("add-doc-as-map", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Added data to the LA document in the cities collection.", output.Stdout);
        }

        [Fact]
        public void UpdateCreateIfMissingTest()
        {
            var output = RunAddData("update-create-if-missing", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Merged data into the LA document in the cities collection.", output.Stdout);
        }

        [Fact]
        public void AddDocDataTypesTest()
        {
            var output = RunAddData("add-doc-data-types", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Set multiple data-type data for the one document in the data collection.", output.Stdout);
        }

        [Fact]
        public void AddSimpleDocAsEntityTest()
        {
            var output = RunAddData("add-simple-doc-as-entity", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Added custom City object to the cities collection.", output.Stdout);
        }

        [Fact]
        public void SetRequiresIdTest()
        {
            var output = RunAddData("set-requires-id", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Added document with ID: new-city-id.", output.Stdout);
        }

        [Fact]
        public void AddDocDataWithAutoIdTest()
        {
            var output = RunAddData("add-doc-data-with-auto-id", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Added document with ID:", output.Stdout);
        }

        [Fact]
        public void AddDocDataAfterAutoIdTest()
        {
            var output = RunAddData("add-doc-data-after-auto-id", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Added document with ID:", output.Stdout);
            Assert.Contains("Added data to the", output.Stdout);
            Assert.Contains("document in the cities collection.", output.Stdout);
        }

        [Fact]
        public void UpdateDocTest()
        {
            RunAddData("set-requires-id", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunAddData("update-doc", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Updated the Capital field of the new-city-id document in the cities collection.", output.Stdout);
        }

        [Fact]
        public void UpdateNestedFieldsTest()
        {
            var output = RunAddData("update-nested-fields", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Updated the age and favorite color fields of the Frank document in the users collection.", output.Stdout);
        }

        [Fact]
        public void UpdateServerTimestampTest()
        {
            RunAddData("set-requires-id", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunAddData("update-server-timestamp", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Updated the Timestamp field of the new-city-id document in the cities collection.", output.Stdout);
        }

        [Fact]
        public void UpdateDocumentArrayTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunAddData("update-document-array", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Updated the Regions array of the DC document in the cities collection.", output.Stdout);
        }

        [Fact]
        public void UpdateDocumentIncrementTest()
        {
            RunQueryData("update-document-increment", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunAddData("update-document-increment", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Updated the population of the DC document in the cities collection.", output.Stdout);
        }


        // DELETE DATA TESTS
        [Fact]
        public void DeleteDocTest()
        {
            RunGetData("retrieve-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunDeleteData("delete-doc", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Deleted the DC document in the cities collection.", output.Stdout);
        }

        [Fact]
        public void DeleteFieldTest()
        {
            RunGetData("retrieve-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunDeleteData("delete-field", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Deleted the Capital field from the BJ document in the cities collection.", output.Stdout);
        }

        [Fact]
        public void DeleteCollectionTest()
        {
            RunGetData("retrieve-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunDeleteData("delete-collection", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Deleting document BJ", output.Stdout);
            Assert.Contains("Deleting document LA", output.Stdout);
            Assert.Contains("Deleting document TOK", output.Stdout);
            Assert.Contains("Deleting document SF", output.Stdout);
            Assert.Contains("Finished deleting all documents from the collection.", output.Stdout);
        }

        // GET DATA TESTS
        [Fact]
        public void RetrieveCreateExamplesTest()
        {
            var output = RunGetData("retrieve-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Added example cities data to the cities collection.", output.Stdout);
        }

        [Fact]
        public void GetDocAsMapTest()
        {
            RunGetData("retrieve-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunGetData("get-doc-as-map", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document data for SF document:", output.Stdout);
            Assert.Contains("Name: San Francisco", output.Stdout);
            Assert.Contains("State: CA", output.Stdout);
            Assert.Contains("Country: USA", output.Stdout);
            Assert.Contains("Capital: False", output.Stdout);
            Assert.Contains("Population: 860000", output.Stdout);
        }

        [Fact]
        public void GetDocAsEntityTest()
        {
            RunGetData("retrieve-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunGetData("get-doc-as-entity", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document data for BJ document:", output.Stdout);
            Assert.Contains("State:", output.Stdout);
            Assert.Contains("Country: China", output.Stdout);
            Assert.Contains("Capital: True", output.Stdout);
            Assert.Contains("Population: 21500000", output.Stdout);
        }

        [Fact]
        public void GetMultipleDocsTest()
        {
            RunGetData("retrieve-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunGetData("get-multiple-docs", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document data for DC document:", output.Stdout);
            Assert.Contains("Document data for TOK document:", output.Stdout);
            Assert.Contains("Document data for BJ document:", output.Stdout);
            Assert.DoesNotContain("Document data for SF document:", output.Stdout);
            Assert.DoesNotContain("Document data for LA document:", output.Stdout);
            Assert.Contains("Name: Tokyo", output.Stdout);
            Assert.Contains("State:", output.Stdout);
            Assert.Contains("Country: Japan", output.Stdout);
            Assert.Contains("Capital: True", output.Stdout);
            Assert.Contains("Population: 9000000", output.Stdout);
        }

        [Fact]
        public void GetAllDocsTest()
        {
            RunGetData("retrieve-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunGetData("get-all-docs", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document data for BJ document:", output.Stdout);
            Assert.Contains("Document data for DC document:", output.Stdout);
            Assert.Contains("Document data for LA document:", output.Stdout);
            Assert.Contains("Document data for SF document:", output.Stdout);
            Assert.Contains("Document data for TOK document:", output.Stdout);
            Assert.Contains("Name: Los Angeles", output.Stdout);
            Assert.Contains("State: CA", output.Stdout);
            Assert.Contains("Country: USA", output.Stdout);
            Assert.Contains("Capital: False", output.Stdout);
            Assert.Contains("Population: 3900000", output.Stdout);
        }

        [Fact]
        public void GetCollectionsTest()
        {
            RunGetData("retrieve-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var addSubcollectionOutput = RunGetData("add-subcollection", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Added data to the Marina document in the neighborhoods subcollection in the SF document in the cities collection.", addSubcollectionOutput.Stdout);
            var getCollectionsOutput = RunGetData("get-collections", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Found subcollection with ID: neighborhoods", getCollectionsOutput.Stdout);
        }

        // LISTEN DATA TESTS
        [Fact]
        public void ListenDocumentTest()
        {
            RunDeleteData("delete-collection", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var listenDocumentOutput = RunListenData("listen-document", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Callback received document snapshot.", listenDocumentOutput.Stdout);
            Assert.Contains("Document exists? True", listenDocumentOutput.Stdout);
            Assert.Contains("Document data for SF document:", listenDocumentOutput.Stdout);
            Assert.Contains("Name: San Francisco", listenDocumentOutput.Stdout);
            Assert.Contains("State: CA", listenDocumentOutput.Stdout);
            Assert.Contains("Country: USA", listenDocumentOutput.Stdout);
            Assert.Contains("Capital: False", listenDocumentOutput.Stdout);
            Assert.Contains("Population: 860000", listenDocumentOutput.Stdout);
            Assert.Contains("Stopping the listener", listenDocumentOutput.Stdout);
        }

        [Fact]
        public void ListenMultipleTest()
        {
            RunDeleteData("delete-collection", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var listenMultipleOutput = RunListenData("listen-multiple", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Creating document", listenMultipleOutput.Stdout);
            Assert.Contains("Callback received query snapshot.", listenMultipleOutput.Stdout);
            Assert.Contains("Current cities in California:", listenMultipleOutput.Stdout);
            Assert.Contains("LA", listenMultipleOutput.Stdout);
            Assert.Contains("Stopping the listener", listenMultipleOutput.Stdout);
        }

        [Fact]
        public void ListenForChangesTest()
        {
            RunDeleteData("delete-collection", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var listenForChangesOutput = RunListenData("listen-for-changes", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Creating document", listenForChangesOutput.Stdout);
            Assert.Contains("New city: MTV", listenForChangesOutput.Stdout);
            Assert.Contains("Modifying document", listenForChangesOutput.Stdout);
            Assert.Contains("Modified city: MTV", listenForChangesOutput.Stdout);
            Assert.Contains("Deleting document", listenForChangesOutput.Stdout);
            Assert.Contains("Removed city: MTV", listenForChangesOutput.Stdout);
            Assert.Contains("Stopping the listener", listenForChangesOutput.Stdout);
        }

        // QUERY DATA TESTS
        [Fact]
        public void QueryCreateExamplesTest()
        {
            var output = RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Added example cities data to the cities collection.", output.Stdout);
        }

        [Fact]
        public void CreateQueryStateTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunQueryData("create-query-state", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document LA returned by query State=CA", output.Stdout);
            Assert.Contains("Document SF returned by query State=CA", output.Stdout);
            Assert.DoesNotContain("Document DC returned by query State=CA", output.Stdout);
            Assert.DoesNotContain("Document TOK returned by query State=CA", output.Stdout);
            Assert.DoesNotContain("Document BJ returned by query State=CA", output.Stdout);
        }

        [Fact]
        public void CreateQueryCapitalTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunQueryData("create-query-capital", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document DC returned by query Capital=true", output.Stdout);
            Assert.Contains("Document TOK returned by query Capital=true", output.Stdout);
            Assert.Contains("Document BJ returned by query Capital=true", output.Stdout);
            Assert.DoesNotContain("Document SF returned by query Capital=true", output.Stdout);
            Assert.DoesNotContain("Document LA returned by query Capital=true", output.Stdout);
        }

        [Fact]
        public void SimpleQueriesTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunQueryData("simple-queries", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document LA returned by query State=CA", output.Stdout);
            Assert.Contains("Document SF returned by query State=CA", output.Stdout);
            Assert.DoesNotContain("Document DC returned by query State=CA", output.Stdout);
            Assert.DoesNotContain("Document TOK returned by query State=CA", output.Stdout);
            Assert.DoesNotContain("Document BJ returned by query State=CA", output.Stdout);
            Assert.Contains("Document LA returned by query Population>1000000", output.Stdout);
            Assert.Contains("Document TOK returned by query Population>1000000", output.Stdout);
            Assert.Contains("Document BJ returned by query Population>1000000", output.Stdout);
            Assert.DoesNotContain("Document SF returned by query Population>1000000", output.Stdout);
            Assert.DoesNotContain("Document DC returned by query Population>1000000", output.Stdout);
            Assert.Contains("Document SF returned by query Name>=San Francisco", output.Stdout);
            Assert.Contains("Document TOK returned by query Name>=San Francisco", output.Stdout);
            Assert.Contains("Document DC returned by query Name>=San Francisco", output.Stdout);
            Assert.DoesNotContain("Document LA returned by query Name>=San Francisco", output.Stdout);
            Assert.DoesNotContain("Document BJ returned by query Name>=San Francisco", output.Stdout);
        }

        [Fact]
        public void ArrayContainsQueryTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunQueryData("array-contains-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document LA returned by query 'Regions array_contains west_coast'", output.Stdout);
            Assert.Contains("Document SF returned by query 'Regions array_contains west_coast'", output.Stdout);
            Assert.DoesNotContain("Document DC returned by query 'Regions array_contains west_coast'", output.Stdout);
            Assert.DoesNotContain("Document TOK returned by query 'Regions array_contains west_coast'", output.Stdout);
            Assert.DoesNotContain("Document BJ returned by query 'Regions array_contains west_coast'", output.Stdout);
        }

        [Fact]
        public void ArrayContainsQueryAnyTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunQueryData("array-contains-any-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document SF returned by query 'Regions array_contains_any {west_coast, east_coast}'", output.Stdout);
            Assert.Contains("Document LA returned by query 'Regions array_contains_any {west_coast, east_coast}'", output.Stdout);
            Assert.Contains("Document DC returned by query 'Regions array_contains_any {west_coast, east_coast}'", output.Stdout);
            Assert.DoesNotContain("Document TOK returned by query 'Regions array_contains_any {west_coast, east_coast}'", output.Stdout);
            Assert.DoesNotContain("Document BJ returned by query 'Regions array_contains west_coast'", output.Stdout);
        }

        [Fact]
        public void InQueryWithoutArrayTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            // Expected: "SF", "LA", "DC", "TOK"
            var output = RunQueryData("in-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document SF returned by query 'Country in {USA, Japan}'", output.Stdout);
            Assert.Contains("Document LA returned by query 'Country in {USA, Japan}'", output.Stdout);
            Assert.Contains("Document DC returned by query 'Country in {USA, Japan}'", output.Stdout);
            Assert.Contains("Document TOK returned by query 'Country in {USA, Japan}'", output.Stdout);
            Assert.DoesNotContain("Document BJ returned by query 'Country in {USA, Japan}'", output.Stdout);
        }

        [Fact]
        public void InQueryWithArrayTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            // Expected: "DC"
            var output = RunQueryData("in-query-array", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document DC returned by query 'Regions in {west_coast}, {east_coast}'", output.Stdout);
            Assert.DoesNotContain("Document LA returned by query 'Regions in {west_coast}, {east_coast}'", output.Stdout);
            Assert.DoesNotContain("Document SF returned by query 'Regions in {west_coast}, {east_coast}'", output.Stdout);
            Assert.DoesNotContain("Document TOK returned by query 'Regions in {west_coast}, {east_coast}'", output.Stdout);
            Assert.DoesNotContain("Document BJ returned by query 'Regions in {west_coast}, {east_coast}'", output.Stdout);
        }

        [Fact]
        public void ChainedQueryTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunQueryData("chained-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document SF returned by query State=CA and Name=San Francisco", output.Stdout);
            Assert.DoesNotContain("Document LA returned by query State=CA and Name=San Francisco", output.Stdout);
            Assert.DoesNotContain("Document DC returned by query State=CA and Name=San Francisco", output.Stdout);
            Assert.DoesNotContain("Document TOK returned by query State=CA and Name=San Francisco", output.Stdout);
            Assert.DoesNotContain("Document BJ returned by query State=CA and Name=San Francisco", output.Stdout);
        }

        [Fact(Skip = "b/137857855")]
        public void CompositeIndexChainedQueryTest()
        {
            var manageIndexesOutput = RunManageIndexes("create-indexes", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            if (!manageIndexesOutput.Stdout.Contains("completed"))
            {
                var numIndexesCreatedOutput = RunManageIndexes("count-indexes", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
                int numIndexesCreated = numIndexesCreatedOutput.Stdout.Split('\n').Length;
                while (numIndexesCreated < 3)
                {
                    numIndexesCreatedOutput = RunManageIndexes("count-indexes", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
                    numIndexesCreated = numIndexesCreatedOutput.Stdout.Split('\n').Length;
                }
            }
            Assert.Contains("Index creation completed!", manageIndexesOutput.Stdout);
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunQueryData("composite-index-chained-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document SF returned by query State=CA and Population<1000000", output.Stdout);
            Assert.DoesNotContain("Document LA returned by query State=CA and Population<1000000", output.Stdout);
            Assert.DoesNotContain("Document DC returned by query State=CA and Population<1000000", output.Stdout);
            Assert.DoesNotContain("Document TOK returned by query State=CA and Population<1000000", output.Stdout);
            Assert.DoesNotContain("Document BJ returned by query State=CA and Population<1000000", output.Stdout);
        }

        [Fact]
        public void RangeQueryTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunQueryData("range-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document LA returned by query CA<=State<=IN", output.Stdout);
            Assert.Contains("Document SF returned by query CA<=State<=IN", output.Stdout);
            Assert.DoesNotContain("Document DC returned by query CA<=State<=IN", output.Stdout);
            Assert.DoesNotContain("Document TOK returned by query CA<=State<=IN", output.Stdout);
            Assert.DoesNotContain("Document BJ returned by query CA<=State<=IN", output.Stdout);
        }

        [Fact]
        public void InvalidRangeQueryTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunQueryData("invalid-range-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
        }

        [Fact]
        public void SubcollectionQueryTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunQueryData("subcollection-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));

            Assert.Contains(": Golden Gate Bridge", output.Stdout);
        }

        [Fact]
        public void MultipleInequalitiesQueryTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunQueryData("multiple-inequalities", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));

            Assert.Contains("Los Angeles", output.Stdout);
            Assert.Contains("Beijing", output.Stdout);
        }

        // ORDER LIMIT DATA TESTS
        [Fact]
        public void OrderByNameLimitQueryTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunOrderLimitData("order-by-name-limit-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document BJ returned by order by name with limit query", output.Stdout);
            Assert.Contains("Document LA returned by order by name with limit query", output.Stdout);
            Assert.Contains("Document SF returned by order by name with limit query", output.Stdout);
            Assert.DoesNotContain("Document DC returned by order by name with limit query", output.Stdout);
            Assert.DoesNotContain("Document TOK returned by order by name with limit query", output.Stdout);
        }

        [Fact]
        public void OrderByNameDescLimitQueryTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunOrderLimitData("order-by-name-desc-limit-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document DC returned by order by name descending with limit query", output.Stdout);
            Assert.Contains("Document TOK returned by order by name descending with limit query", output.Stdout);
            Assert.Contains("Document SF returned by order by name descending with limit query", output.Stdout);
            Assert.DoesNotContain("Document LA returned by order by name descending with limit query", output.Stdout);
            Assert.DoesNotContain("Document BJ returned by order by name descending with limit query", output.Stdout);
        }

        [Fact(Skip = "b/137857855")]
        public void OrderByStateAndPopulationQueryTest()
        {
            var manageIndexesOutput = RunManageIndexes("create-indexes", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            if (!manageIndexesOutput.Stdout.Contains("completed"))
            {
                var numIndexesCreatedOutput = RunManageIndexes("count-indexes", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
                int numIndexesCreated = numIndexesCreatedOutput.Stdout.Split('\n').Length;
                while (numIndexesCreated < 3)
                {
                    numIndexesCreatedOutput = RunManageIndexes("count-indexes", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
                    numIndexesCreated = numIndexesCreatedOutput.Stdout.Split('\n').Length;
                }
            }
            Assert.Contains("Index creation completed!", manageIndexesOutput.Stdout);
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunOrderLimitData("order-by-state-and-population-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document LA returned by order by state and descending population query", output.Stdout);
            Assert.Contains("Document SF returned by order by state and descending population query", output.Stdout);
            Assert.Contains("Document BJ returned by order by state and descending population query", output.Stdout);
            Assert.Contains("Document DC returned by order by state and descending population query", output.Stdout);
            Assert.Contains("Document TOK returned by order by state and descending population query", output.Stdout);
        }

        [Fact]
        public void WhereOrderByLimitQueryTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunOrderLimitData("where-order-by-limit-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document LA returned by where order by limit query", output.Stdout);
            Assert.Contains("Document TOK returned by where order by limit query", output.Stdout);
            Assert.DoesNotContain("Document SF returned by where order by limit query", output.Stdout);
            Assert.DoesNotContain("Document DC returned by where order by limit query", output.Stdout);
            Assert.DoesNotContain("Document BJ returned by where order by limit query", output.Stdout);
        }

        [Fact]
        public void RangeOrderByQueryTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunOrderLimitData("range-order-by-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document LA returned by range with order by query", output.Stdout);
            Assert.Contains("Document TOK returned by range with order by query", output.Stdout);
            Assert.Contains("Document BJ returned by range with order by query", output.Stdout);
            Assert.DoesNotContain("Document SF returned by range with order by query", output.Stdout);
            Assert.DoesNotContain("Document DC returned by range with order by query", output.Stdout);
        }

        [Fact]
        public void InvalidRangeOrderByQueryTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunOrderLimitData("invalid-range-order-by-query", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
        }

        // DATA MODEL TESTS
        [Fact]
        public void DocumentRefTest()
        {
            RunDataModel("document-ref", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
        }

        [Fact]
        public void CollectionRefTest()
        {
            RunDataModel("collection-ref", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
        }

        [Fact]
        public void DocumentPathRefTest()
        {
            RunDataModel("document-path-ref", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
        }

        [Fact]
        public void SubcollectionRefTest()
        {
            RunDataModel("subcollection-ref", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
        }

        // TRANSACTIONS AND BATCHED WRITES TESTS
        [Fact]
        public void RunSimpleTransactionTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunTransactionsAndBatchedWrites("run-simple-transaction", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Ran a simple transaction to update the population field in the SF document in the cities collection.", output.Stdout);
        }

        [Fact]
        public void ReturnInfoTransactionTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunTransactionsAndBatchedWrites("return-info-transaction", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Population updated successfully.", output.Stdout);
        }

        [Fact]
        public void BatchWriteTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunTransactionsAndBatchedWrites("batch-write", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Batch write successfully completed.", output.Stdout);
        }

        // PAGINATE DATA TESTS
        [Fact]
        public void StartAtFieldQueryCursorTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunPaginateData("start-at-field-query-cursor", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document LA returned by start at population 1000000 field query cursor", output.Stdout);
            Assert.Contains("Document TOK returned by start at population 1000000 field query cursor", output.Stdout);
            Assert.Contains("Document BJ returned by start at population 1000000 field query cursor", output.Stdout);
            Assert.DoesNotContain("Document SF returned by start at population 1000000 field query cursor", output.Stdout);
            Assert.DoesNotContain("Document DC returned by start at population 1000000 field query cursor", output.Stdout);
        }

        [Fact]
        public void EndAtFieldQueryCursorTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunPaginateData("end-at-field-query-cursor", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document DC returned by end at population 1000000 field query cursor", output.Stdout);
            Assert.Contains("Document SF returned by end at population 1000000 field query cursor", output.Stdout);
            Assert.DoesNotContain("Document LA returned by end at population 1000000 field query cursor", output.Stdout);
            Assert.DoesNotContain("Document TOK returned by end at population 1000000 field query cursor", output.Stdout);
            Assert.DoesNotContain("Document BJ returned by end at population 1000000 field query cursor", output.Stdout);
        }

        [Fact]
        public void DocumentSnapshotCursorTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunPaginateData("document-snapshot-cursor", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.DoesNotContain("Document DC returned by query for cities with population greater than or equal to SF.", output.Stdout);
            Assert.Contains("Document SF returned by query for cities with population greater than or equal to SF.", output.Stdout);
            Assert.Contains("Document LA returned by query for cities with population greater than or equal to SF.", output.Stdout);
            Assert.Contains("Document TOK returned by query for cities with population greater than or equal to SF.", output.Stdout);
            Assert.Contains("Document BJ returned by query for cities with population greater than or equal to SF.", output.Stdout);
        }

        [Fact]
        public void PaginatedQueryCursorTest()
        {
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            var output = RunPaginateData("paginated-query-cursor", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Document TOK returned by paginated query cursor.", output.Stdout);
            Assert.Contains("Document BJ returned by paginated query cursor.", output.Stdout);
            Assert.DoesNotContain("Document SF returned by paginated query cursor.", output.Stdout);
            Assert.DoesNotContain("Document LA returned by paginated query cursor.", output.Stdout);
            Assert.DoesNotContain("Document DC returned by paginated query cursor.", output.Stdout);
        }

        [Fact(Skip = "b/137857855")]
        public void MultipleCursorConditionsTest()
        {
            var manageIndexesOutput = RunManageIndexes("create-indexes", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            if (!manageIndexesOutput.Stdout.Contains("completed"))
            {
                var numIndexesCreatedOutput = RunManageIndexes("count-indexes", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
                int numIndexesCreated = numIndexesCreatedOutput.Stdout.Split('\n').Length;
                while (numIndexesCreated < 3)
                {
                    numIndexesCreatedOutput = RunManageIndexes("count-indexes", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
                    numIndexesCreated = numIndexesCreatedOutput.Stdout.Split('\n').Length;
                }
            }
            Assert.Contains("Index creation completed!", manageIndexesOutput.Stdout);
            RunQueryData("query-create-examples", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            RunPaginateData("multiple-cursor-conditions", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
        }

        [Fact]
        public void RunDistributedCounterTest()
        {
            var output = RunDistributedCounter("distributed-counter", Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
            Assert.Contains("Distributed counter created.", output.Stdout);
            Assert.Contains("Distributed counter incremented.", output.Stdout);
            Assert.Contains("Total count: 1", output.Stdout);
        }
    }
}
