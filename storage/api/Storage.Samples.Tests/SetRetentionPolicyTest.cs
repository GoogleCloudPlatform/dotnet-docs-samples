﻿// Copyright 2020 Google Inc.
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

[Collection(nameof(StorageFixture))]
public class SetRetentionPolicyTest
{
    private readonly StorageFixture _fixture;

    public SetRetentionPolicyTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestSetRetentionPolicy()
    {
        SetRetentionPolicySample setRetentionPolicySample = new SetRetentionPolicySample();
        RemoveRetentionPolicySample removeRetentionPolicySample = new RemoveRetentionPolicySample();
        var retentionPeriod = 5;

        // Set retention policy.
        var retentionPolicy = setRetentionPolicySample.SetRetentionPolicy(_fixture.BucketNameGeneric, retentionPeriod);
        _fixture.SleepAfterBucketCreateUpdateDelete();
        Assert.Equal(retentionPolicy.RetentionPeriod, retentionPeriod);

        // Remove retention policy.
        removeRetentionPolicySample.RemoveRetentionPolicy(_fixture.BucketNameGeneric);
        _fixture.SleepAfterBucketCreateUpdateDelete();
    }
}
