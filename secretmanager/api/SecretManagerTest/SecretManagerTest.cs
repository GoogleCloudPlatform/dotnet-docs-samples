// Copyright(c) 2019 Google Inc.
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

using System.IO;
using Xunit;
using System;
using System.Linq;

namespace GoogleCloudSamples
{
    // <summary>
    /// Runs the sample app's methods and tests the outputs
    // </summary>
    public class CommonTests
    {
        private static readonly string s_projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        readonly CommandLineRunner _secretManager = new CommandLineRunner()
        {
            VoidMain = SecretManagerSample.Main,
            Command = "SecretManagerSample"
        };

        protected ConsoleOutput Run(params string[] args)
        {
            return _secretManager.Run(args);
        }

        private readonly RetryRobot _retryRobot = new RetryRobot()
        {
            RetryWhenExceptions = new[] { typeof(Xunit.Sdk.XunitException) }
        };

        private void Eventually(Action action) => _retryRobot.Eventually(action);

        [Fact]
        public void TestCreateAddAccessDelete()
        {
            string secretId = $"csharp-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            Run("create", $"projects/{s_projectId}", secretId);
            Run("add-version", $"projects/{s_projectId}/secrets/{secretId}");

            var accessVersionOut = Run("access-version", $"projects/{s_projectId}/secrets/{secretId}/versions/1");
            Assert.Contains("my super secret data", accessVersionOut.Stdout);

            Run("delete", $"projects/{s_projectId}/secrets/{secretId}");
        }

        [Fact]
        public void TestEnableDisableDestroy()
        {
            string secretId = $"csharp-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            Run("create", $"projects/{s_projectId}", secretId);
            Run("add-version", $"projects/{s_projectId}/secrets/{secretId}");

            var disableOut = Run("disable-version", $"projects/{s_projectId}/secrets/{secretId}/versions/1");
            Assert.Contains("Disabled secret version", disableOut.Stdout);

            var enableOut = Run("enable-version", $"projects/{s_projectId}/secrets/{secretId}/versions/1");
            Assert.Contains("Enabled secret version", enableOut.Stdout);

            var destroyOut = Run("destroy-version", $"projects/{s_projectId}/secrets/{secretId}/versions/1");
            Assert.Contains("Destroyed secret version", destroyOut.Stdout);

            Run("delete", $"projects/{s_projectId}/secrets/{secretId}");
        }

        [Fact]
        public void TestList()
        {
            string ts = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            string secret1Id = $"csharp-{ts}-1";
            string secret2Id = $"csharp-{ts}-2";
            string secret3Id = $"csharp-{ts}-3";


            Run("create", $"projects/{s_projectId}", secret1Id);
            Run("create", $"projects/{s_projectId}", secret2Id);
            Run("create", $"projects/{s_projectId}", secret3Id);

            Run("add-version", $"projects/{s_projectId}/secrets/{secret1Id}");
            Run("add-version", $"projects/{s_projectId}/secrets/{secret1Id}");
            Run("add-version", $"projects/{s_projectId}/secrets/{secret1Id}");

            var secretsOut = Run("list", $"projects/{s_projectId}");
            Assert.Contains($"{ts}", secretsOut.Stdout);

            var versionsOut = Run("list-versions", $"projects/{s_projectId}/secrets/{secret1Id}");
            Assert.Contains($"{secret1Id}/versions/1", versionsOut.Stdout);

            Run("delete", $"projects/{s_projectId}/secrets/{secret1Id}");
            Run("delete", $"projects/{s_projectId}/secrets/{secret2Id}");
            Run("delete", $"projects/{s_projectId}/secrets/{secret3Id}");
        }
    }

    public class QuickStartTests
    {
        readonly CommandLineRunner _quickStart = new CommandLineRunner()
        {
            VoidMain = QuickStart.Main,
            Command = "QuickStart"
        };

        [Fact]
        public void TestRun()
        {
            var output = _quickStart.Run();
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("secrets/", output.Stdout);
        }
    }
}
