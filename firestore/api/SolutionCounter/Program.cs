/*
 * Copyright (c) 2019 Google LLC.
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

using Google.Api.Gax;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    /// <summary>
    /// To support more frequent counter updates, create a distributed counter.
    /// Each counter is a document with a subcollection of "shards," and 
    /// the value of the counter is the sum of the value of the shards.
    /// </summary>
    public class DistributedCounter
    {
        private static readonly Random s_rand = new Random();
        private static readonly object s_randLock = new object();
        private const string Usage = @"Usage:
C:\> dotnet run command YOUR_PROJECT_ID

Where command is one of
    distributed-counter
";
        // [START fs_counter_classes]
        /// <summary>
        /// Shard is a document that contains the count.
        /// </summary>
        [FirestoreData]
        public class Shard
        {
            [FirestoreProperty(name: "count")]
            public int Count { get; set; }
        }
        // [END fs_counter_classes]

        // [START fs_create_counter]
        /// <summary>
        /// Create a given number of shards as a
        /// subcollection of specified document.
        /// </summary>
        /// <param name="docRef">The document reference <see cref="DocumentReference"/></param>
        private static async Task CreateCounterAsync(DocumentReference docRef, int numOfShards)
        {
            CollectionReference colRef = docRef.Collection("shards");
            var tasks = new List<Task>();
            // Initialize each shard with Count=0
            for (var i = 0; i < numOfShards; i++)
            {
                tasks.Add(colRef.Document(i.ToString()).SetAsync(new Shard() { Count = 0 }));
            }
            await Task.WhenAll(tasks);
        }
        // [END fs_create_counter]

        // [START fs_increment_counter]
        /// <summary>
        /// Increment a randomly picked shard by 1.
        /// </summary>
        /// <param name="docRef">The document reference <see cref="DocumentReference"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private static async Task IncrementCounterAsync(DocumentReference docRef, int numOfShards)
        {
            int documentId;
            lock (s_randLock)
            {
                documentId = s_rand.Next(numOfShards);
            }
            var shardRef = docRef.Collection("shards").Document(documentId.ToString());
            await shardRef.UpdateAsync("count", FieldValue.Increment(1));
        }
        // [END fs_increment_counter]

        // [START fs_get_count]
        /// <summary>
        /// Get total count across all shards.
        /// </summary>
        /// <param name="docRef">The document reference <see cref="DocumentReference"/></param>
        /// <returns>The <see cref="int"/></returns>
        private static async Task<int> GetCountAsync(DocumentReference docRef)
        {
            var snapshotList = await docRef.Collection("shards").GetSnapshotAsync();
            return snapshotList.Sum(shard => shard.GetValue<int>("count"));
        }
        // [END fs_get_count]

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Write(Usage);
                return;
            }
            string command = args[0].ToLower();
            string projectId = string.Join(" ",
                new ArraySegment<string>(args, 1, args.Length - 1));
            switch (command)
            {
                case "distributed-counter":
                    FirestoreDb db = FirestoreDb.Create(projectId);
                    const int numberOfShards = 5;
                    var docRef = db.Collection("counter_samples").Document("DCounter");

                    Task.Run(() => CreateCounterAsync(docRef, numberOfShards)).WaitWithUnwrappedExceptions();
                    Console.WriteLine("Distributed counter created.");

                    Task.Run(() => IncrementCounterAsync(docRef, numberOfShards)).WaitWithUnwrappedExceptions();
                    Console.WriteLine("Distributed counter incremented.");

                    var countTotal = Task.Run(() => GetCountAsync(docRef)).ResultWithUnwrappedExceptions();
                    Console.WriteLine($"Total count: {countTotal}");
                    break;

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}
