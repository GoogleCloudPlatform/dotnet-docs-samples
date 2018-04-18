// Copyright(c) 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using System.Text.RegularExpressions;
using Xunit;

namespace GoogleCloudSamples
{
    public class IntentManagementTests : DialogflowTest
    {
        readonly string DisplayName = TestUtil.RandomName();
        readonly string MessageText = "fake message for testing";
        readonly string[] TrainingPhrasesParts = new[] { "test part 1", "test part 2" };
        string TrainingPartPhrasesArgument => string.Join(',', TrainingPhrasesParts);

        Regex CreateOutputPattern => new Regex(
            $"Created Intent: projects/{ProjectId}/agent/intents/(?<intentId>.*)");

        // Extract and return Intents ID from output of `intents:create`
        string GetIntentId(string createOutput) =>
            CreateOutputPattern.Match(createOutput).Groups["intentId"].Value;

        [Fact]
        void TestCreate()
        {
            Run("intents:list");
            Assert.DoesNotContain(DisplayName, Stdout);

            Run("intents:create", DisplayName, MessageText, TrainingPartPhrasesArgument);
            Assert.Matches(CreateOutputPattern, Stdout);

            Run("intents:list");
            Assert.Contains(DisplayName, Stdout);
        }

        [Fact(Skip = "Not implemented")]
        void TestList()
        {

        }

        [Fact]
        void TestDelete()
        {
            Run("intents:create", DisplayName, MessageText, TrainingPartPhrasesArgument);

            // Get the ID of the created Intent to delete from output of intents:create.
            // The Intent ID is needed to delete, the display name is not sufficient.
            var intentId = GetIntentId(createOutput: Stdout);

            Run("intents:list");
            Assert.Contains(DisplayName, Stdout);

            Run("intents:delete", intentId);
            Assert.Contains($"Deleted Intent: {intentId}", Stdout);

            Run("intents:list");
            Assert.DoesNotContain(DisplayName, Stdout);
        }
    }
}
