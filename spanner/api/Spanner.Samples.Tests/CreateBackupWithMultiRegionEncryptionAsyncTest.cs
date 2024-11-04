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
using System.Linq;
using System.Threading.Tasks;
using Xunit;

/// <summary>
/// Tests creating a backup using MR CMEK.
/// </summary>
[Collection(nameof(SpannerFixture))]
public class CreateBackupWithMultiRegionEncryptionAsyncTest
{
    private readonly SpannerFixture _fixture;

    public CreateBackupWithMultiRegionEncryptionAsyncTest(SpannerFixture fixture)
    {
        _fixture = fixture;
    }

    [SkippableFact]
    public async Task TestCreateBackupWithMultiRegionEncryptionAsync()
    {
        Skip.If(_fixture.SkipCmekBackupSampleTests, SpannerFixture.SkipCmekBackupSamplesMessage);
        // Create a backup with custom encryption keys.
        var sample = new CreateBackupWithMultiRegionEncryptionAsyncSample();
        var backup = await sample.CreateBackupWithMultiRegionEncryptionAsync(_fixture.ProjectId, _fixture.InstanceId, _fixture.FixedMultiRegionEncryptedBackupId, _fixture.MultiRegionEncryptedBackupId, _fixture.KmsKeyNames);
        Assert.All(backup.EncryptionInformation, encryptionInfo => _fixture.KmsKeyNames.Any(keyName => encryptionInfo.KmsKeyVersion.StartsWith(keyName.ToString())));
    }
}
