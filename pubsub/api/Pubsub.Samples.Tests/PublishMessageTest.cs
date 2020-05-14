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

using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class PublishMessageTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly GetPublisherAsyncSample _getPublisherAsyncSample;
    private readonly GetCustomPublisherAsyncSample _getCustomPublisherAsyncSample;
    private readonly PublishMessagesAsyncSample _publishMessagesAsyncSample;
    private readonly PullMessagesAsyncSample _pullMessagesAsyncSample;
    public PublishMessageTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _getPublisherAsyncSample = new GetPublisherAsyncSample();
        _getCustomPublisherAsyncSample = new GetCustomPublisherAsyncSample();
        _publishMessagesAsyncSample = new PublishMessagesAsyncSample();
        _pullMessagesAsyncSample = new PullMessagesAsyncSample();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PublishMessage(bool customBatch)
    {
        string topicId = "testTopicForMessageCreation" + _pubsubFixture.RandomName();
        string subscriptionId = "testSubscriptionForMessageCreation" + _pubsubFixture.RandomName();

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);


        List<string> messageTexts = new[] { "Hello World!", "Good day.", "Bye bye." }.ToList();

        PublisherClient publisher;
        if (customBatch)
        {
            publisher = Task.Run(() =>
                 _getCustomPublisherAsyncSample.GetCustomPublisherAsync(_pubsubFixture.ProjectId, topicId))
                .ResultWithUnwrappedExceptions();
        }
        else
        {
            publisher = Task.Run(() =>
            _getPublisherAsyncSample.GetPublisherAsync(_pubsubFixture.ProjectId, topicId))
                .ResultWithUnwrappedExceptions();
        }


        var output = Task.Run(() =>
        _publishMessagesAsyncSample.PublishMessagesAsync(publisher, messageTexts))
            .ResultWithUnwrappedExceptions();

        Assert.Contains("Published message", output);

        // Pull the Message to confirm it is valid
        _pubsubFixture.Eventually(() =>
        {
            var result = Task.Run(() =>
            _pullMessagesAsyncSample.PullMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, false))
            .ResultWithUnwrappedExceptions();
            Assert.True(result.Count > 0);
        });
    }
}
