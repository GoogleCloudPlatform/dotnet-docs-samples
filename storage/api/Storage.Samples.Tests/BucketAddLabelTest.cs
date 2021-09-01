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

[Collection(nameof(StorageFixture))]
public class BucketAddLabelTest
{
    private readonly StorageFixture _fixture;

    public BucketAddLabelTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void BucketAddLabel()
    {
        BucketAddLabelSample bucketAddLabelSample = new BucketAddLabelSample();
        BucketRemoveLabelSample bucketRemoveLabelSample = new BucketRemoveLabelSample();
        var labelKey = "usage";
        var labelValue = "chat-attachments";

        // Add Label
        var bucket = bucketAddLabelSample.BucketAddLabel(_fixture.BucketNameGeneric, labelKey, labelValue);
        _fixture.SleepAfterBucketCreateUpdateDelete();
        Assert.Contains(bucket.Labels, l => l.Key == labelKey && l.Value == labelValue);

        // Remove Label
        bucketRemoveLabelSample.BucketRemoveLabel(_fixture.BucketNameGeneric, labelKey);
        _fixture.SleepAfterBucketCreateUpdateDelete();
    }
}
