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
    public class DeleteData
    {
        public static string Usage = @"Usage:
C:\> dotnet run command YOUR_PROJECT_ID

Where command is one of
    delete-doc
    delete-field
    delete-collection
";
        private static async Task DeleteDoc(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_delete_doc]
            DocumentReference cityRef = db.Collection("cities").Document("DC");
            await cityRef.DeleteAsync();
            // [END fs_delete_doc]
            Console.WriteLine("Deleted the DC document in the cities collection.");
        }

        private static async Task DeleteField(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_delete_field]
            DocumentReference cityRef = db.Collection("cities").Document("BJ");
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Capital", FieldValue.Delete }
            };
            await cityRef.UpdateAsync(updates);
            // [END fs_delete_field]
            Console.WriteLine("Deleted the Capital field from the BJ document in the cities collection.");
        }

        // [START fs_delete_collection]
        private static async Task DeleteCollection(CollectionReference collectionReference, int batchSize)
        {
            QuerySnapshot snapshot = await collectionReference.Limit(batchSize).GetSnapshotAsync();
            IReadOnlyList<DocumentSnapshot> documents = snapshot.Documents;
            while (documents.Count > 0)
            {
                foreach (DocumentSnapshot document in documents)
                {
                    Console.WriteLine("Deleting document {0}", document.Id);
                    await document.Reference.DeleteAsync();
                }
                snapshot = await collectionReference.Limit(batchSize).GetSnapshotAsync();
                documents = snapshot.Documents;
            }
            Console.WriteLine("Finished deleting all documents from the collection.");
        }
        // [END fs_delete_collection]

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
                case "delete-doc":
                    DeleteDoc(project).Wait();
                    break;

                case "delete-field":
                    DeleteField(project).Wait();
                    break;

                case "delete-collection":
                    FirestoreDb db = FirestoreDb.Create(project);
                    CollectionReference cityCollection = db.Collection("cities");
                    int batchSize = 2;
                    DeleteCollection(cityCollection, batchSize).Wait();
                    break;

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}
