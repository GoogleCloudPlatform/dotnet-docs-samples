// Copyright 2016 Google Inc.
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

using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Xunit;

namespace GoogleCloudSamples
{
    public class BaseTest
    {
        private static readonly RetryRobot s_retryTransientRpcErrors = new RetryRobot
        {
            RetryWhenExceptions = new[] { typeof(Newtonsoft.Json.JsonReaderException) }
        };

        private static readonly CommandLineRunner s_runner = new CommandLineRunner
        {
            Command = "Storage.exe",
            Main = Storage.Main,
        };

        /// <summary>Runs StorageSample.exe with the provided arguments</summary>
        /// <returns>The console output of this program</returns>
        public static ConsoleOutput Run(params string[] arguments)
        {
            return s_retryTransientRpcErrors.Eventually(
                () => s_runner.Run(arguments));
        }

        protected static void AssertSucceeded(ConsoleOutput output)
        {
            Assert.True(0 == output.ExitCode,
                $"Exit code: {output.ExitCode}\n{output.Stdout}");
        }
    }

    public class BadCommandTests : BaseTest
    {
        [Fact]
        public void TestNoArgs()
        {
            var ran = Run();
            Assert.Equal(-1, ran.ExitCode);
            Assert.Contains("Storage", ran.Stdout);
        }

        [Fact]
        public void TestBadCommand()
        {
            var ran = Run("throw");
            Assert.Equal(-1, ran.ExitCode);
            Assert.Contains("Storage", ran.Stdout);
        }

        [Fact]
        public void TestMissingDeleteArg()
        {
            var ran = Run("delete");
            Assert.Equal(-1, ran.ExitCode);
            Assert.Contains("Storage", ran.Stdout);
        }
    }

    /// <summary>
    /// Trying to create a bucket for each test will exceed our rate limit.
    /// Therefore, share one bucket across all the tests.
    /// </summary>
    public class BucketFixture : IDisposable
    {
        public BucketFixture()
        {
            BucketName = StorageTest.CreateRandomBucket();
            BucketName1 = StorageTest.CreateRandomRegionalBucket();
        }
        public void Dispose()
        {
            StorageTest.DeleteBucket(BucketName);
            StorageTest.DeleteBucket(BucketName1);
        }

        public string BucketName { get; private set; }
        public string BucketName1 { get; private set; }
    }

    public class GarbageCollector : IDisposable
    {
        private readonly StorageTest _test;
        public GarbageCollector(StorageTest test)
        {
            _test = test;
        }
        public void Dispose()
        {
            _test.DeleteGarbage();
        }
    }

    public class StorageTest : BaseTest, IDisposable, IClassFixture<BucketFixture>
    {
        private readonly string _bucketName;
        private readonly string _bucketName1;
        private readonly string _kmsKeyRing =
            Environment.GetEnvironmentVariable("STORAGE_KMS_KEYRING");
        private readonly string _kmsKeyName =
            Environment.GetEnvironmentVariable("STORAGE_KMS_KEYNAME");
        private readonly static string s_kmsKeyLocation = "us-west1";

        /// <summary>
        /// Maintain a list of objects that must be deleted at the end of the test.
        /// </summary>
        private readonly SortedDictionary<string, SortedSet<string>> _garbage =
            new SortedDictionary<string, SortedSet<string>>();

        public StorageTest(BucketFixture fixture)
        {
            _bucketName = fixture.BucketName;
            _bucketName1 = fixture.BucketName1;
        }

        /// <summary>
        /// Add an object to delete at the end of the test.
        /// </summary>
        /// <returns>The objectName.</returns>
        private string Collect(string bucketName, string objectName)
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
        /// Add an object to delete at the end of the test.
        /// </summary>
        /// <returns>The objectName.</returns>
        private string Collect(string objectName) => Collect(_bucketName, objectName);

        /// <summary>
        /// Add a object located in a regional bucket to delete
        /// at the end of the test.
        /// </summary>
        /// <returns>The regional objectName.</returns>
        private string CollectRegionalObject(string objectName)
            => Collect(_bucketName1, objectName);

        internal static void DeleteBucket(string bucketName)
        {
            Eventually(() => AssertSucceeded(Run("delete", bucketName)));
        }

        public void Dispose()
        {
            DeleteGarbage();
        }

