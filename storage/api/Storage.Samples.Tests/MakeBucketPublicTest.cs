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

using Google.Cloud.Storage.V1;
using Xunit;

[Collection(nameof(StorageFixture))]
public class MakeBucketPublicTest
{
    private readonly StorageFixture _fixture;

    public MakeBucketPublicTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void MakeBucketPublic()
    {
        var storage = StorageClient.Create();

        MakeBucketPublicSample makeBucketPublicSample = new MakeBucketPublicSample();
        makeBucketPublicSample.MakeBucketPublic(_fixture.BucketNameGeneric);
       
        Assert.Contains(storage.GetBucketIamPolicy(_fixture.BucketNameGeneric).Bindings, binding => binding.Role == "roles/storage.objectViewer" && binding.Members.Contains("allUsers"));
    }
}
