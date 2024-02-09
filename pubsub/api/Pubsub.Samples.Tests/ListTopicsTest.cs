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

using Xunit;

[Collection(nameof(PubsubFixture))]
public class ListTopicsTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly ListProjectTopicsSample _listProjectTopicsSample;

    public ListTopicsTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _listProjectTopicsSample = new ListProjectTopicsSample();
    }

    [Fact]
    public void ListTopics()
    {
        string topicId = _pubsubFixture.RandomTopicId();
        _pubsubFixture.CreateTopic(topicId);

        var listProjectTopicsOutput = _listProjectTopicsSample.ListProjectTopics(_pubsubFixture.ProjectId);
        Assert.Contains(listProjectTopicsOutput, c => c.TopicName.TopicId.Contains(topicId));
    }
}
