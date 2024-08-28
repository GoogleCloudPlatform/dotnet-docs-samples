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
using System.Threading.Tasks;
using Xunit;

/// <summary>
/// Tests creating a backup using MR CMEK.
/// </summary>
[Collection(nameof(SpannerFixture))]
public class CreateBackupWithMRCMEKAsyncTest
{
    private readonly SpannerFixture _fixture;

    public CreateBackupWithMRCMEKAsyncTest(SpannerFixture fixture)
    {
        _fixture = fixture;
    }

    [SkippableFact]
    public async Task TestCreatBackupWithMRCMEKAsync()
    {
        Skip.If(!_fixture.RunCmekBackupSampleTests, SpannerFixture.SkipCmekBackupSamplesMessage);
        // Create a backup with custom encryption keys.
        var sample = new CreateBackupWithMRCMEKAsyncSample();
        var backup = await sample.CreateBackupWithMRCMEKAsync(_fixture.ProjectId, _fixture.InstanceId, _fixture.FixedEncryptedDatabaseId, _fixture.EncryptedBackupId, _fixture.KmsKeyNames);
        Assert.Equal(_fixture.KmsKeyNames.Length, backup.EncryptionInfo.KmsKeyVersionsAsCryptoKeyVersionNames.Length);
        foreach (CryptoKeyVersionName KmsKeyVersion in backup.EncryptionInfo.KmsKeyVersionsAsCryptoKeyVersionNames)
        {
          Assert.True(_fixture.KmsKeyNames.contains(KmsKeyVersion.CryptoKeyId));
        }
    }
}