        internal void DeleteGarbage()
        {
            foreach (var bucket in _garbage)
            {
                List<string> args = new List<string>();
                args.Add("delete");
                args.Add(bucket.Key);
                args.AddRange(bucket.Value);
                AssertSucceeded(Run(args.ToArray()));
            }
            _garbage.Clear();
        }

        private static readonly RetryRobot s_retryFailedAssertions = new RetryRobot()
        {
            RetryWhenExceptions = new[] { typeof(Xunit.Sdk.XunitException) },
            MaxTryCount = 6,
        };

        /// <summary>
        /// Retry action.
        /// Datastore guarantees only eventual consistency.  Many tests write
        /// an entity and then query it afterward, but may not find it immediately.
        /// </summary>
        /// <param name="action"></param>
        private static void Eventually(Action action) =>
            s_retryFailedAssertions.Eventually(action);

        public static string CreateRandomBucket()
        {
            var created = Run("create");
            AssertSucceeded(created);
            var created_regex = new Regex(@"Created\s+(.+)\.\s*", RegexOptions.IgnoreCase);
            var match = created_regex.Match(created.Stdout);
            Assert.True(match.Success);
            string bucketName = match.Groups[1].Value;
            return bucketName;
        }

        public static string CreateRandomRegionalBucket()
        {
            var created = Run("create-regional-bucket", s_kmsKeyLocation);
            AssertSucceeded(created);
            var created_regex = new Regex(@"Created\s+(.+)\.\s*", RegexOptions.IgnoreCase);
            var match = created_regex.Match(created.Stdout);
            Assert.True(match.Success);
            string bucketName = match.Groups[1].Value;
            return bucketName;
        }

        [Fact]
        public void TestCreateBucket()
        {
            // Try creating another bucket with the same name.  Should fail.
            var created_again = Run("create", _bucketName);
            Assert.Equal(409, created_again.ExitCode);

            // Try listing the buckets.  We should find the new one.
            Eventually(() =>
            {
                var listed = Run("list");
                AssertSucceeded(listed);
                Assert.Contains(_bucketName, listed.Stdout);
            });
        }

        [Fact]
        public void TestListObjectsInBucket()
        {
            // Try listing the files.  There should be none.
            Eventually(() =>
            {
                var listed = Run("list", _bucketName);
                AssertSucceeded(listed);
                Assert.Equal("", listed.Stdout);
            });

            var uploaded = Run("upload", _bucketName, "Hello.txt", Collect("HelloListObjectsTest.txt"));
            AssertSucceeded(uploaded);

            Eventually(() =>
            {
                var listed = Run("list", _bucketName);
                AssertSucceeded(listed);
                Assert.Contains("HelloListObjectsTest.txt", listed.Stdout);
            });
        }

        public string[] SplitOutput(string stdout) =>
            stdout.Split('\n')
                .Select((s) => s.Trim()).Where((s) => !string.IsNullOrEmpty(s))
                .OrderBy((s) => s).ToArray();

        [Fact]
        public void TestListObjectsInBucketWithPrefix()
        {
            // Try listing the files.  There should be none.
            Eventually(() =>
            {
                var listed = Run("list", _bucketName);
                AssertSucceeded(listed);
                Assert.Equal("", listed.Stdout);
            });

            // Upload 4 files.
            var uploaded = Run("upload", _bucketName, "Hello.txt", Collect("a/1.txt"));
            AssertSucceeded(uploaded);
            uploaded = Run("upload", _bucketName, "Hello.txt", Collect("a/2.txt"));
            AssertSucceeded(uploaded);
            uploaded = Run("upload", _bucketName, "Hello.txt", Collect("b/2.txt"));
            AssertSucceeded(uploaded);
            uploaded = Run("upload", _bucketName, "Hello.txt", Collect("a/b/3.txt"));
            AssertSucceeded(uploaded);

            Eventually(() =>
            {
                // With no delimiter, we should get all 3 files.
                var listed = Run("list", _bucketName, "a/", null);
                AssertSucceeded(listed);
                Assert.Equal(new string[] {
                    "a/1.txt",
                    "a/2.txt",
                    "a/b/3.txt"
                }, SplitOutput(listed.Stdout));

                // With a delimeter, we should see only direct contents.
                listed = Run("list", _bucketName, "a/", "/");
                AssertSucceeded(listed);
                Assert.Equal(new string[] {
                    "a/1.txt",
                    "a/2.txt",
                }, SplitOutput(listed.Stdout));
            });
        }

