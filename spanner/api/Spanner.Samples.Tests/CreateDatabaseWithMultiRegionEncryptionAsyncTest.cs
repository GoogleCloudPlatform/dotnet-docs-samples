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

using System.Threading.Tasks;
using Xunit;
using Google.Cloud.Spanner.Admin.Database.V1;
using System.Linq;

/// <summary>
/// Tests creating a databases using MR CMEK.
/// </summary>
[Collection(nameof(SpannerFixture))]
public class CreateDatabaseWithMultiRegionEncryptionAsyncTest
{
    private readonly SpannerFixture _fixture;

    public CreateDatabaseWithMultiRegionEncryptionAsyncTest(SpannerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TestCreateDatabaseWithMultiRegionEncryptionAsync()
    {
        // Create a database with custom encryption keys.
        var sample = new CreateDatabaseWithMultiRegionEncryptionAsyncSample();
        var database = await sample.CreateDatabaseWithMultiRegionEncryptionAsync(_fixture.ProjectId, _fixture.InstanceId, _fixture.MultiRegionEncryptedDatabaseId, _fixture.KmsKeyNames);
        Assert.All(database.EncryptionConfig.KmsKeyNamesAsCryptoKeyNames, keyName => _fixture.KmsKeyNames.Contains(keyName));
    }
}
