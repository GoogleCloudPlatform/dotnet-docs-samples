// Copyright(c) 2017 Google Inc.
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

using Google.Cloud.Firestore;
using Google.Cloud.Firestore.Admin.V1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Google.Cloud.Firestore.Admin.V1.Index.Types;

namespace GoogleCloudSamples
{
    public class QueryData
    {
        public static string Usage = @"Usage:
C:\> dotnet run command YOUR_PROJECT_ID

Where command is one of
    query-create-examples
    create-query-state
    create-query-capital
    simple-queries
    array-contains-query
    array-contains-any-query
    in-query
    in-query-array
    collection-group-query
    subcollection-query
    chained-query
    composite-index-chained-query
    range-query
    invalid-range-query
    multiple-inequalities
";
        private static async Task QueryCreateExamples(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // Note: the extra braces here are just to allow multiple citiesRef local variables.
            {
                // [START firestore_query_filter_dataset]
                CollectionReference citiesRef = db.Collection("cities");
                await citiesRef.Document("SF").SetAsync(new Dictionary<string, object>
                {
                    { "Name", "San Francisco" },
                    { "State", "CA" },
                    { "Country", "USA" },
                    { "Capital", false },
                    { "Population", 860000 },
                    { "Density", 18000 },
                    { "Regions", new[] {"west_coast", "norcal"} }
                });
                await citiesRef.Document("LA").SetAsync(new Dictionary<string, object>
                {
                    { "Name", "Los Angeles" },
                    { "State", "CA" },
                    { "Country", "USA" },
                    { "Capital", false },
                    { "Population", 3900000 },
                    { "Density", 8300 },
                    { "Regions", new[] {"west_coast", "socal"} }
                });
                await citiesRef.Document("DC").SetAsync(new Dictionary<string, object>
                {
                    { "Name", "Washington D.C." },
                    { "State", null },
                    { "Country", "USA" },
                    { "Capital", true },
                    { "Population", 680000 },
                    { "Density", 11300 },
                    { "Regions", new[] {"east_coast"} }
                });
                await citiesRef.Document("TOK").SetAsync(new Dictionary<string, object>
                {
                    { "Name", "Tokyo" },
                    { "State", null },
                    { "Country", "Japan" },
                    { "Capital", true },
                    { "Population", 9000000 },
                    { "Density", 16000 },
                    { "Regions", new[] {"kanto", "honshu"} }
                });
                await citiesRef.Document("BJ").SetAsync(new Dictionary<string, object>
                {
                    { "Name", "Beijing" },
                    { "State", null },
                    { "Country", "China" },
                    { "Capital", true },
                    { "Population", 21500000 },
                    { "Density", 3500 },
                    { "Regions", new[] {"jingjinji", "hebei"} }
                });
                Console.WriteLine("Added example cities data to the cities collection.");
                // [END firestore_query_filter_dataset]
            }

            {
                // [START firestore_query_collection_group_dataset]
                CollectionReference citiesRef = db.Collection("cities");
                await citiesRef.Document("SF").Collection("landmarks").Document()
                    .SetAsync(new { Name = "Golden Gate Bridge", Type = "bridge" });
                await citiesRef.Document("SF").Collection("landmarks").Document()
                    .SetAsync(new { Name = "Legion of Honor", Type = "museum" });
                await citiesRef.Document("LA").Collection("landmarks").Document()
                    .SetAsync(new { Name = "Griffith Park", Type = "park" });
                await citiesRef.Document("DC").Collection("landmarks").Document()
                    .SetAsync(new { Name = "Lincoln Memorial", Type = "memorial" });
                await citiesRef.Document("DC").Collection("landmarks").Document()
                    .SetAsync(new { Name = "National Air And Space Museum", Type = "museum" });
                await citiesRef.Document("TOK").Collection("landmarks").Document()
                    .SetAsync(new { Name = "Ueno Park", Type = "park" });
                await citiesRef.Document("TOK").Collection("landmarks").Document()
                    .SetAsync(new { Name = "National Museum of Nature and Science", Type = "museum" });
                await citiesRef.Document("BJ").Collection("landmarks").Document()
                    .SetAsync(new { Name = "Jingshan Park", Type = "park" });
                await citiesRef.Document("BJ").Collection("landmarks").Document()
                    .SetAsync(new { Name = "Beijing Ancient Observatory", Type = "museum" });
                // [END firestore_query_collection_group_dataset]
            }
        }

