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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    chained-query
    composite-index-chained-query
    range-query
    invalid-range-query
";
        private static async Task QueryCreateExamples(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_query_create_examples]
            CollectionReference citiesRef = db.Collection("cities");
            await citiesRef.Document("SF").SetAsync(new Dictionary<string, object>(){
                { "Name", "San Francisco" },
                { "State", "CA" },
                { "Country", "USA" },
                { "Capital", false },
                { "Population", 860000 },
                { "Regions", new ArrayList{"west_coast", "norcal"} }
            });
            await citiesRef.Document("LA").SetAsync(new Dictionary<string, object>(){
                { "Name", "Los Angeles" },
                { "State", "CA" },
                { "Country", "USA" },
                { "Capital", false },
                { "Population", 3900000 },
                { "Regions", new ArrayList{"west_coast", "socal"} }
            });
            await citiesRef.Document("DC").SetAsync(new Dictionary<string, object>(){
                { "Name", "Washington D.C." },
                { "State", null },
                { "Country", "USA" },
                { "Capital", true },
                { "Population", 680000 },
                { "Regions", new ArrayList{"east_coast"} }
            });
            await citiesRef.Document("TOK").SetAsync(new Dictionary<string, object>(){
                { "Name", "Tokyo" },
                { "State", null },
                { "Country", "Japan" },
                { "Capital", true },
                { "Population", 9000000 },
                { "Regions", new ArrayList{"kanto", "honshu"} }
            });
            await citiesRef.Document("BJ").SetAsync(new Dictionary<string, object>(){
                { "Name", "Beijing" },
                { "State", null },
                { "Country", "China" },
                { "Capital", true },
                { "Population", 21500000 },
                { "Regions", new ArrayList{"jingjinji", "hebei"} }
            });
            Console.WriteLine("Added example cities data to the cities collection.");
            // [END fs_query_create_examples]
        }

        private static async Task CreateQueryState(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_create_query_state]
            CollectionReference citiesRef = db.Collection("cities");
            Query query = citiesRef.WhereEqualTo("State", "CA");
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query State=CA", documentSnapshot.Id);
            }
            // [END fs_create_query_state]
        }

        private static async Task CreateQueryCapital(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_create_query_capital]
            CollectionReference citiesRef = db.Collection("cities");
            Query query = citiesRef.WhereEqualTo("Capital", true);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query Capital=true", documentSnapshot.Id);
            }
            // [END fs_create_query_capital]
        }

        private static async Task SimpleQueries(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_simple_queries]
            Query stateQuery = citiesRef.WhereEqualTo("State", "CA");
            Query populationQuery = citiesRef.WhereGreaterThan("Population", 1000000);
            Query nameQuery = citiesRef.WhereGreaterThanOrEqualTo("Name", "San Francisco");
            // [END fs_simple_queries]
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
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_array_contains_query]
            Query query = citiesRef.WhereArrayContains("Regions", "west_coast");
            // [END fs_array_contains_query]
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query 'Regions array_contains west_coast'", documentSnapshot.Id);
            }
        }

        private static async Task ArrayContainsAnyQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_query_filter_array_contains_any]
            Query query = citiesRef.WhereArrayContainsAny("Regions", new[] { "west_coast", "east_coast" });
            // [END fs_query_filter_array_contains_any]
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query 'Regions array_contains_any {{west_coast, east_coast}}'", documentSnapshot.Id);
            }
        }

        private static async Task InQueryWithoutArray(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_query_filter_in]
            Query query = citiesRef.WhereIn("Country", new[] { "USA", "Japan" });
            // [END fs_query_filter_in]
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query 'Country in {{USA, Japan}}'", documentSnapshot.Id);
            }
        }

        private static async Task InQueryWithArray(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_query_filter_in_with_array]
            Query query = citiesRef.WhereIn("Regions",
                new[] { new[] { "west_coast" }, new[] { "east_coast" } });
            // [END fs_query_filter_in_with_array]
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query 'Regions in {{west_coast}}, {{east_coast}}'", documentSnapshot.Id);
            }
        }

        private static async Task CollectionGroupQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_collection_group_query_data_setup]
            await citiesRef.Document("SF").Collection("landmarks").Document()
                .CreateAsync(new { Name = "Golden Gate Bridge", Type = "bridge" });
            await citiesRef.Document("SF").Collection("landmarks").Document()
                .CreateAsync(new { Name = "Legion of Honor", Type = "museum" });
            await citiesRef.Document("LA").Collection("landmarks").Document()
                .CreateAsync(new { Name = "Griffith Park", Type = "park" });
            await citiesRef.Document("DC").Collection("landmarks").Document()
                .CreateAsync(new { Name = "Lincoln Memorial", Type = "memorial" });
            await citiesRef.Document("DC").Collection("landmarks").Document()
                .CreateAsync(new { Name = "National Air And Space Museum", Type = "museum" });
            await citiesRef.Document("TOK").Collection("landmarks").Document()
                .CreateAsync(new { Name = "Ueno Park", Type = "park" });
            await citiesRef.Document("TOK").Collection("landmarks").Document()
                .CreateAsync(new { Name = "National Museum of Nature and Science", Type = "museum" });
            await citiesRef.Document("BJ").Collection("landmarks").Document()
                .CreateAsync(new { Name = "Jingshan Park", Type = "park" });
            await citiesRef.Document("BJ").Collection("landmarks").Document()
                .CreateAsync(new { Name = "Beijing Ancient Observatory", Type = "museum" });
            // [END fs_collection_group_query_data_setup]

            // [START fs_collection_group_query]
            Query museums = db.CollectionGroup("landmarks").WhereEqualTo("Type", "museum");
            QuerySnapshot querySnapshot = await museums.GetSnapshotAsync();
            foreach (DocumentSnapshot document in querySnapshot.Documents)
            {
                Console.WriteLine($"{document.Reference.Path}: {document.GetValue<string>("Name")}");
            }
            // [END fs_collection_group_query]
        }

        private static async Task ChainedQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_chained_query]
            Query chainedQuery = citiesRef
                .WhereEqualTo("State", "CA")
                .WhereEqualTo("Name", "San Francisco");
            // [END fs_chained_query]
            QuerySnapshot querySnapshot = await chainedQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query State=CA and Name=San Francisco", documentSnapshot.Id);
            }
        }

        private static async Task CompositeIndexChainedQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_composite_index_chained_query]
            Query chainedQuery = citiesRef
                .WhereEqualTo("State", "CA")
                .WhereLessThan("Population", 1000000);
            // [END fs_composite_index_chained_query]
            QuerySnapshot querySnapshot = await chainedQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query State=CA and Population<1000000", documentSnapshot.Id);
            }
        }

        private static async Task RangeQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_range_query]
            Query rangeQuery = citiesRef
                .WhereGreaterThanOrEqualTo("State", "CA")
                .WhereLessThanOrEqualTo("State", "IN");
            // [END fs_range_query]
            QuerySnapshot querySnapshot = await rangeQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query CA<=State<=IN", documentSnapshot.Id);
            }
        }

        private static void InvalidRangeQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_invalid_range_query]
            Query invalidRangeQuery = citiesRef
                .WhereGreaterThanOrEqualTo("State", "CA")
                .WhereGreaterThan("Population", 1000000);
            // [END fs_invalid_range_query]
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

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}
