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

[Collection(nameof(StorageFixture))]
public class ViewBucketIamMembersTest
{
    private readonly StorageFixture _fixture;

    public ViewBucketIamMembersTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestViewBucketIamMembers()
    {
        string role = "roles/storage.objectViewer";
        string memberType = "serviceAccount";
        var addBucketIamMemberSample = new AddBucketIamMemberSample();
        RemoveBucketIamMemberSample removeBucketIamMemberSample = new RemoveBucketIamMemberSample();
        ViewBucketIamMembersSample viewBucketIamMembersSample = new ViewBucketIamMembersSample();

        // Add bucket Iam members.
        addBucketIamMemberSample.AddBucketIamMember(_fixture.BucketNameGeneric, role, $"{memberType}:{_fixture.ServiceAccountEmail}");
        _fixture.SleepAfterBucketCreateUpdateDelete();

        // Get bucket Iam members.
        var result = viewBucketIamMembersSample.ViewBucketIamMembers(_fixture.BucketNameGeneric);
        Assert.Contains(result.Bindings.Where(b => b.Role == role).SelectMany(b => b.Members), m => m == $"{memberType}:{_fixture.ServiceAccountEmail}");

        // Remove bucket Iam members.
        removeBucketIamMemberSample.RemoveBucketIamMember(_fixture.BucketNameGeneric, role, $"{memberType}:{_fixture.ServiceAccountEmail}");
        _fixture.SleepAfterBucketCreateUpdateDelete();
    }
}
