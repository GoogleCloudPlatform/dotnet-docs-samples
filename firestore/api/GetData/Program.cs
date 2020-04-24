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

using CommandLine;
using Google.Cloud.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace GoogleCloudSamples
{
    public class GetData
    {
        public static string Usage = @"Usage:
C:\> dotnet run command YOUR_PROJECT_ID

Where command is one of
    retrieve-create-examples
    get-doc-as-map
    get-doc-as-entity
    get-multiple-docs
    get-all-docs
    add-subcollection
    get-collections
";
        private static async Task RetrieveCreateExamples(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_retrieve_create_examples]
            CollectionReference citiesRef = db.Collection("cities");
            await citiesRef.Document("SF").SetAsync(new Dictionary<string, object>(){
                { "Name", "San Francisco" },
                { "State", "CA" },
                { "Country", "USA" },
                { "Capital", false },
                { "Population", 860000 }
            });
            await citiesRef.Document("LA").SetAsync(new Dictionary<string, object>(){
                { "Name", "Los Angeles" },
                { "State", "CA" },
                { "Country", "USA" },
                { "Capital", false },
                { "Population", 3900000 }
            });
            await citiesRef.Document("DC").SetAsync(new Dictionary<string, object>(){
                { "Name", "Washington D.C." },
                { "State", null },
                { "Country", "USA" },
                { "Capital", true },
                { "Population", 680000 }
            });
            await citiesRef.Document("TOK").SetAsync(new Dictionary<string, object>(){
                { "Name", "Tokyo" },
                { "State", null },
                { "Country", "Japan" },
                { "Capital", true },
                { "Population", 9000000 }
            });
            await citiesRef.Document("BJ").SetAsync(new Dictionary<string, object>(){
                { "Name", "Beijing" },
                { "State", null },
                { "Country", "China" },
                { "Capital", true },
                { "Population", 21500000 }
            });
            Console.WriteLine("Added example cities data to the cities collection.");
            // [END fs_retrieve_create_examples]
        }

        private static async Task GetDocAsMap(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_get_doc_as_map]
            DocumentReference docRef = db.Collection("cities").Document("SF");
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Console.WriteLine("Document data for {0} document:", snapshot.Id);
                Dictionary<string, object> city = snapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in city)
                {
                    Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                }
            }
            else
            {
                Console.WriteLine("Document {0} does not exist!", snapshot.Id);
            }
            // [END fs_get_doc_as_map]
        }

        // A custom City class used by the GetDocAsEntity function.
        [FirestoreData]
        public class City
        {
            [FirestoreProperty]
            public string Name { get; set; }

            [FirestoreProperty]
            public string State { get; set; }

            [FirestoreProperty]
            public string Country { get; set; }

            [FirestoreProperty]
            public bool Capital { get; set; }

            [FirestoreProperty]
            public long Population { get; set; }
        }

        private static async Task GetDocAsEntity(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_get_doc_as_entity]
            DocumentReference docRef = db.Collection("cities").Document("BJ");
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Console.WriteLine("Document data for {0} document:", snapshot.Id);
                City city = snapshot.ConvertTo<City>();
                Console.WriteLine("Name: {0}", city.Name);
                Console.WriteLine("State: {0}", city.State);
                Console.WriteLine("Country: {0}", city.Country);
                Console.WriteLine("Capital: {0}", city.Capital);
                Console.WriteLine("Population: {0}", city.Population);
            }
            else
            {
                Console.WriteLine("Document {0} does not exist!", snapshot.Id);
            }
            // [END fs_get_doc_as_entity]
        }

        private static async Task GetMultipleDocs(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_get_multiple_docs]
            Query capitalQuery = db.Collection("cities").WhereEqualTo("Capital", true);
            QuerySnapshot capitalQuerySnapshot = await capitalQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                Console.WriteLine("Document data for {0} document:", documentSnapshot.Id);
                Dictionary<string, object> city = documentSnapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in city)
                {
                    Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                }
                Console.WriteLine("");
            }
            // [END fs_get_multiple_docs]
        }

        private static async Task GetAllDocs(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_get_all_docs]
            Query allCitiesQuery = db.Collection("cities");
            QuerySnapshot allCitiesQuerySnapshot = await allCitiesQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                Console.WriteLine("Document data for {0} document:", documentSnapshot.Id);
                Dictionary<string, object> city = documentSnapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in city)
                {
                    Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                }
                Console.WriteLine("");
            }
            // [END fs_get_all_docs]
        }

        private static async Task AddSubcollection(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_add_subcollection]
            DocumentReference cityRef = db.Collection("cities").Document("SF");
            CollectionReference subcollectionRef = cityRef.Collection("neighborhoods");
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "name", "Marina" },
            };
            await subcollectionRef.Document("Marina").SetAsync(data);
            // [END fs_add_subcollection]
            Console.WriteLine("Added data to the Marina document in the neighborhoods subcollection in the SF document in the cities collection.");
        }

        private static async Task GetCollections(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_get_collections]
            DocumentReference cityRef = db.Collection("cities").Document("SF");
            IAsyncEnumerable<CollectionReference> subcollections = cityRef.ListCollectionsAsync();
            IAsyncEnumerator<CollectionReference> subcollectionsEnumerator = subcollections.GetAsyncEnumerator(default);
            while (await subcollectionsEnumerator.MoveNextAsync())
            {
                CollectionReference subcollectionRef = subcollectionsEnumerator.Current;
                Console.WriteLine("Found subcollection with ID: {0}", subcollectionRef.Id);
            }
            // [END fs_get_collections]
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
                case "retrieve-create-examples":
                    RetrieveCreateExamples(project).Wait();
                    break;

                case "get-doc-as-map":
                    GetDocAsMap(project).Wait();
                    break;

                case "get-doc-as-entity":
                    GetDocAsEntity(project).Wait();
                    break;

                case "get-multiple-docs":
                    GetMultipleDocs(project).Wait();
                    break;

                case "get-all-docs":
                    GetAllDocs(project).Wait();
                    break;

                case "add-subcollection":
                    AddSubcollection(project).Wait();
                    break;

                case "get-collections":
                    GetCollections(project).Wait();
                    break;

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}
