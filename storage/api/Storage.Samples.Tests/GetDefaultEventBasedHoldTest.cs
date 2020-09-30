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
public class GetDefaultEventBasedHoldTest
{
    private readonly BucketFixture _bucketFixture;

    public GetDefaultEventBasedHoldTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestGetDefaultEventBasedHold()
    {
        EnableBucketDefaultEventBasedHoldSample enableBucketDefaultEventBasedHoldSample = new EnableBucketDefaultEventBasedHoldSample();
        DisableDefaultEventBasedHoldSample disableDefaultEventBasedHoldSample = new DisableDefaultEventBasedHoldSample();
        GetDefaultEventBasedHoldSample getDefaultEventBasedHoldSample = new GetDefaultEventBasedHoldSample();

        // Enable default event based hold.
        enableBucketDefaultEventBasedHoldSample.EnableBucketDefaultEventBasedHold(_bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        // Get default event based hold.
        var defaultEventBasedHold = getDefaultEventBasedHoldSample.GetDefaultEventBasedHold(_bucketFixture.BucketNameGeneric);
        Assert.True(defaultEventBasedHold);

        // Disable default event based hold.
        disableDefaultEventBasedHoldSample.DisableDefaultEventBasedHold(_bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
    }
}
