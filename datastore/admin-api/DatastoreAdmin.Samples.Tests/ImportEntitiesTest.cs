// Copyright 2020 Google Inc.
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

[Collection(nameof(DatastoreAdminFixture))]
public class ImportEntitiesTest
{
    private readonly DatastoreAdminFixture _datastoreAdminFixture;

    public ImportEntitiesTest(DatastoreAdminFixture datastoreAdminFixture)
    {
        _datastoreAdminFixture = datastoreAdminFixture;
    }

    [Fact]
    public void TestImportEntities()
    {
        ExportEntitiesSample exportEntitiesSample = new ExportEntitiesSample();
        ImportEntitiesSample importEntitiesSample = new ImportEntitiesSample();
        var outputUrl = exportEntitiesSample.ExportEntities(_datastoreAdminFixture.ProjectId, $"gs://{_datastoreAdminFixture.BucketName}", _datastoreAdminFixture.Kind, _datastoreAdminFixture.Namespace);
        var isCompleted = importEntitiesSample.ImportEntities(_datastoreAdminFixture.ProjectId, outputUrl, _datastoreAdminFixture.Kind, _datastoreAdminFixture.Namespace);
        Assert.True(isCompleted);
    }
}
