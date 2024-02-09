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

using Xunit;

[Collection(nameof(PubsubFixture))]
public class CommitProtoSchemaTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CommitProtoSchemaSample _commitProtoSchemaSample;

    public CommitProtoSchemaTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _commitProtoSchemaSample = new CommitProtoSchemaSample();
    }

    [Fact]
    public void CommitProtoSchema()
    {
        string schemaId = _pubsubFixture.RandomSchemaId();
        var initialSchema = _pubsubFixture.CreateProtoSchema(schemaId);
        var updatedSchema = _commitProtoSchemaSample.CommitProtoSchema(_pubsubFixture.ProjectId, schemaId, _pubsubFixture.NewProtoSchemaFile);
        Assert.NotEqual(initialSchema.Definition, updatedSchema.Definition);
        Assert.NotEqual(initialSchema.RevisionId, updatedSchema.RevisionId);

        var allRevisions = _pubsubFixture.ListSchemaRevisions(schemaId);
        Assert.Equal(2, allRevisions.Count);
    }
}
