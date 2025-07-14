/*
 * Copyright 2025 Google LLC
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
using Xunit;

[Collection(nameof(SecretManagerFixture))]
public class CreateSecretWithTagsTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly CreateSecretWithTagsSample _sample;
    private readonly TagKeysClient _tagKeysClient;
    private readonly TagValuesClient _tagValuesClient;
    private string _tagKeyName;
    private string _tagValueName;

    public CreateSecretWithTagsTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateSecretWithTagsSample();
        _tagKeysClient = TagKeysClient.Create();
        _tagValuesClient = TagValuesClient.Create();
    }

    private void CreateTagKeyAndValue(string projectId, string tagKeyShortName, string tagValueShortName)
    {
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
            throw new Exception($"Error creating tag key: {e.Message}");
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
            throw new Exception($"Error creating tag value: {e.Message}");
        }
    }

    private void CleanTagKeyandValue()
    {
        if (!string.IsNullOrEmpty(_tagValueName))
        {
            try
            {
                DeleteTagValueRequest deleteValueRequest = new DeleteTagValueRequest { Name = _tagValueName };
                _tagValuesClient.DeleteTagValue(deleteValueRequest).PollUntilCompleted();
            }
            catch (Exception e)
            {
                Console.WriteLine("Deleting the tag value failed!");
                throw new Exception($"Error deleting tag value: {e.Message}");
            }
        }
        else
        {
            Console.WriteLine("TagValue.name was not set, skipping deletion.");
        }

        if (!string.IsNullOrEmpty(_tagKeyName))
        {
            try
            {
                DeleteTagKeyRequest deleteKeyRequest = new DeleteTagKeyRequest { Name = _tagKeyName };
                _tagKeysClient.DeleteTagKey(deleteKeyRequest).PollUntilCompleted();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Deleting the tag key failed!");
                throw new Exception($"Error deleting tag key: {e.Message}");
            }
        }
        else
        {
            Console.WriteLine("TagKey.name was not set, skipping deletion.");
        }
    }

    [Fact]
    public void CreatesSecretsWithTags()
    {
        // Get the SecretName to create Secret.
        SecretName secretName = new SecretName(_fixture.ProjectId, _fixture.RandomId());

        // Define tag key and value short names
        string tagKeyShortName = $"test-key-{_fixture.RandomId()}";
        string tagValueShortName = $"test-value-{_fixture.RandomId()}";

        CreateTagKeyAndValue(_fixture.ProjectId, tagKeyShortName, tagValueShortName);

        // Create the secret with the specificed fields.
        Secret result = _sample.CreateSecretWithTags(
          projectId: secretName.ProjectId, secretId: secretName.SecretId, tagKeyName: _tagKeyName, tagValueName: _tagValueName);

        // Assert that the secretId is same as expected.
        Assert.Equal(result.SecretName.SecretId, secretName.SecretId);

        _fixture.DeleteSecret(secretName);
        CleanTagKeyandValue();
    }
}
