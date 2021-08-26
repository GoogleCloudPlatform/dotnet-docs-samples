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

using System.Linq;
using Xunit;

[Collection(nameof(BucketFixture))]
public class RemoveBucketIamMemberTest
{
    private readonly BucketFixture _bucketFixture;

    public RemoveBucketIamMemberTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestRemoveBucketIamMember()
    {
        string role = "roles/storage.objectViewer";
        string memberType = "serviceAccount";
        var addBucketIamMemberSample = new AddBucketIamMemberSample();
        RemoveBucketIamMemberSample removeBucketIamMemberSample = new RemoveBucketIamMemberSample();
        ViewBucketIamMembersSample viewBucketIamMembersSample = new ViewBucketIamMembersSample();

        // Add bucket Iam member.
        addBucketIamMemberSample.AddBucketIamMember(_bucketFixture.BucketNameGeneric, role, $"{memberType}:{_bucketFixture.ServiceAccountEmail}");
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        // Remove bucket Iam member.
        removeBucketIamMemberSample.RemoveBucketIamMember(_bucketFixture.BucketNameGeneric, role, $"{memberType}:{_bucketFixture.ServiceAccountEmail}");
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        // Get bucket Iam member.
        var result = viewBucketIamMembersSample.ViewBucketIamMembers(_bucketFixture.BucketNameGeneric);
        Assert.DoesNotContain(result.Bindings.Where(b => b.Role == role).SelectMany(b => b.Members), m => m == $"{memberType}:{_bucketFixture.ServiceAccountEmail}");
    }
}
