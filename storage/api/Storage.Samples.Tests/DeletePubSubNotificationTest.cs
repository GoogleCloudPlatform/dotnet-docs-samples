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

using Google;
using System;
using Xunit;

[Collection(nameof(StorageFixture))]
public class DeletePubSubNotificationTest
{
    private readonly StorageFixture _fixture;

    public DeletePubSubNotificationTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void DeletePubSubNotification()
    {
        CreatePubSubNotificationSample createPubSubNotificationSample = new CreatePubSubNotificationSample();
        GetPubSubNotificationSample getPubSubNotificationSample = new GetPubSubNotificationSample();
        DeletePubSubNotificationSample deletePubSubNotificationSample = new DeletePubSubNotificationSample();

        var topicId = "test" + Guid.NewGuid().ToString();
        var topic = _fixture.CreateTopic(topicId);

        var notification = createPubSubNotificationSample.CreatePubSubNotification(_fixture.BucketNameGeneric, topic.Name);
        deletePubSubNotificationSample.DeletePubSubNotification(_fixture.BucketNameGeneric, notification.Id);

        var exception = Assert.Throws<GoogleApiException>(() => getPubSubNotificationSample.GetPubSubNotification(_fixture.BucketNameGeneric, notification.Id));
        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.HttpStatusCode);
    }
}