        [Fact]
        public void TestDownloadObject()
        {
            var uploaded = Run("upload", _bucketName, "Hello.txt", Collect("HelloDownloadObject.txt"));
            AssertSucceeded(uploaded);
            uploaded = Run("upload", _bucketName, "Hello.txt", Collect("Hello2.txt"));
            AssertSucceeded(uploaded);

            var downloaded = Run("download", _bucketName, "Hello2.txt");
            AssertSucceeded(downloaded);
            try
            {
                Assert.Equal(File.ReadAllText("Hello.txt"),
                    File.ReadAllText("Hello2.txt"));
                downloaded = Run("download", _bucketName, "HelloDownloadObject.txt");
                AssertSucceeded(downloaded);
                Assert.Equal(File.ReadAllText("HelloDownloadObject.txt"),
                    File.ReadAllText("Hello2.txt"));
            }
            finally
            {
                File.Delete("HelloDownloadObject.txt");
                File.Delete("Hello2.txt");
            }
        }

        [Fact]
        public void TestDownloadCompleteByteRange()
        {
            var uploaded = Run("upload", _bucketName, "Hello.txt", Collect("HelloDownloadCompleteByteRange.txt"));
            AssertSucceeded(uploaded);

            var downloaded = Run("download-byte-range", _bucketName,
                "HelloDownloadCompleteByteRange.txt", "0", "20");
            AssertSucceeded(downloaded);
            try
            {
                var helloBytes = File.ReadAllBytes("Hello.txt");
                Assert.Equal(
                    helloBytes,
                    File.ReadAllBytes("HelloDownloadCompleteByteRange.txt_0-20"));
            }
            finally
            {
                File.Delete("HelloDownloadCompleteByteRange.txt_0-20");
            }
        }

        [Fact]
        public void TestDownloadPartialByteRange()
        {
            var uploaded = Run("upload", _bucketName, "Hello.txt", Collect("HelloDownloadPartialByteRange.txt"));
            AssertSucceeded(uploaded);

            var downloaded = Run("download-byte-range", _bucketName,
                "HelloDownloadPartialByteRange.txt", "1", "5");
            AssertSucceeded(downloaded);
            try
            {
                var helloBytes = File.ReadAllBytes("Hello.txt");
                Assert.Equal(
                    helloBytes.Skip(1).Take(5).ToArray(),
                    File.ReadAllBytes("HelloDownloadPartialByteRange.txt_1-5"));
            }
            finally
            {
                File.Delete("HelloDownloadPartialByteRange.txt_1-5");
            }
        }

        [Fact]
        public void TestGetMetadata()
        {
            var uploaded = Run("upload", _bucketName, "Hello.txt", Collect("HelloGetMetadata.txt"));
            var got = Run("get-metadata", _bucketName, "HelloGetMetadata.txt");
            AssertSucceeded(got);
            Assert.Contains("Generation", got.Stdout);
            Assert.Contains("Size", got.Stdout);
        }

        [Fact]
        public void TestMakePublic()
        {
            var uploaded = Run("upload", _bucketName, "Hello.txt", Collect("HelloMakePublic.txt"));
            var got = Run("get-metadata", _bucketName, "HelloMakePublic.txt");
            AssertSucceeded(got);
            var medialink_regex = new Regex(@"MediaLink:\s?(.+)");
            var match = medialink_regex.Match(got.Stdout);
            Assert.True(match.Success);

            // Before making the file public, fetching the medialink should
            // throw an exception.
            string medialink = match.Groups[1].Value.Trim();
            WebClient webClient = new WebClient();
            Assert.Throws<WebException>(() =>
                webClient.DownloadString(medialink));

            // Make it public and try fetching again.
            var madePublic = Run("make-public", _bucketName, "HelloMakePublic.txt");
            AssertSucceeded(madePublic);
            var text = webClient.DownloadString(medialink);
            Assert.Equal(File.ReadAllText("Hello.txt"), text);
        }

