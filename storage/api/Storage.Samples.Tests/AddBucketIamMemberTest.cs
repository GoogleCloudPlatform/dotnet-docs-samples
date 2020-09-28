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
public class AddBucketIamMemberTest
{
    private readonly BucketFixture _bucketFixture;

    public AddBucketIamMemberTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestAddBucketIamMember()
    {
        string role = "roles/storage.objectViewer";
        string memberType = "serviceAccount";
        AddBucketIamMemberSample addBucketIamMemberSample = new AddBucketIamMemberSample();
        RemoveBucketIamMemberSample removeBucketIamMemberSample = new RemoveBucketIamMemberSample();

        // Add bucket Iam member.
        var result = addBucketIamMemberSample.AddBucketIamMember(_bucketFixture.BucketNameGeneric, role, $"{memberType}:{_bucketFixture.ServiceAccountEmail}");
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
        Assert.Contains(result.Bindings, b => b.Role == role && b.Members.Contains($"{memberType}:{_bucketFixture.ServiceAccountEmail}"));

        // Remove bucket Iam member.
        removeBucketIamMemberSample.RemoveBucketIamMember(_bucketFixture.BucketNameGeneric, role, $"{memberType}:{_bucketFixture.ServiceAccountEmail}");
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
    }
}
