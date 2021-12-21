﻿// Copyright 2021 Google Inc.
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
using System;
using System.Collections.Generic;
using System.Text;

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

        var bucketName = Guid.NewGuid().ToString();
        _fixture.CreateBucket(bucketName);

        createPubSubNotificationSample.CreatePubSubNotification(bucketName, "my-topic");
        var listOfPubSubNotifications = listPubSubNotificationSample.ListPubSubNotification(bucketName);

        Assert.Contains(listOfPubSubNotifications, x => x.Topic == "my-topic");
    }

}
