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

using Xunit;

[Collection(nameof(PubsubFixture))]
public class GetSchemaTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly GetSchemaSample _getSchemaSample;

    public GetSchemaTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _getSchemaSample = new GetSchemaSample();
    }

    [Fact]
    public void GetSchema()
    {
        string schemaId = _pubsubFixture.RandomSchemaId();

        _pubsubFixture.CreateAvroSchema(schemaId);
        _pubsubFixture.GetSchema(schemaId);
        var receivedSchema = _getSchemaSample.GetSchema(_pubsubFixture.ProjectId, schemaId);
        var schema = _pubsubFixture.GetSchema(schemaId);

        Assert.Equal(receivedSchema, schema);
    }
}
