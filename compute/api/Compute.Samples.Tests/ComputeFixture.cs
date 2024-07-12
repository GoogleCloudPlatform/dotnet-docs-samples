/*
 * Copyright 2021 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Google.Cloud.Compute.V1;
using Google.Cloud.Storage.V1;
using GoogleCloudSamples;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;
using Xunit.Sdk;

namespace Compute.Samples.Tests
{
    [CollectionDefinition(nameof(ComputeFixture))]
    public class ComputeFixture : IDisposable, ICollectionFixture<ComputeFixture>
    {
        public string ProjectId { get; }

        public string Zone => "us-central1-a";

        public string MachineType => "n1-standard-1";

        public string DiskImage => "projects/debian-cloud/global/images/family/debian-12";

        public long DiskSizeGb => 10;

        public string NetworkName => "default";

        public string NetworkResourceUri => $"global/networks/{NetworkName}";

        public string UsageReportBucketName { get; } = GenerateName("b");

        public string UsageReportPrefix => "test-usage";

        public string PublicImagesProjectId => "windows-sql-cloud";

        private IList<string> MachinesToDelete { get; } = new List<string>();

        private IList<string> FirewallRulesToDelete { get; } = new List<string>();

        public InstancesClient InstancesClient { get; } = InstancesClient.Create();

        public FirewallsClient FirewallsClient { get; } = FirewallsClient.Create();

        public ProjectsClient ProjectsClient { get; } = ProjectsClient.Create();

        private StorageClient StorageClient { get; } = StorageClient.Create();

        public RetryRobot AssertConcurrently { get; } = new RetryRobot
        { 
            RetryWhenExceptions = new Type[] { typeof(XunitException) },
            // We wait a little longer than default to let the concurrent test execution
            // finish before reattempting.
            FirstRetryDelayMs = 30_000,
            MaxTryCount = 15
        };

        // Because of internal policies for firewall creation.
        public RetryRobot FirewallRuleCreated { get; } = new RetryRobot
        {
            ShouldRetry = (ex) => ex is RpcException rpcEx 
                && (rpcEx.StatusCode == StatusCode.NotFound || rpcEx.Message.Contains("resourceNotReady"))
        };

        public RetryRobot FirewallRuleReady { get; } = new RetryRobot
        {
            ShouldRetry = (ex) => ex is RpcException rpcEx
                && (rpcEx.StatusCode == StatusCode.InvalidArgument || rpcEx.Message.Contains("is not ready"))
        };

        public ComputeFixture()
        {
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            if (string.IsNullOrEmpty(ProjectId))
            {
                throw new Exception(
                    "Please set the environment variable GOOGLE_PROJECT_ID to the ID of the project you want to run these tests in.");
            }

            StorageClient.CreateBucket(ProjectId, UsageReportBucketName);
        }

        public string GenerateMachineName()
        {
            string name = GenerateName("i");
            // We always add them to the deletion list. We delete on a best effort basis anyway.
            MachinesToDelete.Add(name);
            return name;
        }

        public string GenerateFirewallRuleName()
        {
            string name = GenerateName("fr");
            // We always add them to the deletion list. We delete on a best effort basis anyway.
            FirewallRulesToDelete.Add(name);
            return name;
        }

        private static string GenerateName(string prefix) =>
            $"{prefix}-{DateTime.UtcNow.ToString("yyyyMMdd-HHmmssfffffff", CultureInfo.InvariantCulture)}";

        private bool _disposed = false;
        public void Dispose()
        {
            if(_disposed)
            {
                return;
            }

            BestEffortCleanup(() => StorageClient.DeleteBucket(UsageReportBucketName, new DeleteBucketOptions { DeleteObjects = true }));

            foreach(var machineName in MachinesToDelete)
            {
                // We don't poll, we delete on a best effort basis.
                BestEffortCleanup(() => InstancesClient.Delete(ProjectId, Zone, machineName));
            }

            foreach(var firewallRule in FirewallRulesToDelete)
            {
                // We don't poll, we delete on a best effort basis.
                BestEffortCleanup(() => FirewallsClient.Delete(ProjectId, firewallRule));
            }

            BestEffortCleanup(() =>
            {
                // We reset the UsageExportBucket on cleanup, so we don't have to do it
                // on each test. This is not infalible as someone may have a set export
                // bucket and this will just remove the any bucket, but doing on each
                // tests is also not infalible.
                // We should have a separate dedicated project for these tests, and still
                // some flakiness may occure because different runs of the tests may happen
                // at the same time.
                ProjectsClient.SetUsageExportBucket(ProjectId, new UsageExportLocation());
            });

            _disposed = true;

            static void BestEffortCleanup(Action cleanupAction)
            {
                try
                {
                    cleanupAction();
                }
                catch
                {
                    // Do nothing, we delete on a best effort basis.
                }
            }
        }
    }
}
