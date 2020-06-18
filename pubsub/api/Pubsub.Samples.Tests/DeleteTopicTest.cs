using System;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class DeleteTopicTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly DeleteTopicSample _deleteTopicSample;
    public DeleteTopicTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _deleteTopicSample = new DeleteTopicSample();
    }

    [Fact]
    public void DeleteTopic()
    {
        string topicId = "testTopicForDeleteTopic" + _pubsubFixture.RandomName();

        _pubsubFixture.CreateTopic(topicId);
        _deleteTopicSample.DeleteTopic(_pubsubFixture.ProjectId, topicId);

        Exception ex = Assert.Throws<Grpc.Core.RpcException>(() => _pubsubFixture.GetTopic(topicId));

        _pubsubFixture.TempTopicIds.Remove(topicId);  // We already deleted it.
    }
}
