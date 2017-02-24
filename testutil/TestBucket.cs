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

using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace GoogleCloudSamples
{
    /// <summary>
    /// Creates a bucket with a random name for use in tests.  Call Dispose()
    /// to delete the bucket.
    /// </summary>
    public class RandomBucketFixture : IDisposable
    {
        readonly StorageClient _storage = StorageClient.Create();
        readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        public RandomBucketFixture()
        {
            BucketName = RandomBucketName();
            _storage.CreateBucket(_projectId, BucketName);
        }

        public void Dispose()
        {
            var robot = new RetryRobot()
            {
                MaxTryCount = 10,
                ShouldRetry = (e) => true,
            };
            robot.Eventually(() =>
            {
                _storage.DeleteBucket(BucketName);
            });
        }

        private static string RandomBucketName()
        {
            return TestUtil.RandomName();
        }

        public string BucketName { get; private set; }
    }

    /// <summary>
    /// Tracks objects in a Cloud Storage bucket for later clean up.
    /// Call Dispose() to clean up.
    /// </summary>
    public class BucketCollector : IDisposable
    {
        readonly StorageClient _storage = StorageClient.Create();
        readonly string _bucketName;
        readonly SortedDictionary<string, SortedSet<string>> _garbage =
        new SortedDictionary<string, SortedSet<string>>();

        public BucketCollector(string bucketName)
        {
            _bucketName = bucketName;
        }

        /// <summary>
        /// Record the name of a Cloud Storage object for later clean up.
        /// </summary>
        /// <returns>The object name.</returns>
        public string Collect(string objectName)
        {
            return Collect(_bucketName, objectName);
        }

        /// <summary>
        /// Record the name of a Cloud Storage object for later clean up.
        /// </summary>
        /// <returns>The object name.</returns>
        public string Collect(string bucketName, string objectName)
        {
            SortedSet<string> objectNames;
            if (!_garbage.TryGetValue(bucketName, out objectNames))
            {
                objectNames = _garbage[bucketName] = new SortedSet<string>();
            }
            objectNames.Add(objectName);
            return objectName;
        }

        /// <summary>
        /// Copies a local file to the Cloud Storage bucket and records its
        /// name for later clean up.
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="objectName"></param>
        public void CopyToBucket(string localPath, string objectName)
        {
            using (Stream m = new FileStream(localPath, FileMode.Open))
            {
                _storage.UploadObject(_bucketName, objectName, null, m);
                Collect(objectName);
            }
        }

        public void Dispose()
        {
            var robot = new RetryRobot()
            {
                MaxTryCount = 10,
                ShouldRetry = (e) => true,
            };
            foreach (var bucket in _garbage)
            {
                foreach (var objectName in bucket.Value)
                {
                    robot.Eventually(() =>
                    {
                        _storage.DeleteObject(bucket.Key, objectName);
                    });
                }
            }
            _garbage.Clear();
        }
    }
}
