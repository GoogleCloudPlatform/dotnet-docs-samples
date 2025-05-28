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
public class CreateTopicWithKinesisIngestionTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreateTopicWithKinesisIngestionSample _createTopicWithKinesisIngestionSample;

    public CreateTopicWithKinesisIngestionTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createTopicWithKinesisIngestionSample = new CreateTopicWithKinesisIngestionSample();
    }

    [Fact]
    public void CreateTopicWithKinesisIngestion()
    {
        string topicId = _pubsubFixture.RandomTopicId();
        var (streamArn, consumerArn, awsRoleArn, gcpServiceAccount) = _pubsubFixture.KinesisIngestionParams();
        var exception = Assert.Throws<RpcException>(() => _createTopicWithKinesisIngestionSample.CreateTopicWithKinesisIngestion(_pubsubFixture.ProjectId, topicId, streamArn, consumerArn, awsRoleArn, gcpServiceAccount));
        Assert.Equal(StatusCode.PermissionDenied, exception.Status.StatusCode);
        Assert.Equal(_pubsubFixture.PermissionDeniedMessage, exception.Status.Detail);
    }
}
