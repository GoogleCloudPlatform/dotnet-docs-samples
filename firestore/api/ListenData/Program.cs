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
    public class ListenData
    {
        public static string Usage = @"Usage:
C:\> dotnet run command YOUR_PROJECT_ID

Where command is one of
    listen-document
    listen-multiple
    listen-for-changes
";
        private static async Task ListenDocument(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_listen_document]
            DocumentReference docRef = db.Collection("cities").Document("SF");
            FirestoreChangeListener listener = docRef.Listen(snapshot =>
            {
                Console.WriteLine("Callback received document snapshot.");
                Console.WriteLine("Document exists? {0}", snapshot.Exists);
                if (snapshot.Exists)
                {
                    Console.WriteLine("Document data for {0} document:", snapshot.Id);
                    Dictionary<string, object> city = snapshot.ToDictionary();
                    foreach (KeyValuePair<string, object> pair in city)
                    {
                        Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                    }
                }
            });
            // [END firestore_listen_document]

            // Create a new document at cities/SF to demonstrate realtime listener
            Console.WriteLine("Creating document");
            Dictionary<string, object> cityObject = new Dictionary<string, object>
            {
                { "Name", "San Francisco" },
                { "State", "CA" },
                { "Country", "USA" },
                { "Capital", false },
                { "Population", 860000 }
            };
            await docRef.CreateAsync(cityObject);
            await Task.Delay(3000);

            // Stop the listener when you no longer want to receive updates.
            Console.WriteLine("Stopping the listener");
            // [START firestore_listen_detach]
            await listener.StopAsync();
            // [END firestore_listen_detach]
        }

        private static async Task ListenMultiple(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_listen_query_snapshots]
            CollectionReference citiesRef = db.Collection("cities");
            Query query = db.Collection("cities").WhereEqualTo("State", "CA");

            FirestoreChangeListener listener = query.Listen(snapshot =>
            {
                Console.WriteLine("Callback received query snapshot.");
                Console.WriteLine("Current cities in California:");
                foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
                {
                    Console.WriteLine(documentSnapshot.Id);
                }
            });
            // [END firestore_listen_query_snapshots]

            // Create a new document at cities/LA to demonstrate realtime listener
            Console.WriteLine("Creating document");
            DocumentReference docRef = db.Collection("cities").Document("LA");
            Dictionary<string, object> cityObject = new Dictionary<string, object>
            {
                { "Name", "Los Angeles" },
                { "State", "CA" },
                { "Country", "USA" },
                { "Capital", false },
                { "Population", 3900000 }
            };
            await docRef.CreateAsync(cityObject);
            await Task.Delay(3000);

            // Stop the listener when you no longer want to receive updates.
            Console.WriteLine("Stopping the listener");
            await listener.StopAsync();
        }

        private static async Task ListenForChanges(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START firestore_listen_query_changes]
            CollectionReference citiesRef = db.Collection("cities");
            Query query = db.Collection("cities").WhereEqualTo("State", "CA");

            FirestoreChangeListener listener = query.Listen(snapshot =>
            {
                foreach (DocumentChange change in snapshot.Changes)
                {
                    if (change.ChangeType.ToString() == "Added")
                    {
                        Console.WriteLine("New city: {0}", change.Document.Id);
                    }
                    else if (change.ChangeType.ToString() == "Modified")
                    {
                        Console.WriteLine("Modified city: {0}", change.Document.Id);
                    }
                    else if (change.ChangeType.ToString() == "Removed")
                    {
                        Console.WriteLine("Removed city: {0}", change.Document.Id);
                    }
                }
            });
            // [END firestore_listen_query_changes]

            // Create a new document at cities/MTV to demonstrate realtime listener
            Console.WriteLine("Creating document");
            DocumentReference docRef = db.Collection("cities").Document("MTV");
            Dictionary<string, object> cityObject = new Dictionary<string, object>
            {
                { "Name", "Mountain View" },
                { "State", "CA" },
                { "Country", "USA" },
                { "Capital", false },
                { "Population", 80000 }
            };
            await docRef.CreateAsync(cityObject);
            await Task.Delay(3000);

            // Modify the cities/MTV document to demonstrate detection of the 'Modified change
            Console.WriteLine("Modifying document");
            Dictionary<string, object> city = new Dictionary<string, object>
            {
                { "Name", "Mountain View" },
                { "State", "CA" },
                { "Country", "USA" },
                { "Capital", false },
                { "Population", 90000 }
            };
            await docRef.SetAsync(city);
            await Task.Delay(3000);

            // Modify the cities/MTV document to demonstrate detection of the 'Modified change
            Console.WriteLine("Deleting document");
            await docRef.DeleteAsync();
            await Task.Delay(3000);

            // Stop the listener when you no longer want to receive updates.
            Console.WriteLine("Stopping the listener");
            await listener.StopAsync();
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
                case "listen-document":
                    ListenDocument(project).Wait();
                    break;

                case "listen-multiple":
                    ListenMultiple(project).Wait();
                    break;

                case "listen-for-changes":
                    ListenForChanges(project).Wait();
                    break;

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}