        [Fact]
        public void TestMove()
        {
            Run("upload", _bucketName, "Hello.txt", Collect("HelloMove.txt"));
            // Make sure the file doesn't exist until we move it there.
            var got = Run("get-metadata", _bucketName, "ByeMove.txt");
            Assert.Equal(404, got.ExitCode);
            Eventually(() =>
            {
                // Now move it there.
                AssertSucceeded(Run("move", _bucketName, "HelloMove.txt",
                    Collect("ByeMove.txt")));
            });
            // If we try to clean up "HelloMove.txt", it will fail because it moved.
            _garbage[_bucketName].Remove("HelloMove.txt");
            AssertSucceeded(Run("get-metadata", _bucketName, "ByeMove.txt"));
        }

        [Fact]
        public void TestCopy()
        {
            Run("upload", _bucketName, "Hello.txt", Collect("HelloCopy.txt"));
            using (var otherBucket = new BucketFixture())
            {
                AssertSucceeded(Run("copy", _bucketName, "HelloCopy.txt",
                    otherBucket.BucketName, "ByeCopy.txt"));
                try
                {
                    AssertSucceeded(Run("get-metadata", otherBucket.BucketName,
                        "ByeCopy.txt"));
                }
                finally
                {
                    Run("delete", otherBucket.BucketName, "ByeCopy.txt");
                }
            }
        }

        [Fact]
        public void TestPrintBucketAcl()
        {
            var printedAcl = Run("print-acl", _bucketName);
            AssertSucceeded(printedAcl);
        }

        [Fact]
        public void TestHmacSamples()
        {
            //These need to all run as one test so that we can use the created key in every test
            DeleteAllHmacKeys(Storage.s_projectId);

            String serviceAccountEmail = GetServiceAccountEmail();
            var createdHmacKey = Run("create-hmac-key", serviceAccountEmail);
            AssertSucceeded(createdHmacKey);

            String id = Regex.Match(createdHmacKey.Stdout, @"Access ID: ([0-9A-Z]+)").Groups[1].Value;

            var listedHmacKeys = Run("list-hmac-keys");
            AssertSucceeded(listedHmacKeys);
            Assert.Contains(id, listedHmacKeys.Stdout);

            var fetchedHmacKey = Run("get-hmac-key", id);
            AssertSucceeded(fetchedHmacKey);
            Assert.Contains(id, fetchedHmacKey.Stdout);

            var deactivatedHmacKey = Run("deactivate-hmac-key", id);
            AssertSucceeded(deactivatedHmacKey);
            Assert.Contains(id, deactivatedHmacKey.Stdout);

            var reactivatedHmacKey = Run("activate-hmac-key", id);
            AssertSucceeded(reactivatedHmacKey);
            Assert.Contains(id, reactivatedHmacKey.Stdout);

            Run("deactivate-hmac-key", id);

            var deletedKey = Run("delete-hmac-key", id);
            AssertSucceeded(deletedKey);
            Assert.Contains(id, deletedKey.Stdout);

            listedHmacKeys = Run("list-hmac-keys", serviceAccountEmail);
            AssertSucceeded(listedHmacKeys);
            Assert.DoesNotContain(id, listedHmacKeys.Stdout);
        }

        [Fact]
        public void TestAddBucketOwner()
        {
            using (var bucket = new BucketFixture())
            {
                string userEmail = "gcs-iam-acl-test@dotnet-docs-samples-tests.iam.gserviceaccount.com";
                var printedAcl = Run("print-acl", bucket.BucketName);
                AssertSucceeded(printedAcl);
                Assert.DoesNotContain(userEmail, printedAcl.Stdout);
                var printedAclForUser = Run("print-acl-for-user", _bucketName, userEmail);
                Assert.Equal("", printedAclForUser.Stdout);

                var addedOwner = Run("add-owner", bucket.BucketName, userEmail);
                AssertSucceeded(addedOwner);

                printedAcl = Run("print-acl", bucket.BucketName);
                AssertSucceeded(printedAcl);
                Assert.Contains(userEmail, printedAcl.Stdout);

                // Make sure we print-acl-for-user shows us the user, but not all the ACLs.
                printedAclForUser = Run("print-acl-for-user", bucket.BucketName, userEmail);
                Assert.Contains(userEmail, printedAclForUser.Stdout);
                Assert.True(printedAcl.Stdout.Length > printedAclForUser.Stdout.Length);

                // Remove the owner.
                var removedOwner = Run("remove-owner", bucket.BucketName, userEmail);
                AssertSucceeded(addedOwner);
                printedAcl = Run("print-acl", bucket.BucketName);
                AssertSucceeded(printedAcl);
                Assert.DoesNotContain(userEmail, printedAcl.Stdout);
            }
        }

