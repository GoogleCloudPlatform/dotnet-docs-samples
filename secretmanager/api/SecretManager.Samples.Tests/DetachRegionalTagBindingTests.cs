/*
 * Copyright 2026 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Google.Cloud.ResourceManager.V3;
using Google.Cloud.SecretManager.V1;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class DetachTagFromRegionalSecretTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly BindTagsToRegionalSecretSample _bindSample;
    private readonly DetachTagFromRegionalSecretSample _detachSample;
    private readonly ListRegionalSecretTagBindingsSample _listSample;
    private readonly TagKeysClient _tagKeysClient;
    private readonly TagValuesClient _tagValuesClient;
    private string _tagKeyName;
    private string _tagValueName;

    public DetachTagFromRegionalSecretTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _bindSample = new BindTagsToRegionalSecretSample();
        _detachSample = new DetachTagFromRegionalSecretSample();
        _listSample = new ListRegionalSecretTagBindingsSample();
        _tagKeysClient = TagKeysClient.Create();
        _tagValuesClient = TagValuesClient.Create();
    }

    private void CreateTagKeyAndValue(string projectId)
    {
        // Generate unique names for tag key and value
        string tagKeyShortName = $"test-key-{_fixture.RandomId()}";
        string tagValueShortName = $"test-value-{_fixture.RandomId()}";

        var createKeyRequest = new CreateTagKeyRequest
        {
            TagKey = new TagKey
            {
                Parent = $"projects/{projectId}",
                ShortName = tagKeyShortName,
            }
        };

        try
        {
            var createKeyOperation = _tagKeysClient.CreateTagKey(createKeyRequest);
            TagKey tagKey = createKeyOperation.PollUntilCompleted().Result;
            _tagKeyName = tagKey.Name;
        }
        catch (Exception e)
        {
            throw new Exception("Error creating tag key: " + e.Message, e);
        }

        var createValueRequest = new CreateTagValueRequest
        {
            TagValue = new TagValue
            {
                Parent = _tagKeyName,
                ShortName = tagValueShortName,
            }
        };

        try
        {
            var createValueOperation = _tagValuesClient.CreateTagValue(createValueRequest);
            TagValue tagValue = createValueOperation.PollUntilCompleted().Result;
            _tagValueName = tagValue.Name;
        }
        catch (Exception e)
        {
            throw new Exception("Error creating tag value: " + e.Message, e);
        }
    }

    private void CleanupResource()
    {

        // Delete the tag value
        if (!string.IsNullOrEmpty(_tagValueName))
        {
            try
            {
                var deleteValueRequest = new DeleteTagValueRequest { Name = _tagValueName };
                _tagValuesClient.DeleteTagValue(deleteValueRequest).PollUntilCompleted();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting tag value: {e.GetType().Name}: {e.Message}");
            }
        }

        // Delete the tag key
        if (!string.IsNullOrEmpty(_tagKeyName))
        {
            try
            {
                var deleteKeyRequest = new DeleteTagKeyRequest { Name = _tagKeyName };
                _tagKeysClient.DeleteTagKey(deleteKeyRequest).PollUntilCompleted();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting tag key: {e.GetType().Name}: {e.Message}");
            }
        }
    }

    [Fact]
    public async Task DetachesTagFromRegionalSecret()
    {
        // Create a tag key and value for testing
        CreateTagKeyAndValue(_fixture.ProjectId);

        SecretName secretName = SecretName.FromProjectLocationSecret(_fixture.ProjectId, _fixture.LocationId, _fixture.RandomId());

        // Create a regional secret and bind the tag to it
        await _bindSample.BindTagsToRegionalSecretAsync(
            projectId: secretName.ProjectId,
            locationId: secretName.LocationId,
            secretId: secretName.SecretId,
            tagValue: _tagValueName);


        // Detach the tag
        string bindingName = await _detachSample.DetachTagFromRegionalSecretAsync(
            projectId: secretName.ProjectId,
            locationId: secretName.LocationId,
            secretId: secretName.SecretId,
            tagValue: _tagValueName);

        Assert.NotEmpty(bindingName);

        bindingName = await _detachSample.DetachTagFromRegionalSecretAsync(
            projectId: secretName.ProjectId,
            locationId: secretName.LocationId,
            secretId: secretName.SecretId,
            tagValue: _tagValueName);

        Assert.Empty(bindingName);

        // Clean up all resources
        _fixture.DeleteSecret(secretName);
        CleanupResource();
    }
};