        private static async Task CreateQueryState(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_query_filter_eq_string]
            CollectionReference citiesRef = db.Collection("cities");
            Query query = citiesRef.WhereEqualTo("State", "CA");
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query State=CA", documentSnapshot.Id);
            }
            // [END firestore_query_filter_eq_string]
        }

        private static async Task CreateQueryCapital(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_query_filter_eq_boolean]
            CollectionReference citiesRef = db.Collection("cities");
            Query query = citiesRef.WhereEqualTo("Capital", true);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query Capital=true", documentSnapshot.Id);
            }
            // [END firestore_query_filter_eq_boolean]
        }

        private static async Task SimpleQueries(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_query_filter_single_examples]
            CollectionReference citiesRef = db.Collection("cities");
            Query stateQuery = citiesRef.WhereEqualTo("State", "CA");
            Query populationQuery = citiesRef.WhereGreaterThan("Population", 1000000);
            Query nameQuery = citiesRef.WhereGreaterThanOrEqualTo("Name", "San Francisco");
            // [END firestore_query_filter_single_examples]
            QuerySnapshot stateQuerySnapshot = await stateQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in stateQuerySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query State=CA", documentSnapshot.Id);
            }
            QuerySnapshot populationQuerySnapshot = await populationQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in populationQuerySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query Population>1000000", documentSnapshot.Id);
            }
            QuerySnapshot nameQuerySnapshot = await nameQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in nameQuerySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query Name>=San Francisco", documentSnapshot.Id);
            }
        }

        private static async Task ArrayContainsQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_query_filter_array_contains]
            CollectionReference citiesRef = db.Collection("cities");
            Query query = citiesRef.WhereArrayContains("Regions", "west_coast");
            // [END firestore_query_filter_array_contains]
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query 'Regions array_contains west_coast'", documentSnapshot.Id);
            }
        }

        private static async Task ArrayContainsAnyQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_query_filter_array_contains_any]
            CollectionReference citiesRef = db.Collection("cities");
            Query query = citiesRef.WhereArrayContainsAny("Regions", new[] { "west_coast", "east_coast" });
            // [END firestore_query_filter_array_contains_any]
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query 'Regions array_contains_any {{west_coast, east_coast}}'", documentSnapshot.Id);
            }
        }

        private static async Task InQueryWithoutArray(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_query_filter_in]
            CollectionReference citiesRef = db.Collection("cities");
            Query query = citiesRef.WhereIn("Country", new[] { "USA", "Japan" });
            // [END firestore_query_filter_in]
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query 'Country in {{USA, Japan}}'", documentSnapshot.Id);
            }
        }

        private static async Task InQueryWithArray(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_query_filter_in_with_array]
            CollectionReference citiesRef = db.Collection("cities");
            Query query = citiesRef.WhereIn("Regions",
                new[] { new[] { "west_coast" }, new[] { "east_coast" } });
            // [END firestore_query_filter_in_with_array]
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query 'Regions in {{west_coast}}, {{east_coast}}'", documentSnapshot.Id);
            }
        }

        private static async Task CollectionGroupQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_query_collection_group_filter_eq]
            Query museums = db.CollectionGroup("landmarks").WhereEqualTo("Type", "museum");
            QuerySnapshot querySnapshot = await museums.GetSnapshotAsync();
            foreach (DocumentSnapshot document in querySnapshot.Documents)
            {
                Console.WriteLine($"{document.Reference.Path}: {document.GetValue<string>("Name")}");
            }
            // [END firestore_query_collection_group_filter_eq]
        }

        private static async Task SubcollectionQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_query_subcollection]
            CollectionReference landmarks = db.Collection("cities").Document("SF").Collection("landmarks");
            QuerySnapshot querySnapshot = await landmarks.GetSnapshotAsync();
            foreach (DocumentSnapshot document in querySnapshot.Documents)
            {
                Console.WriteLine($"{document.Reference.Path}: {document.GetValue<string>("Name")}");
            }
            // [END firestore_query_subcollection]
        }

        private static async Task ChainedQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_query_filter_compound_multi_eq]
            CollectionReference citiesRef = db.Collection("cities");
            Query chainedQuery = citiesRef
                .WhereEqualTo("State", "CA")
                .WhereEqualTo("Name", "San Francisco");
            // [END firestore_query_filter_compound_multi_eq]
            QuerySnapshot querySnapshot = await chainedQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query State=CA and Name=San Francisco", documentSnapshot.Id);
            }
        }

        private static async Task CompositeIndexChainedQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_query_filter_compound_multi_eq_lt]
            CollectionReference citiesRef = db.Collection("cities");
            Query chainedQuery = citiesRef
                .WhereEqualTo("State", "CA")
                .WhereLessThan("Population", 1000000);
            // [END firestore_query_filter_compound_multi_eq_lt]
            QuerySnapshot querySnapshot = await chainedQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query State=CA and Population<1000000", documentSnapshot.Id);
            }
        }

        private static async Task RangeQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_query_filter_range_valid]
            CollectionReference citiesRef = db.Collection("cities");
            Query rangeQuery = citiesRef
                .WhereGreaterThanOrEqualTo("State", "CA")
                .WhereLessThanOrEqualTo("State", "IN");
            // [END firestore_query_filter_range_valid]
            QuerySnapshot querySnapshot = await rangeQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query CA<=State<=IN", documentSnapshot.Id);
            }
        }

        private static void InvalidRangeQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_query_filter_range_invalid]
            CollectionReference citiesRef = db.Collection("cities");
            Query invalidRangeQuery = citiesRef
                .WhereGreaterThanOrEqualTo("State", "CA")
                .WhereGreaterThan("Population", 1000000);
            // [END firestore_query_filter_range_invalid]
        }

        private static async Task MultipleInequalitiesQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            FirestoreAdminClient adminClient = FirestoreAdminClient.Create();
            var index = new Google.Cloud.Firestore.Admin.V1.Index
            {
                Fields =
                {
                    new IndexField { FieldPath = "Density", Order = IndexField.Types.Order.Ascending },
                    new IndexField { FieldPath = "Population", Order = IndexField.Types.Order.Ascending }
                },
                QueryScope = QueryScope.Collection
            };

            // We speculatively try to create the index, and just ignore an error of it already existing.
            try
            {
                var lro = await adminClient.CreateIndexAsync(new CollectionGroupName(db.ProjectId, db.DatabaseId, "cities"), index);
                await lro.PollUntilCompletedAsync();
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists)
            {
                // Assume the index is okay.
            }

            // [START firestore_query_filter_compound_multi_ineq]
            CollectionReference citiesRef = db.Collection("cities");
            Query query = citiesRef
                .WhereGreaterThan("Population", 1000000)
                .WhereLessThan("Density", 10000);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot)
            {
                var name = documentSnapshot.GetValue<string>("Name");
                var population = documentSnapshot.GetValue<int>("Population");
                var density = documentSnapshot.GetValue<int>("Density");
                Console.WriteLine($"City '{name}' returned by query. Population={population}; Density={density}");
            }
            // [END firestore_query_filter_compound_multi_ineq]
        }

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
                case "query-create-examples":
                    QueryCreateExamples(project).Wait();
                    break;

                case "create-query-state":
                    CreateQueryState(project).Wait();
                    break;

                case "create-query-capital":
                    CreateQueryCapital(project).Wait();
                    break;

                case "simple-queries":
                    SimpleQueries(project).Wait();
                    break;

                case "array-contains-query":
                    ArrayContainsQuery(project).Wait();
                    break;

                case "array-contains-any-query":
                    ArrayContainsAnyQuery(project).Wait();
                    break;

                case "in-query":
                    InQueryWithoutArray(project).Wait();
                    break;

                case "in-query-array":
                    InQueryWithArray(project).Wait();
                    break;

                case "collection-group-query":
                    CollectionGroupQuery(project).Wait();
                    break;

                case "subcollection-query":
                    SubcollectionQuery(project).Wait();
                    break;

                case "chained-query":
                    ChainedQuery(project).Wait();
                    break;

                case "composite-index-chained-query":
                    CompositeIndexChainedQuery(project).Wait();
                    break;

                case "range-query":
                    RangeQuery(project).Wait();
                    break;

                case "invalid-range-query":
                    InvalidRangeQuery(project);
                    break;

                case "multiple-inequalities":
                    MultipleInequalitiesQuery(project).Wait();
                    break;

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}
