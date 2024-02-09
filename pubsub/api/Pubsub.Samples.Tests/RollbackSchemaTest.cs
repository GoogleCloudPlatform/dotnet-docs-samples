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
public class RollbackSchemaTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CommitAvroSchemaSample _commitAvroSchemaSample;
    private readonly RollbackSchemaSample _rollbackSchemaSample;

    public RollbackSchemaTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _commitAvroSchemaSample = new CommitAvroSchemaSample();
        _rollbackSchemaSample = new RollbackSchemaSample();
    }

    [Fact]
    public void RollbackSchema()
    {
        string schemaId = _pubsubFixture.RandomSchemaId();
        var initialSchema = _pubsubFixture.CreateAvroSchema(schemaId);
        var updatedSchema = _commitAvroSchemaSample.CommitAvroSchema(_pubsubFixture.ProjectId, schemaId, _pubsubFixture.NewAvroSchemaFile);

        var beforeRollback = _pubsubFixture.GetSchema(schemaId);
        Assert.Equal(updatedSchema.Definition, beforeRollback.Definition);

        _rollbackSchemaSample.RollbackSchema(_pubsubFixture.ProjectId, schemaId, initialSchema.RevisionId);

        // The post-rollback revision ID won't be the same as the initial schema, but the definition will be.
        var afterRollback = _pubsubFixture.GetSchema(schemaId);
        Assert.Equal(initialSchema.Definition, afterRollback.Definition);
    }
}