        [Fact]
        public void TestAddDefaultOwner()
        {
            using (var bucket = new BucketFixture())
            {
                string userEmail = "gcs-iam-acl-test@dotnet-docs-samples-tests.iam.gserviceaccount.com";
                var printedAcl = Run("print-default-acl", bucket.BucketName);
                AssertSucceeded(printedAcl);
                Assert.DoesNotContain(userEmail, printedAcl.Stdout);

                // Add the default owner.
                var addedOwner = Run("add-default-owner", bucket.BucketName,
                    userEmail);
                AssertSucceeded(addedOwner);

                printedAcl = Run("print-default-acl", bucket.BucketName);
                AssertSucceeded(printedAcl);
                Assert.Contains(userEmail, printedAcl.Stdout);

                // Remove the default owner.
                var removedOwner = Run("remove-default-owner", bucket.BucketName,
                    userEmail);
                AssertSucceeded(removedOwner);

                printedAcl = Run("print-default-acl", bucket.BucketName);
                AssertSucceeded(printedAcl);
                Assert.DoesNotContain(userEmail, printedAcl.Stdout);
            }
        }

        [Fact]
        public void TestAddObjectOwner()
        {
            string userEmail = "gcs-iam-acl-test@dotnet-docs-samples-tests.iam.gserviceaccount.com";
            Run("upload", _bucketName, "Hello.txt", Collect("HelloAddObjectOwner.txt"));
            var printedAcl = Run("print-acl", _bucketName, "HelloAddObjectOwner.txt");
            AssertSucceeded(printedAcl);
            Assert.DoesNotContain(userEmail, printedAcl.Stdout);
            var printedAclForUser = Run("print-acl-for-user", _bucketName,
                "HelloAddObjectOwner.txt", userEmail);
            Assert.Equal("", printedAclForUser.Stdout);

            // Add the owner.
            var addedOwner = Run("add-owner", _bucketName,
                "HelloAddObjectOwner.txt", userEmail);
            AssertSucceeded(addedOwner);

            // Make sure we print-acl shows us the user.
            printedAcl = Run("print-acl", _bucketName, "HelloAddObjectOwner.txt");
            AssertSucceeded(printedAcl);
            Assert.Contains(userEmail, printedAcl.Stdout);

            // Make sure we print-acl-for-user shows us the user,
            // but not all the ACLs.
            printedAclForUser = Run("print-acl-for-user", _bucketName,
                "HelloAddObjectOwner.txt", userEmail);
            Assert.Contains(userEmail, printedAclForUser.Stdout);
            Assert.True(printedAcl.Stdout.Length >
                printedAclForUser.Stdout.Length);

            // Remove the owner.
            var removedOwner = Run("remove-owner", _bucketName, "HelloAddObjectOwner.txt",
                userEmail);
            AssertSucceeded(removedOwner);
            printedAcl = Run("print-acl", _bucketName, "HelloAddObjectOwner.txt");
            AssertSucceeded(printedAcl);
            Assert.DoesNotContain(userEmail, printedAcl.Stdout);
        }

        [Fact]
        public void TestViewBucketIamMembers()
        {
            var printedIamMembers = Run("view-bucket-iam-members", _bucketName);
            AssertSucceeded(printedIamMembers);
        }

