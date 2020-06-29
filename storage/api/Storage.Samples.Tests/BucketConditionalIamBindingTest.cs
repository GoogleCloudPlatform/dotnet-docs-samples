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
public class BucketConditionalIamBindingTest
{
    private readonly BucketFixture _bucketFixture;

    public BucketConditionalIamBindingTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void BucketConditionalIamBinding()
    {
        AddBucketConditionalIamBindingSample addBucketConditionalIamBindingSample = new AddBucketConditionalIamBindingSample();
        RemoveBucketConditionalIamBindingSample removeBucketConditionalIamBindingSample = new RemoveBucketConditionalIamBindingSample();
        ViewBucketIamMembersSample viewBucketIamMembersSample = new ViewBucketIamMembersSample();
        EnableUniformBucketLevelAccessSample enableUniformBucketLevelAccessSample = new EnableUniformBucketLevelAccessSample();
        DisableUniformBucketLevelAccessSample disableUniformBucketLevelAccessSample = new DisableUniformBucketLevelAccessSample();
        string member = "230835935096-8io28ro0tvbbv612p5k6nstlaucmhnrq@developer.gserviceaccount.com";
        string memberType = "serviceAccount";
        string role = "roles/storage.objectViewer";

        // Enable Uniform bucket level access
        enableUniformBucketLevelAccessSample.EnableUniformBucketLevelAccess(_bucketFixture.BucketName);

        // Add Conditional Binding
        addBucketConditionalIamBindingSample.AddBucketConditionalIamBinding(_bucketFixture.BucketName,
           role, $"{memberType}:{member}", "title", "description",
           "resource.name.startsWith(\"projects/_/buckets/bucket-name/objects/prefix-a-\")");

        // View Bucket Iam Members
        var policy = viewBucketIamMembersSample.ViewBucketIamMembers(_bucketFixture.BucketName);
        Assert.Contains(policy.Bindings, c => c.Members.Contains($"{memberType}:{member}"));

        // Remove Conditional Binding
        removeBucketConditionalIamBindingSample.RemoveBucketConditionalIamBinding(_bucketFixture.BucketName,
            role, "title", "description",
            "resource.name.startsWith(\"projects/_/buckets/bucket-name/objects/prefix-a-\")");

        policy = viewBucketIamMembersSample.ViewBucketIamMembers(_bucketFixture.BucketName);
        Assert.DoesNotContain(policy.Bindings, c => c.Members.Contains($"{memberType}:{member}"));

        // Disable Uniform bucket level access
        disableUniformBucketLevelAccessSample.DisableUniformBucketLevelAccess(_bucketFixture.BucketName);
    }
}
