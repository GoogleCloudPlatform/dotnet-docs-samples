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

[Collection(nameof(BucketFixture))]
public class DisableDefaultEventBasedHoldTest
{
    private readonly BucketFixture _bucketFixture;

    public DisableDefaultEventBasedHoldTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestDisableDefaultEventBasedHold()
    {
        EnableBucketDefaultEventBasedHoldSample enableBucketDefaultEventBasedHoldSample = new EnableBucketDefaultEventBasedHoldSample();
        DisableDefaultEventBasedHoldSample disableDefaultEventBasedHoldSample = new DisableDefaultEventBasedHoldSample();

        //enable default event based hold
        enableBucketDefaultEventBasedHoldSample.EnableBucketDefaultEventBasedHold(_bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        //disable default event based hold
        var updatedBucket = disableDefaultEventBasedHoldSample.DisableDefaultEventBasedHold(_bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
        Assert.False(updatedBucket.DefaultEventBasedHold);
    }
}
