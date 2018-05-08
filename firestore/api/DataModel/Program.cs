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
    public class DataModel
    {
        public static string Usage = @"Usage:
C:\> dotnet run command YOUR_PROJECT_ID

Where command is one of
    document-ref
    collection-ref
    document-path-ref
    subcollection-ref
";

        private static void DocumentRef(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_document_ref]
            DocumentReference documentRef = db.Collection("users").Document("alovelace");
            // [END fs_document_ref]
        }

        private static void CollectionRef(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_collection_ref]
            CollectionReference collectionRef = db.Collection("users");
            // [END fs_collection_ref]
        }

        private static void DocumentPathRef(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_document_path_ref]
            DocumentReference documentRef = db.Document("users/alovelace");
            // [END fs_document_path_ref]
        }

        private static void SubcollectionRef(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_subcollection_ref]
            DocumentReference documentRef = db
                .Collection("Rooms").Document("RoomA")
                .Collection("Messages").Document("Message1");
            // [END fs_subcollection_ref]
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
                case "document-ref":
                    DocumentRef(project);
                    break;

                case "collection-ref":
                    CollectionRef(project);
                    break;

                case "document-path-ref":
                    DocumentPathRef(project);
                    break;

                case "subcollection-ref":
                    SubcollectionRef(project);
                    break;

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}
