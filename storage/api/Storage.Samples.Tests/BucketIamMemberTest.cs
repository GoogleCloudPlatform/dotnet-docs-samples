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
public class BucketIamMemberTest
{
    private readonly BucketFixture _bucketFixture;

    public BucketIamMemberTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void BucketIamMember()
    {
        string role = "roles/storage.objectViewer";
        string memberType = "serviceAccount";
        var addBucketIamMemberSample = new AddBucketIamMemberSample();
        RemoveBucketIamMemberSample removeBucketIamMemberSample = new RemoveBucketIamMemberSample();
        ViewBucketIamMembersSample viewBucketIamMembersSample = new ViewBucketIamMembersSample();

        // add bucket Iam member
        var result = addBucketIamMemberSample.AddBucketIamMember(_bucketFixture.BucketName, role, $"{memberType}:{_bucketFixture.ServiceAccountEmail}");
        Assert.Contains(result.Bindings, cs => cs.Role == role);
        Assert.Contains(result.Bindings.SelectMany(c => c.Members), c => c == $"{memberType}:{_bucketFixture.ServiceAccountEmail}");

        // remove bucket Iam member
        removeBucketIamMemberSample.RemoveBucketIamMember(_bucketFixture.BucketName, role, $"{memberType}:{_bucketFixture.ServiceAccountEmail}");

        result = viewBucketIamMembersSample.ViewBucketIamMembers(_bucketFixture.BucketName);
        Assert.DoesNotContain(result.Bindings, cs => cs.Role == role);
        Assert.DoesNotContain(result.Bindings.SelectMany(c => c.Members), c => c == $"{memberType}:{_bucketFixture.ServiceAccountEmail}");
    }
}
