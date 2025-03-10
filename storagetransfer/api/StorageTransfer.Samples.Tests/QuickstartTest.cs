/**
 * Copyright 2021 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Google.Cloud.StorageTransfer.V1;
using System;
using Xunit;

namespace StorageTransfer.Samples.Tests
{
    [Collection(nameof(StorageFixture))]
    public class QuickstartTest : IDisposable
    {
        private readonly StorageFixture _fixture;
        private string _transferJobName;

        public QuickstartTest(StorageFixture fixture)
        {
            _fixture = fixture;
            _fixture.BucketNameSource = _fixture.GenerateBucketName();
            _fixture.BucketNameSink = _fixture.GenerateBucketName();
            _fixture.CreateBucketAndGrantStsPermissions(_fixture.BucketNameSource);
            _fixture.CreateBucketAndGrantStsPermissions(_fixture.BucketNameSink);
        }

        [Fact]
        public void TestQuickstart()
        {
            QuickstartSample quickstartSample = new QuickstartSample();
            var transferJob = quickstartSample.Quickstart(_fixture.ProjectId, _fixture.BucketNameSource, _fixture.BucketNameSink);
            Assert.Contains("transferJobs/", transferJob.Name);
            _transferJobName = transferJob.Name;
        }

        public void Dispose()
        {
            try
            {
                _fixture.Sts.UpdateTransferJob(new UpdateTransferJobRequest()
                {
                    ProjectId = _fixture.ProjectId,
                    JobName = _transferJobName,
                    TransferJob = new TransferJob()
                    {
                        Name = _transferJobName,
                        Status = TransferJob.Types.Status.Deleted
                    }
                });
            }
            catch (Exception)
            {
                // Do nothing, we delete on a best effort basis.
            }
        }
    }
}
