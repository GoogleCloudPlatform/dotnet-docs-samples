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

[Collection(nameof(SecretManagerFixture))]
public class BindTagsToSecretTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly BindTagsToSecretSample _sample;
    private readonly TagKeysClient _tagKeysClient;
    private readonly TagValuesClient _tagValuesClient;
    private readonly TagBindingsClient _tagBindingsClient;
    private string _tagKeyName;
    private string _tagValueName;
    private string _tagBindingName;

    public BindTagsToSecretTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new BindTagsToSecretSample();
        _tagKeysClient = TagKeysClient.Create();
        _tagValuesClient = TagValuesClient.Create();
        _tagBindingsClient = TagBindingsClient.Create();
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

    private void CleanupResources()
    {
        // Delete the tag binding if it exists
        if (!string.IsNullOrEmpty(_tagBindingName))
        {
            try
            {
                var deleteBindingRequest = new DeleteTagBindingRequest { Name = _tagBindingName };
                _tagBindingsClient.DeleteTagBinding(deleteBindingRequest).PollUntilCompleted();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting tag binding: {e.Message}");
            }
        }

        // Delete the tag value if it exists
        if (!string.IsNullOrEmpty(_tagValueName))
        {
            try
            {
                var deleteValueRequest = new DeleteTagValueRequest { Name = _tagValueName };
                _tagValuesClient.DeleteTagValue(deleteValueRequest).PollUntilCompleted();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting tag value: {e.Message}");
            }
        }

        // Delete the tag key if it exists
        if (!string.IsNullOrEmpty(_tagKeyName))
        {
            try
            {
                var deleteKeyRequest = new DeleteTagKeyRequest { Name = _tagKeyName };
                _tagKeysClient.DeleteTagKey(deleteKeyRequest).PollUntilCompleted();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting tag key: {e.Message}");
            }
        }
    }

    [Fact]
    public async Task BindsTagToSecretAsync()
    {
        // Create a tag key and value for testing
        CreateTagKeyAndValue(_fixture.ProjectId);

        // Generate a unique secret ID
        SecretName secretName = new SecretName(_fixture.ProjectId, _fixture.RandomId());
        // string secretId = _fixture.RandomId();

        // Capture console output
        StringWriter sw = new StringWriter();
        Console.SetOut(sw);

        await _sample.BindTagsToSecretAsync(
            projectId: secretName.ProjectId,
            secretId: secretName.SecretId,
            tagValue: _tagValueName);

        // Get the console output
        string consoleOutput = sw.ToString().Trim();

        // Verify the result

        Assert.Contains($"Created secret: ", consoleOutput);
        Assert.Contains($"Created Tag Binding", consoleOutput);

        // Reset console
        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);

        _fixture.DeleteSecret(secretName);
        CleanupResources();

    }
};
