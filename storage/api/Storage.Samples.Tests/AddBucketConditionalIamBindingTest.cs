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

using System;
using Xunit;

[Collection(nameof(BucketFixture))]
public class AddBucketConditionalIamBindingTest
{
    private readonly BucketFixture _bucketFixture;

    public AddBucketConditionalIamBindingTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestAddBucketConditionalIamBinding()
    {
        CreateBucketSample createBucketSample = new CreateBucketSample();
        AddBucketConditionalIamBindingSample addBucketConditionalIamBindingSample = new AddBucketConditionalIamBindingSample();
        RemoveBucketConditionalIamBindingSample removeBucketConditionalIamBindingSample = new RemoveBucketConditionalIamBindingSample();
        ViewBucketIamMembersSample viewBucketIamMembersSample = new ViewBucketIamMembersSample();
        EnableUniformBucketLevelAccessSample enableUniformBucketLevelAccessSample = new EnableUniformBucketLevelAccessSample();
        DisableUniformBucketLevelAccessSample disableUniformBucketLevelAccessSample = new DisableUniformBucketLevelAccessSample();
        var bucketName = Guid.NewGuid().ToString();
        string memberType = "serviceAccount";
        string role = "roles/storage.objectViewer";

        // Create bucket
        createBucketSample.CreateBucket(_bucketFixture.ProjectId, bucketName);
        _bucketFixture.SleepAfterBucketCreateDelete();
        _bucketFixture.TempBucketNames.Add(bucketName);

        // Enable Uniform bucket level access.
        enableUniformBucketLevelAccessSample.EnableUniformBucketLevelAccess(bucketName);

        // Add Conditional Binding.
        var policy = addBucketConditionalIamBindingSample.AddBucketConditionalIamBinding(bucketName,
           role, $"{memberType}:{_bucketFixture.ServiceAccountEmail}", "title", "description",
           "resource.name.startsWith(\"projects/_/buckets/bucket-name/objects/prefix-a-\")");

        Assert.Contains(policy.Bindings, c => c.Members.Contains($"{memberType}:{_bucketFixture.ServiceAccountEmail}"));

        // Remove Conditional Binding.
        removeBucketConditionalIamBindingSample.RemoveBucketConditionalIamBinding(bucketName,
            role, "title", "description",
            "resource.name.startsWith(\"projects/_/buckets/bucket-name/objects/prefix-a-\")");

        // Disable Uniform bucket level access.
        disableUniformBucketLevelAccessSample.DisableUniformBucketLevelAccess(bucketName);
    }
}
