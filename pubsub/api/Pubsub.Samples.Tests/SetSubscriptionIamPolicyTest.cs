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
public class SetSubscriptionIamPolicyTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly SetSubscriptionIamPolicySample _setSubscriptionIamPolicySample;
    private readonly GetSubscriptionIamPolicySample _getSubscriptionIamPolicySample;
    public SetSubscriptionIamPolicyTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _setSubscriptionIamPolicySample = new SetSubscriptionIamPolicySample();
        _getSubscriptionIamPolicySample = new GetSubscriptionIamPolicySample();
    }

    [Fact]
    public void SetSubscriptionIamPolicy()
    {
        string topicId = "testTopicSetSubscriptionIamPolicy" + _pubsubFixture.RandomName();
        string subscriptionId = "testSubscriptionSetSubscriptionIamPolicy" + _pubsubFixture.RandomName();
        string testRoleValueToConfirm = "pubsub.editor";
        string testMemberValueToConfirm = "group:cloud-logs@google.com";

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        var policy = _setSubscriptionIamPolicySample.SetSubscriptionIamPolicy(
            _pubsubFixture._projectId, subscriptionId, testRoleValueToConfirm,
            testMemberValueToConfirm);

        var policyOutput = _getSubscriptionIamPolicySample.GetSubscriptionIamPolicy(
            _pubsubFixture._projectId, subscriptionId);

        Assert.Equal(policy, policyOutput);
    }
}
