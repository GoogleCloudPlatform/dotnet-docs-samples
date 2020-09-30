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
public class AddFileOwnerTest
{
    private readonly BucketFixture _bucketFixture;

    public AddFileOwnerTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestAddFileOwner()
    {
        AddFileOwnerSample addFileOwnerSample = new AddFileOwnerSample();
        RemoveFileOwnerSample removeFileOwnerSample = new RemoveFileOwnerSample();

        // Add file owner.
        var result = addFileOwnerSample.AddFileOwner(_bucketFixture.BucketNameGeneric, _bucketFixture.FileName, _bucketFixture.ServiceAccountEmail);
        Assert.Contains(result.Acl, c => c.Role == "OWNER" && c.Email == _bucketFixture.ServiceAccountEmail);

        // Remove file owner.
        removeFileOwnerSample.RemoveFileOwner(_bucketFixture.BucketNameGeneric, _bucketFixture.FileName, _bucketFixture.ServiceAccountEmail);
    }
}
