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

using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Linq;
using Xunit;

[Collection(nameof(BucketFixture))]
public class HmacKeyTest
{
    private readonly BucketFixture _bucketFixture;

    public HmacKeyTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void HmacKeySampleTest()
    {
        CreateHmacKeySample createHmacKeySample = new CreateHmacKeySample();
        ListHmacKeysSample listHmacKeysSample = new ListHmacKeysSample();
        HmacKeyGetSample hmacKeyGetSample = new HmacKeyGetSample();
        HmacKeyDeactivateSample hmacKeyDeactivateSample = new HmacKeyDeactivateSample();
        HmacKeyActivateSample hmacKeyActivateSample = new HmacKeyActivateSample();
        HmacKeyDeleteSample hmacKeyDeleteSample = new HmacKeyDeleteSample();

        //These need to all run as one test so that we can use the created key in every test
        DeleteAllHmacKeys(_bucketFixture.ProjectId);

        string serviceAccountEmail = GetServiceAccountEmail();

        //create key
        var key = createHmacKeySample.CreateHmacKey(_bucketFixture.ProjectId, serviceAccountEmail);
        Assert.Equal(key.Metadata.ServiceAccountEmail, serviceAccountEmail);

        //list keys
        var keys = listHmacKeysSample.ListHmacKeys(_bucketFixture.ProjectId).ToList();
        Assert.True(keys.Count > 0);

        //get key
        var keyMetadata = hmacKeyGetSample.GetHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);
        Assert.Equal(keyMetadata.ServiceAccountEmail, serviceAccountEmail);

        //deactivate key
        hmacKeyDeactivateSample.DeactivateHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);
        keyMetadata = hmacKeyGetSample.GetHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);
        Assert.Equal("INACTIVE", keyMetadata.State);

        //activate key
        hmacKeyActivateSample.ActivateHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);
        keyMetadata = hmacKeyGetSample.GetHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);
        Assert.Equal("ACTIVE", keyMetadata.State);

        //deactivate key
        hmacKeyDeactivateSample.DeactivateHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);

        //delete key
        hmacKeyDeleteSample.DeleteHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);
        keys = listHmacKeysSample.ListHmacKeys(_bucketFixture.ProjectId).ToList();
        Assert.True(keys.Count == 0);
    }

    private static string GetServiceAccountEmail()
    {
        var cred = GoogleCredential.GetApplicationDefault().UnderlyingCredential;
        switch (cred)
        {
            case ServiceAccountCredential sac:
                return sac.Id;
            // TODO: We may well need to handle ComputeCredential for Kokoro.
            default:
                throw new InvalidOperationException($"Unable to retrieve service account email address for credential type {cred.GetType()}");
        }
    }

    private static void DeleteAllHmacKeys(string projectId)
    {
        var client = StorageClient.Create();
        var key = client.ListHmacKeys(projectId);
        foreach (var metadata in key)
        {
            if (metadata.State == "ACTIVE")
            {
                metadata.State = HmacKeyStates.Inactive;
                client.UpdateHmacKey(metadata);
            }
            client.DeleteHmacKey(projectId, metadata.AccessId);
        }
    }
}
