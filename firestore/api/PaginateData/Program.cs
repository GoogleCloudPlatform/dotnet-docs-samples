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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    public class PaginateData
    {
        public static string Usage = @"Usage:
C:\> dotnet run command YOUR_PROJECT_ID

Where command is one of
    start-at-field-query-cursor
    end-at-field-query-cursor
    document-snapshot-cursor
    paginated-query-cursor
    multiple-cursor-conditions
";
        private static async Task StartAtFieldQueryCursor(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_start_at_field_query_cursor]
            Query query = citiesRef.OrderBy("Population").StartAt(1000000);
            // [END fs_start_at_field_query_cursor]
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by start at population 1000000 field query cursor", documentSnapshot.Id);
            }
        }

        private static async Task EndAtFieldQueryCursor(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_end_at_field_query_cursor]
            Query query = citiesRef.OrderBy("Population").EndAt(1000000);
            // [END fs_end_at_field_query_cursor]
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by end at population 1000000 field query cursor", documentSnapshot.Id);
            }
        }

        private static async Task DocumentSnapshotCursor(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_document_snapshot_cursor]
            CollectionReference citiesRef = db.Collection("cities");
            DocumentReference docRef = citiesRef.Document("SF");
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            Query query = citiesRef.OrderBy("Population").StartAt(snapshot);
            // [END fs_document_snapshot_cursor]
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by query for cities with population greater than or equal to SF.", documentSnapshot.Id);
            }
        }

        private static async Task PaginatedQueryCursor(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_paginated_query_cursor]
            CollectionReference citiesRef = db.Collection("cities");
            Query firstQuery = citiesRef.OrderBy("Population").Limit(3);

            // Get the last document from the results
            QuerySnapshot querySnapshot = await firstQuery.GetSnapshotAsync();
            long lastPopulation = 0;
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                lastPopulation = documentSnapshot.GetValue<long>("Population");
            }

            // Construct a new query starting at this document.
            // Note: this will not have the desired effect if multiple cities have the exact same population value
            Query secondQuery = citiesRef.OrderBy("Population").StartAfter(lastPopulation);
            QuerySnapshot secondQuerySnapshot = await secondQuery.GetSnapshotAsync();
            // [END fs_paginated_query_cursor]
            foreach (DocumentSnapshot documentSnapshot in secondQuerySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by paginated query cursor.", documentSnapshot.Id);
            }
        }

        private static async Task MultipleCursorConditions(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_multiple_cursor_conditions]
            Query query1 = db.Collection("cities").OrderBy("Name").OrderBy("State").StartAt("Springfield");
            Query query2 = db.Collection("cities").OrderBy("Name").OrderBy("State").StartAt("Springfield", "Missouri");
            // [END fs_multiple_cursor_conditions]
            QuerySnapshot snapshot1 = await query1.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in snapshot1.Documents)
            {
                Console.WriteLine("Document {0} returned by start at Springfield query.", documentSnapshot.Id);
            }
            QuerySnapshot snapshot2 = await query2.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in snapshot2.Documents)
            {
                Console.WriteLine("Document {0} returned by start at Springfield, Missouri query.", documentSnapshot.Id);
            }
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
                case "start-at-field-query-cursor":
                    StartAtFieldQueryCursor(project).Wait();
                    break;

                case "end-at-field-query-cursor":
                    EndAtFieldQueryCursor(project).Wait();
                    break;

                case "document-snapshot-cursor":
                    DocumentSnapshotCursor(project).Wait();
                    break;

                case "paginated-query-cursor":
                    PaginatedQueryCursor(project).Wait();
                    break;

                case "multiple-cursor-conditions":
                    MultipleCursorConditions(project).Wait();
                    break;

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}
