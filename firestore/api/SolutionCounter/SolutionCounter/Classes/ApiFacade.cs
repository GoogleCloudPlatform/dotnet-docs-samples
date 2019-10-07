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
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace SolutionCounter.Classes
{
    /// <summary>
    /// Defines the <see cref="ApiFacade" />
    /// </summary>
    public class ApiFacade
    {
        /// <summary>
        /// Gets or sets the FireStoreDb
        /// </summary>
        public FirestoreDb FireStoreDb { get; set; }

        /// <summary>
        /// Gets or sets the ShardsCounter
        /// </summary>
        public Counter ShardsCounter { get; set; }

        /// <summary>
        /// The InitFireStoreDb
        /// </summary>
        public void InitFireStoreDb()
        {
            var projectId = Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID");

            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentException("ArgumentException: failed read Firestore Project ID");

            FireStoreDb = FirestoreDb.Create(projectId: projectId);
        }

        /// <summary>
        /// The InitShardsCounter
        /// </summary>
        /// <param name="numShards">The numShards<see cref="int"/></param>
        public void InitShardsCounter(int numShards)
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
        /// subcollection of specified document.
        /// </summary>
        /// <param name="docRef">The docRef<see cref="DocumentReference"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task InitCounterAsync(DocumentReference docRef)
        {
            if (ShardsCounter.NumShards == 0)
                throw new ArgumentException("ArgumentException: NumShards must be more than 0");

            CollectionReference colRef = docRef.Collection("shards");

            // Initialize each shard with count=0
            for (var i = 0; i < ShardsCounter.NumShards; i++)
            {
                var docNew = colRef.Document(i.ToString());

                await docNew.SetAsync(new Shard { Count = 0 });
            }
        }

        // [END fs_create_counter]

        // [START fs_increment_counter]

        /// <summary>
        /// The IncrementCounterAsync increments a randomly picked shard.
        /// </summary>
        /// <param name="docRef">The docRef<see cref="DocumentReference"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task IncrementCounterAsync(DocumentReference docRef)
        {
            if (ShardsCounter.NumShards == 0)
                throw new ArgumentException("ArgumentException: NumShards must be more than 0");

            var rand = new Random();

            var docId = rand.Next(0, ShardsCounter.NumShards);

            var shardRef = docRef.Collection("shards").Document(docId.ToString());

            var shard = await shardRef.GetSnapshotAsync();

            var currentCount = shard.GetValue<int>("Count");

            await shardRef.UpdateAsync("Count", ++currentCount);
        }

        // [END fs_increment_counter]

        // [START fs_get_count]

        /// <summary>
        /// The GetCount returns a total count across all shards.
        /// </summary>
        /// <param name="docRef">The docRef<see cref="DocumentReference"/></param>
        /// <returns>The <see cref="int"/></returns>
        public int GetCount(DocumentReference docRef)
        {
            var snapshotList = docRef.Collection("shards").GetSnapshotAsync().Result;

            return snapshotList.Sum(snap => snap.GetValue<int>("Count"));
        }

        // [END fs_get_count]
    }
}
