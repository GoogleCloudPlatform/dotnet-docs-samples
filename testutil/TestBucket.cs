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

using Google.Api.Gax;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GoogleCloudSamples
{
    /// <summary>
    /// Creates a bucket with a random name for use in tests.  Call Dispose()
    /// to delete the bucket.
    /// </summary>
    public class RandomBucketFixture : IDisposable
    {
        private readonly StorageClient _storage = StorageClient.Create();
        private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        public RandomBucketFixture()
        {
            BucketName = RandomBucketName();
            _storage.CreateBucket(_projectId, BucketName);
        }

        public void Dispose()
        {
            int retryDelayMs = 0;
            for (int errorCount = 0; errorCount < 10; ++errorCount)
            {
                Thread.Sleep(retryDelayMs);
                retryDelayMs = (retryDelayMs + 1000) * 2;
                PagedEnumerable<Google.Apis.Storage.v1.Data.Objects,
                    Google.Apis.Storage.v1.Data.Object> objectsInBucket;
                try
                {
                    objectsInBucket = _storage.ListObjects(BucketName);
                }
                catch (Google.GoogleApiException e)
                when (e.Error.Code == 404)
                {
                    return;  // Bucket does not exist.  Ok.
                }

                // Try to delete each object in the bucket.
                foreach (var obj in objectsInBucket)
                {
                    try
                    {
                        _storage.DeleteObject(obj);
                    }
                    catch (Google.GoogleApiException)
                    {
                        continue;
                    }
                }

                try
                {
                    _storage.DeleteBucket(BucketName);
                }
                catch (Google.GoogleApiException e)
                when (e.Error.Code == 404)
                {
                    return;  // Bucket does not exist.  Ok.
                }
                catch (Google.GoogleApiException)
                {
                    continue;
                }
            }
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
        private readonly StorageClient _storage = StorageClient.Create();
        private readonly string _bucketName;

        private readonly SortedDictionary<string, SortedSet<string>> _garbage =
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
            using (Stream m = new FileStream(localPath, FileMode.Open,
                FileAccess.Read, FileShare.Read))
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