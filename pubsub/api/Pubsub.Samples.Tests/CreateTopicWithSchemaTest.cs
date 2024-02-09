// Copyright 2021 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Cloud.PubSub.V1;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class CreateTopicWithSchemaTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreateTopicWithSchemaSample _createTopicWithSchemaSample;

    public CreateTopicWithSchemaTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createTopicWithSchemaSample = new CreateTopicWithSchemaSample();
    }

    [Fact]
    public void CreateTopicWithSchema()
    {
        string schemaId = _pubsubFixture.RandomSchemaId();
        _pubsubFixture.CreateAvroSchema(schemaId);

        string topicId = _pubsubFixture.RandomTopicId();
        var newlyCreatedTopic = _createTopicWithSchemaSample.CreateTopicWithSchema(_pubsubFixture.ProjectId, topicId, schemaId, Encoding.Json);
        _pubsubFixture.TempTopicIds.Add(topicId);
        var topic = _pubsubFixture.GetTopic(topicId);
        Assert.Equal(newlyCreatedTopic, topic);
    }
}
