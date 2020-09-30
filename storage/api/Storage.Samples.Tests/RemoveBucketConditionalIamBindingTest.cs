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
public class RemoveBucketConditionalIamBindingTest
{
    private readonly BucketFixture _bucketFixture;

    public RemoveBucketConditionalIamBindingTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
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
        enableUniformBucketLevelAccessSample.EnableUniformBucketLevelAccess(_bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        // Add Conditional Binding.
        addBucketConditionalIamBindingSample.AddBucketConditionalIamBinding(_bucketFixture.BucketNameGeneric,
           role, $"{memberType}:{_bucketFixture.ServiceAccountEmail}", "title", "description",
           "resource.name.startsWith(\"projects/_/buckets/bucket-name/objects/prefix-a-\")");
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        // Remove Conditional Binding.
        removeBucketConditionalIamBindingSample.RemoveBucketConditionalIamBinding(_bucketFixture.BucketNameGeneric,
            role, "title", "description",
            "resource.name.startsWith(\"projects/_/buckets/bucket-name/objects/prefix-a-\")");
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        // Get Bucket Iam Members.
        var policy = viewBucketIamMembersSample.ViewBucketIamMembers(_bucketFixture.BucketNameGeneric);
        Assert.DoesNotContain(policy.Bindings, c => c.Members.Contains($"{memberType}:{_bucketFixture.ServiceAccountEmail}"));

        // Disable Uniform bucket level access
        disableUniformBucketLevelAccessSample.DisableUniformBucketLevelAccess(_bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
    }
}
