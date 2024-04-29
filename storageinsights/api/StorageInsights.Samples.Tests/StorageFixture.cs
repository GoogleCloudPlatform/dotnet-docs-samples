// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Api.Gax.ResourceNames;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.ResourceManager.V3;
using Google.Cloud.Storage.V1;
using Google.Cloud.StorageInsights.V1;
using System;
using System.Collections.Generic;
using Xunit;

namespace StorageInsights.Samples.Tests
{
    [CollectionDefinition(nameof(StorageFixture))]
    public class StorageFixture : IDisposable, ICollectionFixture<StorageFixture>
    {
        public string ProjectId { get; }
        public string BucketNameSource { get; } = Guid.NewGuid().ToString();
        public string BucketNameSink { get; } = Guid.NewGuid().ToString();
        public string BucketLocation => "us-west1";
        public StorageClient Storage { get; } = StorageClient.Create();
        public StorageInsightsClient StorageInsights { get; } = StorageInsightsClient.Create();

        public StorageFixture()
        {
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            if (string.IsNullOrWhiteSpace(ProjectId))
            {
                throw new Exception("You need to set the Environment variable 'GOOGLE_PROJECT_ID' with your Google Cloud Project's project id.");
            }

            ProjectsClient projectsClient = ProjectsClient.Create();
            Project project = projectsClient.GetProject(ProjectName.Format(ProjectId));
            string projectNumber = project.Name.Split("/")[1];
            String insightsServiceAccount = "service-" + projectNumber + "@gcp-sa-storageinsights.iam.gserviceaccount.com";

            CreateBucketAndGrantInsightsPermissions(BucketNameSource, insightsServiceAccount);
            CreateBucketAndGrantInsightsPermissions(BucketNameSink, insightsServiceAccount);
        }


        private void CreateBucketAndGrantInsightsPermissions(string bucketName, string email)
        {

            Storage.CreateBucket(ProjectId, new Bucket
            {
                Name = bucketName,
                Location = BucketLocation
            });

            string member = "serviceAccount:" + email;
            string insightsCollectorService = "roles/storage.insightsCollectorService";
            string objectCreator = "roles/storage.objectCreator";

            var policy = Storage.GetBucketIamPolicy(bucketName, new GetBucketIamPolicyOptions
            {
                RequestedPolicyVersion = 3
            });
            // Set the policy schema version. For more information, please refer to https://cloud.google.com/iam/docs/policies#versions.
            policy.Version = 3;

            Policy.BindingsData insightsCollectorServiceBinding = new Policy.BindingsData
            {
                Role = insightsCollectorService,
                Members = new List<string> { member }
            };
            Policy.BindingsData objectCreatorBinding = new Policy.BindingsData
            {
                Role = objectCreator,
                Members = new List<string> { member }
            };

            policy.Bindings.Add(insightsCollectorServiceBinding);
            policy.Bindings.Add(objectCreatorBinding);
            Storage.SetBucketIamPolicy(bucketName, policy);
        }

        public void Dispose()
        {
            try
            {
                Storage.DeleteBucket(BucketNameSink);
            }
            catch (Exception)
            {
                // Do nothing, we delete on a best effort basis.
            }
            try
            {
                Storage.DeleteBucket(BucketNameSource);
            }
            catch (Exception)
            {
                // Do nothing, we delete on a best effort basis.
            }
        }
    }
}
