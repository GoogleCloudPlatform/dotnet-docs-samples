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
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Compute.Samples.Tests
{
    [CollectionDefinition(nameof(ComputeFixture))]
    public class ComputeFixture : IDisposable, ICollectionFixture<ComputeFixture>
    {
        public string ProjectId { get; }

        public string Zone { get => "us-central1-a"; }

        public string MachineType { get => "n1-standard-1"; }

        public string DiskImage { get => "projects/debian-cloud/global/images/family/debian-10"; }

        public string DiskSizeGb { get => "10"; }

        public string UsageReportBucketName { get; } = GenerateName("b");

        public string UsageReportPrefix { get; } = "test-usage";

        private IList<string> MachinesToDelete { get; } = new List<string>();

        public InstancesClient InstancesClient { get; } = InstancesClient.Create();

        public ProjectsClient ProjectsClient { get; } = ProjectsClient.Create();

        private StorageClient StorageClient { get; } = StorageClient.Create();

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

        private static string GenerateName(string prefix) =>
            $"{prefix}-{DateTime.UtcNow.ToString("yyyyMMdd-HHmmssfff", CultureInfo.InvariantCulture)}";

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
