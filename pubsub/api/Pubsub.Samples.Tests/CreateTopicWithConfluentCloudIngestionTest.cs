// Copyright 2025 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Grpc.Core;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class CreateTopicWithConfluentCloudIngestionTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreateTopicWithConfluentCloudIngestionSample _createTopicWithConfluentCloudIngestionSample;

    public CreateTopicWithConfluentCloudIngestionTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createTopicWithConfluentCloudIngestionSample = new CreateTopicWithConfluentCloudIngestionSample();
    }

    [Fact]
    public void CreateTopicWithConfluentCloudIngestion()
    {
        string topicId = _pubsubFixture.RandomTopicId();
        var (bootstrapServer, clusterId, confluentTopic, identityPoolId, gcpServiceAccount) = _pubsubFixture.ConfluentCloudIngestionParams();
        var exception = Assert.Throws<RpcException>(() => _createTopicWithConfluentCloudIngestionSample.CreateTopicWithConfluentCloudIngestion(_pubsubFixture.ProjectId, topicId, bootstrapServer, clusterId, confluentTopic, identityPoolId, gcpServiceAccount));
        Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
        Assert.Equal("Cloud Pub/Sub received invalid argument/s for the ingestion data source: Unreachable bootstrap server.", exception.Status.Detail);
    }
}
