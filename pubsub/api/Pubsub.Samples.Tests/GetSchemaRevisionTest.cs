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
public class GetSchemaRevisionTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CommitAvroSchemaSample _commitAvroSchemaSample;
    private readonly GetSchemaRevisionSample _getSchemaRevisionSample;

    public GetSchemaRevisionTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _commitAvroSchemaSample = new CommitAvroSchemaSample();
        _getSchemaRevisionSample = new GetSchemaRevisionSample();
    }

    [Fact]
    public void GetSchemaRevision()
    {
        string schemaId = _pubsubFixture.RandomSchemaId();

        var initialSchema = _pubsubFixture.CreateAvroSchema(schemaId);
        var updatedSchema = _commitAvroSchemaSample.CommitAvroSchema(_pubsubFixture.ProjectId, schemaId, _pubsubFixture.NewAvroSchemaFile);
        var initialSchemaRefetched = _getSchemaRevisionSample.GetSchemaRevision(_pubsubFixture.ProjectId, schemaId, initialSchema.RevisionId);
        var updatedSchemaRefetched = _getSchemaRevisionSample.GetSchemaRevision(_pubsubFixture.ProjectId, schemaId, updatedSchema.RevisionId);

        // We can't check the fully schemas against each other directly as the name includes the revision ID for the "refetched"
        // ones, but we can check the revision IDs.
        Assert.Equal(initialSchema.RevisionId, initialSchemaRefetched.RevisionId);
        Assert.Equal(updatedSchemaRefetched.RevisionId, updatedSchemaRefetched.RevisionId);
    }
}
