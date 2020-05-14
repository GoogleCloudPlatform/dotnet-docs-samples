using System;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class DeleteSubscriptionTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly DeleteSubscriptionSample _deleteSubscriptionSample;
    private readonly GetSubscriptionSample _getSubscriptionSample;
    public DeleteSubscriptionTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _deleteSubscriptionSample = new DeleteSubscriptionSample();
        _getSubscriptionSample = new GetSubscriptionSample();
    }

    [Fact]
    public void TestDeleteSubscription()
    {
        string topicId = "testTopicForDeleteSubscription" + _pubsubFixture.RandomName();
        string subscriptionId = "testSubscriptionForDeleteSubscription" + _pubsubFixture.RandomName();

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        _deleteSubscriptionSample.DeleteSubscription(_pubsubFixture.ProjectId, subscriptionId);

        _pubsubFixture.TempSubscriptionIds.Remove(subscriptionId);  // We already deleted it.

        Exception e = Assert.Throws<Grpc.Core.RpcException>(() =>
            _getSubscriptionSample.GetSubscription(_pubsubFixture.ProjectId,subscriptionId));
    }
}
