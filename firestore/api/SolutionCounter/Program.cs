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

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace GoogleCloudSamples
{
    // Counter is a collection of documents (shards)
    // to realize counter with high frequency.

    // [START counter_classes]
    // counters/${ID}
    /// <summary>
    /// Defines the <see cref="Counter" />
    /// </summary>
    [FirestoreData]
    public class Counter
    {
        /// <summary>
        /// Gets or sets the NumShards
        /// </summary>
        [FirestoreProperty]
        public int NumShards { get; set; }
    }

    // Shard is a single counter, which is used in a group
    // of other shards within Counter.

    // counters/${ID}/shards/${NUM}
    /// <summary>
    /// Defines the <see cref="Shard" />
    /// </summary>
    [FirestoreData]
    public class Shard
    {
        /// <summary>
        /// Gets or sets the Count
        /// </summary>
        [FirestoreProperty]
        public int Count { get; set; }
    }
    // [END fs_counter_classes]

    /// <summary>
    /// Defines the <see cref="DistributedCounter" />
    /// </summary>
    public class DistributedCounter
    { 
        public static string Usage = @"Usage:
C:\> dotnet run command YOUR_PROJECT_ID

Where command is one of
    run-distributed-counter
";
        /// <summary>
        /// Gets or sets the FireStoreDb
        /// </summary>
        public static FirestoreDb FirestoreDb { get; set; }

        /// <summary>
        /// Gets or sets the ShardsCounter
        /// </summary>
        public static Counter ShardsCounter { get; set; }

        /// <summary>
        /// The InitFireStoreDb
        /// </summary>
        public static void InitFirestoreDb(string project)
        {
            var projectId = project;
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentException("ArgumentException: failed read Firestore Project ID");

            FirestoreDb = FirestoreDb.Create(projectId: projectId);
            Console.WriteLine($"Created Cloud Firestore client with project ID: {project}");
        }

        /// <summary>
        /// The InitShardsCounter
        /// </summary>
        /// <param name="numShards">The numShards<see cref="int"/></param>
        public static void InitShardsCounter(int numShards)
        {
            // Initialize whole global count of shards
            // in ShardsCounter property 
            ShardsCounter = new Counter
            {
                NumShards = numShards
            };
        }

        // [START fs_create_counter]
        /// <summary>
        /// The InitCounterAsync creates a given number of shards as
        /// sub collection of specified document.
        /// </summary>
        /// <param name="docRef">The docRef<see cref="DocumentReference"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public static async Task InitCounterAsync(DocumentReference docRef)
        {
            CollectionReference colRef = docRef.Collection("shards");

            // Initialize each shard with count=0
            for (var i = 0; i < ShardsCounter.NumShards; i++)
            {
                await colRef.Document(i.ToString()).SetAsync(new Shard { Count = 0 });
            }
        }
        // [END fs_create_counter]

        // [START fs_increment_counter]
        /// <summary>
        /// The IncrementCounterAsync increments a randomly picked shard.
        /// </summary>
        /// <param name="docRef">The docRef<see cref="DocumentReference"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public static async Task IncrementCounterAsync(DocumentReference docRef)
        {
            var rand = new Random();
            var docId = rand.Next(0, ShardsCounter.NumShards);
            var shardRef = docRef.Collection("shards").Document(docId.ToString());
            await shardRef.UpdateAsync("Count", FieldValue.Increment(1));
        }

        // [END fs_increment_counter]

        // [START fs_get_count]

        /// <summary>
        /// The GetCount returns a total count across all shards.
        /// </summary>
        /// <param name="docRef">The docRef<see cref="DocumentReference"/></param>
        /// <returns>The <see cref="int"/></returns>
        public static int GetCount(DocumentReference docRef)
        {
            var snapshotList = docRef.Collection("shards").GetSnapshotAsync().Result;
            return snapshotList.Sum(snap => snap.GetValue<int>("Count"));
        }
        // [END fs_get_count]

        private static void RunCodeSample(string project)
        {
            InitFirestoreDb(project);
            InitShardsCounter(5);

            var docRef = FirestoreDb.Collection("counter_samples").Document("DCounter");

            Console.WriteLine("Application start ...");
            InitCounterAsync(docRef);
            Console.WriteLine("Distributed counter initialized.");

            IncrementCounterAsync(docRef);
            Console.WriteLine("Distributed counter incremented.");

            var countTotal = GetCount(docRef);

            Console.WriteLine($"Total count: {countTotal}");
            Console.WriteLine("Application stopped ...");
        }

        /// <summary>
        /// The Main
        /// </summary>
        /// <param name="args">The args<see cref="string[]"/></param>
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
                case "run-distributed-counter":
                    RunCodeSample(project);
                    break;

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}
