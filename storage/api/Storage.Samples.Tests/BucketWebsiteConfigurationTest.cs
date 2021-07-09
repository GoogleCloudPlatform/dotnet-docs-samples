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
public class BucketWebsiteConfigurationTest
{
    private readonly BucketFixture _bucketFixture;

    public BucketWebsiteConfigurationTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void BucketWebsiteConfiguration()
    {
        BucketWebsiteConfigurationSample bucketWebsiteConfigurationSample = new BucketWebsiteConfigurationSample();

        var mainPageSuffix = "index.html";
        var notFoundPage = "404.html";

        var bucket = bucketWebsiteConfigurationSample.BucketWebsiteConfiguration(_bucketFixture.BucketNameGeneric, mainPageSuffix, notFoundPage);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        Assert.Equal(mainPageSuffix, bucket.Website.MainPageSuffix);
        Assert.Equal(notFoundPage, bucket.Website.NotFoundPage);
    }
}
