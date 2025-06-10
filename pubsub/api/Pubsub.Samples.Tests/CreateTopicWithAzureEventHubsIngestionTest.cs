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
public class CreateTopicWithAzureEventHubsIngestionTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreateTopicWithAzureEventHubsIngestionSample _createTopicWithAzureEventHubsIngestionSample;

    public CreateTopicWithAzureEventHubsIngestionTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createTopicWithAzureEventHubsIngestionSample = new CreateTopicWithAzureEventHubsIngestionSample();
    }

    [Fact]
    public void CreateTopicWithAzureEventHubsIngestion()
    {
        string topicId = _pubsubFixture.RandomTopicId();
        var (resourceGroup, nameSpace, eventHub, clientId, tenantId, subscriptionId, gcpServiceAccount) = _pubsubFixture.AzureEventHubsIngestionParams();
        var exception = Assert.Throws<RpcException>(() => _createTopicWithAzureEventHubsIngestionSample.CreateTopicWithAzureEventHubsIngestion(_pubsubFixture.ProjectId, topicId, resourceGroup, nameSpace, eventHub, clientId, tenantId, subscriptionId, gcpServiceAccount));
        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
        Assert.Equal($"Cloud Pub/Sub encountered a not-found error while trying to connect to the ingestion data source: Failed to resolve bootstrap server {nameSpace}.servicebus.windows.net to an IP.", exception.Status.Detail);
    }
}
