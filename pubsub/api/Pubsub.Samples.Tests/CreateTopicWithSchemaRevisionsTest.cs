// Copyright 2024 Google Inc.
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
public class CreateTopicWithSchemaRevisionsTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CommitAvroSchemaSample _commitAvroSchemaSample;
    private readonly CreateTopicWithSchemaRevisionsSample _createTopicWithSchemaRevisionsSample;

    public CreateTopicWithSchemaRevisionsTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _commitAvroSchemaSample = new CommitAvroSchemaSample();
        _createTopicWithSchemaRevisionsSample = new CreateTopicWithSchemaRevisionsSample();
    }

    [Fact]
    public void CreateTopicWithSchemaRevisions()
    {
        string schemaId = _pubsubFixture.RandomSchemaId();
        Schema initialSchema = _pubsubFixture.CreateAvroSchema(schemaId);
        Schema amendedSchema = _commitAvroSchemaSample.CommitAvroSchema(_pubsubFixture.ProjectId, schemaId, _pubsubFixture.NewAvroSchemaFile);

        string topicId = _pubsubFixture.RandomTopicId();
        var newlyCreatedTopic = _createTopicWithSchemaRevisionsSample.CreateTopicWithSchemaRevisions(
            _pubsubFixture.ProjectId, topicId, schemaId, initialSchema.RevisionId, amendedSchema.RevisionId, Encoding.Json);
        _pubsubFixture.TempTopicIds.Add(topicId);
        var topic = _pubsubFixture.GetTopic(topicId);
        Assert.Equal(newlyCreatedTopic, topic);
    }
}
