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

using Google.Cloud.Firestore;

namespace SolutionCounter.Classes
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
}
