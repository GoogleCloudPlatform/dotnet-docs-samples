// Copyright 2022 Google Inc.
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

[Collection(nameof(StorageFixture))]
public class ListPubSubNotificationTest
{
    private readonly StorageFixture _fixture;

    public ListPubSubNotificationTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ListPubSubNotification()
    {
        CreatePubSubNotificationSample createPubSubNotificationSample = new CreatePubSubNotificationSample();
        ListPubSubNotificationSample listPubSubNotificationSample = new ListPubSubNotificationSample();

        var topicId = "test" + Guid.NewGuid().ToString();
        var topic = _fixture.CreateTopic(topicId);

        var notification = createPubSubNotificationSample.CreatePubSubNotification(_fixture.BucketNameGeneric, topic.Name);
        var listOfPubSubNotifications = listPubSubNotificationSample.ListPubSubNotification(_fixture.BucketNameGeneric);

        Assert.Contains(listOfPubSubNotifications, x => x.Id == notification.Id);
    }

}
