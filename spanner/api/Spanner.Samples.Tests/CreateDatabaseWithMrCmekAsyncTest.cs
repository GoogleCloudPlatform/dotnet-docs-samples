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

/// <summary>
/// Tests creating a databases using MR CMEK.
/// </summary>
[Collection(nameof(SpannerFixture))]
public class CreateDatabaseWithMrCmekAsyncTest
{
    private readonly SpannerFixture _fixture;

    public CreateDatabaseWithMrCmekAsyncTest(SpannerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TestCreateDatabaseWithMrCmekAsync()
    {
        // Create a database with custom encryption keys.
        var sample = new CreateDatabaseWithMrCmekAsyncSample();
        var database = await sample.CreateDatabaseWithMrCmekAsync(_fixture.ProjectId, _fixture.InstanceId, _fixture.MrCmekDatabaseId, _fixture.KmsKeyNames);
        Assert.All(database.EncryptionConfig.KmsKeyNames, keyName => _fixture.KmsKeyNames.Contains(CryptoKeyName.Parse(keyName)));
    }
}
