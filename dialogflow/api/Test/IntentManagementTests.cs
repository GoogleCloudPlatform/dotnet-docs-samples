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
        readonly string _displayName = TestUtil.RandomName();
        readonly string _messageText = "fake message for testing";
        readonly string[] _trainingPhrasesParts = new[] { "test part 1", "test part 2" };
        string TrainingPartPhrasesArgument => string.Join(',', _trainingPhrasesParts);

        Regex CreateOutputPattern => new Regex(
            $"Created Intent: projects/{ProjectId}/agent/intents/(?<intentId>.*)");

        // Extract and return Intents ID from output of `intents:create`
        string GetIntentId(string createOutput) =>
            CreateOutputPattern.Match(createOutput).Groups["intentId"].Value;

        [Fact]
        void TestCreate()
        {
            Run("intents:list");
            Assert.DoesNotContain(_displayName, Stdout);

            Run("intents:create", _displayName, _messageText, TrainingPartPhrasesArgument);
            Assert.Matches(CreateOutputPattern, Stdout);
            var intentId = GetIntentId(createOutput: Stdout);
            CleanupAfterTest("intents:delete", intentId);

            _retryRobot.Eventually(() =>
            {
                Run("intents:list");
                Assert.Contains(_displayName, Stdout);
            });
        }

        [Fact]
        void TestDelete()
        {
            Run("intents:create", _displayName, _messageText, TrainingPartPhrasesArgument);

            // Get the ID of the created Intent to delete from output of intents:create.
            // The Intent ID is needed to delete, the display name is not sufficient.
            var intentId = GetIntentId(createOutput: Stdout);
            var cancelCleanup = CleanupAfterTest("intents:delete", intentId);

            _retryRobot.Eventually(() =>
            {
                Run("intents:list");
                Assert.Contains(_displayName, Stdout);
            });

            Run("intents:delete", intentId);
            Assert.Contains($"Deleted Intent: {intentId}", Stdout);
            cancelCleanup.Cancel();

            _retryRobot.Eventually(() =>
           {
               Run("intents:list");
               Assert.DoesNotContain(_displayName, Stdout);
           });
        }
    }
}
