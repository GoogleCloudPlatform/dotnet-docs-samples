// Copyright 2017 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
// [START datastore_quickstart]
using Google.Cloud.Datastore.V1;

namespace GoogleCloudSamples
{
    public class QuickStart
    {
        public static void Main(string[] args)
        {
            // Your Google Cloud Platform project ID
            string projectId = "YOUR-PROJECT-ID";

            // Instantiates a client
            DatastoreDb db = DatastoreDb.Create(projectId);

            // The kind for the new entity
            string kind = "Task";
            // The name/ID for the new entity
            string name = "sampletask1";
            KeyFactory keyFactory = db.CreateKeyFactory(kind);
            // The Cloud Datastore key for the new entity
            Key key = keyFactory.CreateKey(name);

            var task = new Entity
            {
                Key = key,
                ["description"] = "Buy milk"
            };
            using (DatastoreTransaction transaction = db.BeginTransaction())
            {
                // Saves the task
                transaction.Upsert(task);
                transaction.Commit();

                Console.WriteLine($"Saved {task.Key.Path[0].Name}: {(string)task["description"]}");
            }
        }
    }
}
// [END datastore_quickstart]