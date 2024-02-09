// Copyright 2021 Google LLC
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     https://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Threading.Tasks;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class EmulatorSupportTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly EmulatorSupportSample _emulatorSupportSample;
    private readonly PullMessagesAsyncSample _pullMessagesAsyncSample;

    public EmulatorSupportTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _emulatorSupportSample = new EmulatorSupportSample();
        _pullMessagesAsyncSample = new PullMessagesAsyncSample();
    }

    [Fact]
    public async Task WithEmulatorAsync()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();

        _pubsubFixture.TempTopicIds.Add(topicId);
        _pubsubFixture.TempSubscriptionIds.Add(subscriptionId);

        // We just test that the method finishes without error.
        // Note that we don't set the PUBSUB_EMULATOR_HOST so, this really executes against production,
        // but that's fine.
        await _emulatorSupportSample.WithEmulatorAsync(_pubsubFixture.ProjectId, topicId, subscriptionId);
    }
}