        [Fact]
        public void TestAddBucketIamMember()
        {
            using (var iamTests = new BucketFixture())
            {
                string member = "gcs-iam-acl-test@dotnet-docs-samples-tests.iam.gserviceaccount.com";
                string memberType = "serviceAccount";
                string role = "roles/storage.objectViewer";

                // Add the member.
                var addedMember = Run("add-bucket-iam-member", iamTests.BucketName,
                    role, $"{memberType}:{member}");
                AssertSucceeded(addedMember);

                // Make sure view-bucket-iam-members shows us the user.
                var printedIamMembers = Run("view-bucket-iam-members", iamTests.BucketName);
                AssertSucceeded(printedIamMembers);
                Assert.Contains(member, printedIamMembers.Stdout);

                // Remove the member.
                var removedMember = Run("remove-bucket-iam-member", iamTests.BucketName,
                    role, $"{memberType}:{member}");
                AssertSucceeded(removedMember);
                printedIamMembers = Run("view-bucket-iam-members", iamTests.BucketName);
                AssertSucceeded(printedIamMembers);
                Assert.DoesNotContain(member, printedIamMembers.Stdout);

                // Enable Uniform Bucket Level Access
                var enableUniformBucketLevelAccess = Run("enable-uniform-bucket-level-access", iamTests.BucketName);
                AssertSucceeded(enableUniformBucketLevelAccess);

                // Add Conditional Binding
                var addBucketConditionalIamBinding = Run("add-bucket-iam-conditional-binding", iamTests.BucketName,
                    role, $"{memberType}:{member}", "title", "description",
                    "resource.name.startsWith(\"projects/_/buckets/bucket-name/objects/prefix-a-\")");
                AssertSucceeded(addBucketConditionalIamBinding);

                // Remove Conditional Binding
                var removeBucketConditionalIamBinding = Run("remove-bucket-iam-conditional-binding", iamTests.BucketName,
                    role, "title", "description",
                    "resource.name.startsWith(\"projects/_/buckets/bucket-name/objects/prefix-a-\")");
                Assert.Contains("Conditional Binding was removed.", removeBucketConditionalIamBinding.Stdout);
                AssertSucceeded(removeBucketConditionalIamBinding);
            }
        }

        [Fact]
        public void TestAddBucketDefaultKmsKey()
        {
            var addBucketDefaultKmsKeyResponse =
                Run("add-bucket-default-kms-key", _bucketName1,
                    s_kmsKeyLocation, _kmsKeyRing, _kmsKeyName);
            AssertSucceeded(addBucketDefaultKmsKeyResponse);
            var bucketMetadata = Run("get-bucket-metadata", _bucketName1);
            Assert.Contains(_kmsKeyName, bucketMetadata.Stdout);
        }

        [Fact]
        public void TestUploadWithKmsKey()
        {
            var uploadWithKmsKeyResponse = Run("upload-with-kms-key",
                    _bucketName1, s_kmsKeyLocation, _kmsKeyRing,
                    _kmsKeyName, "Hello.txt", CollectRegionalObject("HelloUploadWithKmsKey.txt"));
            AssertSucceeded(uploadWithKmsKeyResponse);
            var objectMetadata = Run("get-metadata", _bucketName1, "HelloUploadWithKmsKey.txt");
            Assert.Contains(_kmsKeyName, objectMetadata.Stdout);
        }

        [Fact]
        public void TestSignUrl()
        {
            Run("upload", _bucketName, "Hello.txt", Collect("HelloSignUrl.txt"));
            var output = Run("generate-signed-url", _bucketName, "HelloSignUrl.txt");
            AssertSucceeded(output);
            // Try fetching the url to make sure it works.
            var client = new HttpClient();
            var response = client.GetAsync(output.Stdout).Result;
            Assert.InRange((int)response.StatusCode, 200, 299);
        }

        [Fact]
        public void TestBucketLockBucket()
        {
            using (var bucketLock = new BucketFixture())
            {
                var retentionPeriod = "5";
                var setRetentionPolicy = Run("set-bucket-retention-policy", bucketLock.BucketName, retentionPeriod);
                AssertSucceeded(setRetentionPolicy);
                var getRetentionPolicy = Run("get-bucket-retention-policy", bucketLock.BucketName);
                AssertSucceeded(getRetentionPolicy);
                Assert.Contains($"period: {retentionPeriod}", getRetentionPolicy.Stdout);
                var enableBucketDefaultEventBasedHold = Run("enable-bucket-default-event-based-hold", _bucketName);
                AssertSucceeded(enableBucketDefaultEventBasedHold);
                var getBucketDefaultEventBasedHold = Run("get-bucket-default-event-based-hold", _bucketName);
                Assert.Equal(1, getBucketDefaultEventBasedHold.ExitCode);
                Assert.Contains("Default event-based hold: True", getBucketDefaultEventBasedHold.Stdout);
                var disableBucketDefaultEventBasedHold = Run("disable-bucket-default-event-based-hold", _bucketName);
                AssertSucceeded(enableBucketDefaultEventBasedHold);
                getBucketDefaultEventBasedHold = Run("get-bucket-default-event-based-hold", _bucketName);
                Assert.Equal(0, getBucketDefaultEventBasedHold.ExitCode);
                Assert.Contains("Default event-based hold: False", getBucketDefaultEventBasedHold.Stdout);
                var removeRetentionPolicy = Run("remove-bucket-retention-policy", bucketLock.BucketName);
                AssertSucceeded(setRetentionPolicy);
                getRetentionPolicy = Run("get-bucket-retention-policy", bucketLock.BucketName);
                AssertSucceeded(getRetentionPolicy);
                Assert.DoesNotContain($"period:", getRetentionPolicy.Stdout);
                setRetentionPolicy = Run("set-bucket-retention-policy", bucketLock.BucketName, retentionPeriod);
                AssertSucceeded(setRetentionPolicy);
                var lockRetentionPolicy = Run("lock-bucket-retention-policy", bucketLock.BucketName);
                AssertSucceeded(lockRetentionPolicy);
            }
        }

