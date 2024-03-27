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

using System.Linq;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class ListSchemaRevisionsTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CommitAvroSchemaSample _commitAvroSchemaSample;
    private readonly ListSchemaRevisionsSample _listSchemaRevisionsSample;

    public ListSchemaRevisionsTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _commitAvroSchemaSample = new CommitAvroSchemaSample();
        _listSchemaRevisionsSample = new ListSchemaRevisionsSample();
    }

    [Fact]
    public void ListSchemaRevisions()
    {
        string schemaId = _pubsubFixture.RandomSchemaId();
        var initialSchema = _pubsubFixture.CreateAvroSchema(schemaId);
        var updatedSchema = _commitAvroSchemaSample.CommitAvroSchema(_pubsubFixture.ProjectId, schemaId, _pubsubFixture.NewAvroSchemaFile);

        var allRevisions = _listSchemaRevisionsSample.ListSchemaRevisions(_pubsubFixture.ProjectId, schemaId);
        Assert.Equal(
            new[] { initialSchema.RevisionId, updatedSchema.RevisionId },
            allRevisions.OrderBy(rev => rev.RevisionCreateTime).Select(rev => rev.RevisionId).ToArray());
    }
}
