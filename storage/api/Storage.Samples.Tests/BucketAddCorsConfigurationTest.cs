// Copyright 2021 Google Inc.
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
public class BucketAddCorsConfigurationTest
{
    private readonly BucketFixture _bucketFixture;

    public BucketAddCorsConfigurationTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void BucketAddCorsConfiguration()
    {
        BucketAddCorsConfigurationSample bucketAddCorsConfigurationSample = new BucketAddCorsConfigurationSample();
        BucketRemoveCorsConfigurationSample bucketRemoveCorsConfigurationSample= new BucketRemoveCorsConfigurationSample();

        // Add Cors Configuration
        var bucket = bucketAddCorsConfigurationSample.BucketAddCorsConfiguration(_bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        Assert.Equal("*", bucket.Cors[0].Origin[0]);
        Assert.Equal("PUT", bucket.Cors[0].Method[0]);
        Assert.Equal(3600, bucket.Cors[0].MaxAgeSeconds);

        // Remove Cors Configurations
        bucketRemoveCorsConfigurationSample.BucketRemoveCorsConfiguration(_bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
    }
}
