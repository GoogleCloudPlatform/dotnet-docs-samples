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
using NUnit.Framework;
using SolutionCounter.Classes;
using System;
using System.Threading.Tasks;

namespace SolutionCounter.Tests
{
    /// <summary>
    /// Defines the <see cref="SolutionCounter_UnitTests" />
    /// </summary>
    [TestFixture]
    public class SolutionCounter_UnitTests
    {
        /// <summary>
        /// Defines the _api
        /// </summary>
        private readonly ApiFacade _api;

        /// <summary>
        /// Defines the _docRef
        /// </summary>
        private DocumentReference _docRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionCounter_UnitTests"/> class.
        /// </summary>
        public SolutionCounter_UnitTests()
        {
            _api = new ApiFacade();
        }

        /// <summary>
        /// The Setup
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var projectId = Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID");

            var isNullProjectId = string.IsNullOrEmpty(projectId);
            Assert.AreNotEqual(true, isNullProjectId);

            _api.FireStoreDb = FirestoreDb.Create(projectId);
            Assert.AreNotEqual(null, _api.FireStoreDb);

            _docRef = _api.FireStoreDb.Collection("counter_samples_test").Document("TestDocCounter");
            Assert.AreNotEqual(null, _docRef);
        }

        /// <summary>
        /// The TestInitShardsCounter
        /// </summary>
        [Test]
        public void TestInitShardsCounter()
        {
            _api.InitShardsCounter(2);

            var currentShardsCount = _api.ShardsCounter.NumShards;
            Assert.AreNotEqual(0, currentShardsCount);
        }

        /// <summary>
        /// The TestInitCounterAsync
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [Test]
        public async Task TestInitCounterAsync()
        {
            await Task.Run(async () =>
            {
                await _api.InitCounterAsync(_docRef);

            }).ContinueWith(async t =>
            {
                var snapShots = await _docRef.Collection("shards").GetSnapshotAsync();
                Assert.AreNotEqual(0, snapShots.Count);
            });
        }

        /// <summary>
        /// The TestIncrementCounterAsync
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [Test]
        public async Task TestIncrementCounterAsync()
        {
            await Task.Run(async () =>
            {
                await _api.IncrementCounterAsync(_docRef);
                await _api.IncrementCounterAsync(_docRef);

            }).ContinueWith(async t =>
            {
                await Task.Delay(5000);

                var count = _api.GetCount(_docRef);
                Assert.AreEqual(2, count);
            });
        }
    }
}