        [Fact]
        public void TestBucketLockObject()
        {
            using (var bucketLock = new BucketFixture())
            {
                var objectName = "HelloBucketLock.txt";
                try
                {
                    var uploaded = Run("upload", bucketLock.BucketName,
                        "Hello.txt", objectName);
                    AssertSucceeded(uploaded);
                    var setTemporaryHold = Run("set-object-temporary-hold", bucketLock.BucketName, objectName);
                    AssertSucceeded(setTemporaryHold);
                    var getMetadata = Run("get-metadata", bucketLock.BucketName, objectName);
                    AssertSucceeded(getMetadata);
                    Assert.Contains("Temporary hold enabled? True", getMetadata.Stdout);
                    var releaseTemporaryHold = Run("release-object-temporary-hold", bucketLock.BucketName, objectName);
                    AssertSucceeded(releaseTemporaryHold);
                    getMetadata = Run("get-metadata", bucketLock.BucketName, objectName);
                    AssertSucceeded(getMetadata);
                    Assert.Contains("Temporary hold enabled? False", getMetadata.Stdout);
                    var retentionPeriod = "5";
                    var setRetentionPolicy = Run("set-bucket-retention-policy", bucketLock.BucketName, retentionPeriod);
                    AssertSucceeded(setRetentionPolicy);
                    var setEventBasedHold = Run("set-object-event-based-hold", bucketLock.BucketName, objectName);
                    AssertSucceeded(setEventBasedHold);
                    getMetadata = Run("get-metadata", bucketLock.BucketName, objectName);
                    AssertSucceeded(getMetadata);
                    Assert.Contains("Event-based hold enabled? True", getMetadata.Stdout);
                    var releaseEventBasedHold = Run("release-object-event-based-hold", bucketLock.BucketName, objectName);
                    AssertSucceeded(releaseEventBasedHold);
                    getMetadata = Run("get-metadata", bucketLock.BucketName, objectName);
                    AssertSucceeded(getMetadata);
                    Assert.Contains("Event-based hold enabled? False", getMetadata.Stdout);
                    var removeRetentionPolicy = Run("remove-bucket-retention-policy", bucketLock.BucketName);
                    AssertSucceeded(removeRetentionPolicy);
                }
                finally
                {
                    AssertSucceeded(Run("remove-bucket-retention-policy", bucketLock.BucketName));
                    AssertSucceeded(Run("release-object-temporary-hold", bucketLock.BucketName, objectName));
                    AssertSucceeded(Run("release-object-event-based-hold", bucketLock.BucketName, objectName));
                    AssertSucceeded(Run("delete", bucketLock.BucketName, objectName));
                }
            }
        }

        [Fact]
        public void TestUniformBucketLevelAccess()
        {
            using (var uniformBucketLevelAccess = new BucketFixture())
            {
                var enableUniformBucketLevelAccess = Run("enable-uniform-bucket-level-access", uniformBucketLevelAccess.BucketName);
                AssertSucceeded(enableUniformBucketLevelAccess);
                var getUniformBucketLevelAccess = Run("get-uniform-bucket-level-access", uniformBucketLevelAccess.BucketName);
                AssertSucceeded(getUniformBucketLevelAccess);
                var disableUniformBucketLevelAccess = Run("disable-uniform-bucket-level-access", uniformBucketLevelAccess.BucketName);
                AssertSucceeded(disableUniformBucketLevelAccess);
                getUniformBucketLevelAccess = Run("get-uniform-bucket-level-access", uniformBucketLevelAccess.BucketName);
                AssertSucceeded(getUniformBucketLevelAccess);
            }
        }

