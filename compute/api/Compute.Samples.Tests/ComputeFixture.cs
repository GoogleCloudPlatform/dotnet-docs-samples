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

        private IList<string> MachinesToDelete { get; } = new List<string>();

        public InstancesClient Client { get; } = InstancesClient.Create();

        public ComputeFixture()
        {
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            if (string.IsNullOrEmpty(ProjectId))
            {
                throw new Exception(
                    "Please set the environment variable GOOGLE_PROJECT_ID to the ID of the project you want to run these tests in.");
            }
        }

        public string GenerateMachineName()
        {
            string name = $"i-{DateTime.UtcNow.ToString("yyyyMMdd-HHmmssfff", CultureInfo.InvariantCulture)}";
            // We always add them to the deletion list. We delete on a best effort basis anyway.
            MachinesToDelete.Add(name);
            return name;
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if(_disposed)
            {
                return;
            }

            foreach(var machineName in MachinesToDelete)
            {
                try
                {
                    // We don't poll, we delete on a best effort basis.
                    Client.Delete(ProjectId, Zone, machineName);
                }
                catch
                { 
                    // Do nothing, we delete on a best effort basis.
                }
            }
            _disposed = true;
        }
    }
}
