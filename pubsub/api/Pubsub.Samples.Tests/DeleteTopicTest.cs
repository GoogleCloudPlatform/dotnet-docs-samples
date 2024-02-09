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

using System;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class DeleteTopicTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly DeleteTopicSample _deleteTopicSample;

    public DeleteTopicTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _deleteTopicSample = new DeleteTopicSample();
    }

    [Fact]
    public void DeleteTopic()
    {
        string topicId = _pubsubFixture.RandomTopicId();

        _pubsubFixture.CreateTopic(topicId);
        _deleteTopicSample.DeleteTopic(_pubsubFixture.ProjectId, topicId);

        Exception ex = Assert.Throws<Grpc.Core.RpcException>(() => _pubsubFixture.GetTopic(topicId));

        _pubsubFixture.TempTopicIds.Remove(topicId);  // We already deleted it.
    }
}
