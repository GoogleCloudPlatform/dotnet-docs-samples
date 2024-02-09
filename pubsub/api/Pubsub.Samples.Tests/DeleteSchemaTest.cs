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

using System;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class DeleteSchemaTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly DeleteSchemaSample _deleteSchemaSample;

    public DeleteSchemaTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _deleteSchemaSample = new DeleteSchemaSample();
    }

    [Fact]
    public void DeleteSchema()
    {
        string schemaId = _pubsubFixture.RandomSchemaId();

        _pubsubFixture.CreateAvroSchema(schemaId);
        _deleteSchemaSample.DeleteSchema(_pubsubFixture.ProjectId, schemaId);

        Exception ex = Assert.Throws<Grpc.Core.RpcException>(() => _pubsubFixture.GetSchema(schemaId));

        _pubsubFixture.TempSchemaIds.Remove(schemaId);  // We already deleted it.
    }
}
