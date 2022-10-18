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

[Collection(nameof(StorageFixture))]
public class RemoveBucketConditionalIamBindingTest
{
    private readonly StorageFixture _fixture;

    public RemoveBucketConditionalIamBindingTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestRemoveBucketConditionalIamBinding()
    {
        AddBucketConditionalIamBindingSample addBucketConditionalIamBindingSample = new AddBucketConditionalIamBindingSample();
        RemoveBucketConditionalIamBindingSample removeBucketConditionalIamBindingSample = new RemoveBucketConditionalIamBindingSample();
        ViewBucketIamMembersSample viewBucketIamMembersSample = new ViewBucketIamMembersSample();
        EnableUniformBucketLevelAccessSample enableUniformBucketLevelAccessSample = new EnableUniformBucketLevelAccessSample();
        DisableUniformBucketLevelAccessSample disableUniformBucketLevelAccessSample = new DisableUniformBucketLevelAccessSample();
        string memberType = "serviceAccount";
        string role = "roles/storage.objectViewer";

        // Enable Uniform bucket level access.
        enableUniformBucketLevelAccessSample.EnableUniformBucketLevelAccess(_fixture.BucketNameGeneric);
        _fixture.SleepAfterBucketCreateUpdateDelete();

        // Add Conditional Binding.
        addBucketConditionalIamBindingSample.AddBucketConditionalIamBinding(_fixture.BucketNameGeneric,
           role, $"{memberType}:{_fixture.ServiceAccountEmail}", "title", "description",
           "resource.name.startsWith(\"projects/_/buckets/bucket-name/objects/prefix-a-\")");
        _fixture.SleepAfterBucketCreateUpdateDelete();

        // Remove Conditional Binding.
        removeBucketConditionalIamBindingSample.RemoveBucketConditionalIamBinding(_fixture.BucketNameGeneric,
            role, "title", "description",
            "resource.name.startsWith(\"projects/_/buckets/bucket-name/objects/prefix-a-\")");
        _fixture.SleepAfterBucketCreateUpdateDelete();

        // Get Bucket Iam Members.
        var policy = viewBucketIamMembersSample.ViewBucketIamMembers(_fixture.BucketNameGeneric);
        Assert.DoesNotContain(policy.Bindings, c => c.Members.Contains($"{memberType}:{_fixture.ServiceAccountEmail}"));

        // Disable Uniform bucket level access
        disableUniformBucketLevelAccessSample.DisableUniformBucketLevelAccess(_fixture.BucketNameGeneric);
        _fixture.SleepAfterBucketCreateUpdateDelete();
    }
}
