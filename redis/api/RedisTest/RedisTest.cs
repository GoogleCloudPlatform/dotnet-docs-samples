// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
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
            Command = "Redis.exe",
            Main = Redis.Main,
        };

        /// <summary>Runs RedisSample.exe with the provided arguments</summary>
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

    /// <summary>
    /// Trying to create a Redis instance for each test will exceed our rate limit.
    /// Therefore, share one Redis instance across all the tests.
    /// </summary>
    public class RedisFixture : IDisposable
    {
        public RedisFixture()
        {
            InstanceId = RedisTest.CreateRandomInstance();
        }
        public void Dispose()
        {
            RedisTest.DeleteInstance(InstanceId);
        }

        public string InstanceId { get; private set; }
    }

    public class RedisTest : BaseTest, IClassFixture<RedisFixture>
    {
        private readonly string _instanceId;

        private static readonly RetryRobot s_retryFailedAssertions = new RetryRobot()
        {
            RetryWhenExceptions = new[] { typeof(Xunit.Sdk.XunitException) },
            MaxTryCount = 6,
        };

        public RedisTest(RedisFixture fixture)
        {
            _instanceId = fixture.InstanceId;
        }

        /// <summary>
        /// Retry action.
        /// </summary>
        private static void Eventually(Action action) =>
            s_retryFailedAssertions.Eventually(action);

        public static string CreateRandomInstance()
        {
            string instanceId = TestUtil.RandomName();
            var created = Run("create", instanceId);
            AssertSucceeded(created);
            return instanceId;
        }

        internal static void DeleteInstance(string instanceId)
        {
            Eventually(() => AssertSucceeded(Run("delete", instanceId)));
        }

        [Fact]
        public void TestCreateInstance()
        {
            // Try creating another Redis instance with the same name. Should fail.
            var created_again = Run("create", _instanceId);
            Assert.Equal((int)Grpc.Core.StatusCode.AlreadyExists, created_again.ExitCode);

            // Try listing the instances. We should find the new one.
            Eventually(() =>
            {
                var listed = Run("list");
                AssertSucceeded(listed);
                Assert.Contains(_instanceId, listed.Stdout);
            });
        }

        [Fact]
        public void TestUpdateInstance()
        {
            //Update the Redis instance with given name. Should update successfully.
            var updated = Run("update", _instanceId);
            AssertSucceeded(updated);
            Assert.Contains("successfully updated", updated.Stdout);
        }

        [Fact]
        public void TestGetInstance()
        {
            //Get the Redis instance with given name. Should print instanceId.
            var consoleOutput = Run("get", _instanceId);
            AssertSucceeded(consoleOutput);
            Assert.Contains($"InstanceId:\t{_instanceId}", consoleOutput.Stdout);
        }
    }
}
