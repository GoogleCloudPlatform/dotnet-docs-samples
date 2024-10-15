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

using Google.Cloud.Spanner.Admin.Database.V1;
using System;
using System.Threading.Tasks;
using Xunit;

/// <summary>
/// Tests restoring a databases using customer managed encryption.
/// </summary>
[Collection(nameof(SpannerFixture))]
public class RestoreDatabaseWithMrCmekAsyncTest
{
    private readonly SpannerFixture _fixture;

    public RestoreDatabaseWithMrCmekAsyncTest(SpannerFixture fixture)
    {
        _fixture = fixture;
    }

    [SkippableFact]
    public async Task TestRestoreDatabaseWithMrCmekAsync()
    {
        Skip.If(!_fixture.RunCmekBackupSampleTests, SpannerFixture.SkipCmekBackupSamplesMessage);
        var sample = new RestoreDatabaseWithMrCmekAsyncSample();
        var database = await sample.RestoreDatabaseWithMrCmekAsync(_fixture.ProjectId, _fixture.InstanceId, _fixture.MrCmekRestoreDatabaseId, _fixture.FixedMrCmekBackupId, _fixture.KmsKeyNames);
        Assert.All(database.EncryptionConfig.KmsKeyNames, keyName => _fixture.KmsKeyNames.Contains(CryptoKeyName.Parse(keyName)));
    }
}
