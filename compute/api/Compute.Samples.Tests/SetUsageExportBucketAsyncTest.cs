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

using System.Threading.Tasks;
using Xunit;

namespace Compute.Samples.Tests
{
    [Collection(nameof(ComputeFixture))]
    public class SetUsageExportBucketAsyncTest
    {
        private readonly ComputeFixture _fixture;
        private readonly SetUsageExportBucketAsyncSample _sample = new SetUsageExportBucketAsyncSample();

        public SetUsageExportBucketAsyncTest(ComputeFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task SetsUsageExportBucket()
        {
            await _fixture.AssertConcurrently.Eventually(async () =>
            {
                await _sample.SetUsageExportBucketAsync(_fixture.ProjectId, _fixture.UsageReportBucketName, _fixture.UsageReportPrefix);

                var project = await _fixture.ProjectsClient.GetAsync(_fixture.ProjectId);
                var newUsageExportLocation = project.UsageExportLocation;

                Assert.NotNull(newUsageExportLocation);
                Assert.Equal(_fixture.UsageReportBucketName, newUsageExportLocation.BucketName);
                Assert.Equal(_fixture.UsageReportPrefix, newUsageExportLocation.ReportNamePrefix);
            });
        }
    }
}
