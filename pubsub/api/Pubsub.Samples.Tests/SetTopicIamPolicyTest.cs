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

[Collection(nameof(PubsubFixture))]
public class SetTopicIamPolicyTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly SetTopicIamPolicySample _setTopicIamPolicySample;

    public SetTopicIamPolicyTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _setTopicIamPolicySample = new SetTopicIamPolicySample();
    }

    [Fact]
    public void SetTopicIamPolicy()
    {
        string topicId = _pubsubFixture.RandomTopicId();
        string testRoleValueToConfirm = "pubsub.editor";
        string testMemberValueToConfirm = "group:cloud-logs@google.com";

        _pubsubFixture.CreateTopic(topicId);

        var policy = _setTopicIamPolicySample.SetTopicIamPolicy(_pubsubFixture.ProjectId, topicId, testRoleValueToConfirm, testMemberValueToConfirm);

        Assert.Equal($"roles/{testRoleValueToConfirm}", policy.Bindings[0].Role);
        Assert.Contains(policy.Bindings[0].Members, c => c.Contains(testMemberValueToConfirm));
    }
}
