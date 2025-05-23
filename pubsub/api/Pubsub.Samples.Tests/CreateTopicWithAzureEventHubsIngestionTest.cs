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

using Google.Cloud.PubSub.V1;
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
        var (resourceGroup, nameSpace, eventHub, clientId, tenantId, subscriptionId, gcpServiceAccount) = _pubsubFixture.RandomAzureEventHubsIngestionParams();
        Topic createdTopic = _createTopicWithAzureEventHubsIngestionSample.CreateTopicWithAzureEventHubsIngestion(_pubsubFixture.ProjectId, topicId, resourceGroup, nameSpace, eventHub, clientId, tenantId, subscriptionId, gcpServiceAccount);

        // Confirm that the created topic and topic retrieved by ID are equal
        Topic retrievedTopic = _pubsubFixture.GetTopic(topicId);
        Assert.Equal(createdTopic, retrievedTopic);

        // Confirm that all Ingestion params are equal to expected values
        Assert.Equal(resourceGroup, createdTopic.IngestionDataSourceSettings.AzureEventHubs.ResourceGroup);
        Assert.Equal(nameSpace, createdTopic.IngestionDataSourceSettings.AzureEventHubs.Namespace);
        Assert.Equal(eventHub, createdTopic.IngestionDataSourceSettings.AzureEventHubs.EventHub);
        Assert.Equal(clientId, createdTopic.IngestionDataSourceSettings.AzureEventHubs.ClientId);
        Assert.Equal(tenantId, createdTopic.IngestionDataSourceSettings.AzureEventHubs.TenantId);
        Assert.Equal(subscriptionId, createdTopic.IngestionDataSourceSettings.AzureEventHubs.SubscriptionId);
        Assert.Equal(gcpServiceAccount, createdTopic.IngestionDataSourceSettings.AzureEventHubs.GcpServiceAccount);
    }
}
