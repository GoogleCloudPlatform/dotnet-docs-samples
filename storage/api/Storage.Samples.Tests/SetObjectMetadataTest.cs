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
public class SetObjectMetadataTest
{
    private readonly BucketFixture _bucketFixture;

    public SetObjectMetadataTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void SetObjectMetadata()
    {
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        SetObjectMetadataSample setObjectMetadataSample = new SetObjectMetadataSample();

        var key = "key-to-add";
        var value = "value-to-add";
        var objectName = "HelloSetObjectMetadata.txt";

        uploadFileSample.UploadFile(_bucketFixture.BucketNameGeneric, _bucketFixture.FilePath, _bucketFixture.Collect(objectName));

        setObjectMetadataSample.SetObjectMetadata(_bucketFixture.BucketNameGeneric, objectName, key, value);

        var file = getMetadataSample.GetMetadata(_bucketFixture.BucketNameGeneric, objectName);
        Assert.Contains(file.Metadata, m => m.Key == key && m.Value == value);
    }
}