        [Fact]
        public void TestBucketLifecycleManagement()
        {
            using (var bucketLifecyleManagement = new BucketFixture())
            {
                var enableBucketLifecycleManagement = Run("enable-bucket-lifecycle-management", bucketLifecyleManagement.BucketName);
                AssertSucceeded(enableBucketLifecycleManagement);
                Assert.Contains("Age: 100 Action: Delete", enableBucketLifecycleManagement.Stdout);
                var disableBucketLifecycleManagement = Run("enable-bucket-lifecycle-management", bucketLifecyleManagement.BucketName);
                AssertSucceeded(disableBucketLifecycleManagement);
            }
        }

        [Fact]
        public void TestGenerateEncryptionKey()
        {
            var output = Run("generate-encryption-key");
            AssertSucceeded(output);
        }

        [Fact]
        public void TestEncryptUploadAndDownload()
        {
            var output = Run("generate-encryption-key");
            AssertSucceeded(output);
            string key = output.Stdout.Trim();
            output = Run("upload", "-key", key, _bucketName,
                "Hello.txt", Collect("HelloEncryptUploadAndDownload.txt"));
            AssertSucceeded(output);
            // Downloading without the key should fail.
            output = Run("download", _bucketName, "HelloEncryptUploadAndDownload.txt");
            Assert.NotEqual(0, output.ExitCode);
            // Downloading with the key should yield the original file.
            output = Run("download", "-key", key, _bucketName, "HelloEncryptUploadAndDownload.txt",
                "Hello-downloaded.txt");
            AssertSucceeded(output);
            Assert.Equal(File.ReadAllText("Hello.txt"),
                File.ReadAllText("Hello-downloaded.txt"));
        }

        [Fact]
        public void TestEnableAndDisableRequesterPays()
        {
            try
            {
                var enabled = Run("enable-requester-pays", _bucketName);
                AssertSucceeded(enabled);
                Assert.Equal(1, Run("get-requester-pays", _bucketName).ExitCode);

                var disabled = Run("disable-requester-pays", _bucketName);
                AssertSucceeded(disabled);
                Assert.Equal(0, Run("get-requester-pays", _bucketName).ExitCode);

                enabled = Run("enable-requester-pays", _bucketName);
                AssertSucceeded(enabled);
                Assert.Equal(1, Run("get-requester-pays", _bucketName).ExitCode);
            }
            finally
            {
                Run("disable-requester-pays", _bucketName);
            }
        }

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/1060")]
        public void TestDownloadObjectRequesterPays()
        {
            try
            {
                var enabled = Run("enable-requester-pays", _bucketName);
                AssertSucceeded(enabled);

                var uploaded = Run("upload", _bucketName, "-pay",
                    "Hello.txt", Collect("HelloDownloadObjectRequesterPays.txt"));
                AssertSucceeded(uploaded);

                var downloaded = Run("download", _bucketName, "-pay",
                    "HelloDownloadObjectRequesterPays.txt", "HelloDownloadObjectRequesterPays2.txt");
                AssertSucceeded(downloaded);
                try
                {
                    Assert.Equal(File.ReadAllText("Hello.txt"),
                        File.ReadAllText("HelloDownloadObjectRequesterPays2.txt"));
                }
                finally
                {
                    File.Delete("HelloDownloadObjectRequesterPays2.txt");
                }
            }
            finally
            {
                Run("disable-requester-pays", _bucketName);
            }
        }

        private static string GetServiceAccountEmail()
        {
            var cred = GoogleCredential.GetApplicationDefault().UnderlyingCredential;
            switch (cred)
            {
                case ServiceAccountCredential sac:
                    return sac.Id;
                // TODO: We may well need to handle ComputeCredential for Kokoro.
                default:
                    throw new InvalidOperationException($"Unable to retrieve service account email address for credential type {cred.GetType()}");
            }
        }

        private static void DeleteAllHmacKeys(String projectId)
        {
            var client = StorageClient.Create();
            var key = client.ListHmacKeys(projectId);
            foreach (var metadata in key)
            {
                if (metadata.State == "ACTIVE")
                {
                    metadata.State = HmacKeyStates.Inactive;
                    client.UpdateHmacKey(metadata);
                }
                client.DeleteHmacKey(projectId, metadata.AccessId);
            }
        }
    }
}
