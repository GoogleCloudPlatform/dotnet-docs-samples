using System;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class DeleteTopicTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly DeleteTopicSample _deleteTopicSample;
    private readonly GetTopicSample _getTopicSample;
    public DeleteTopicTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _deleteTopicSample = new DeleteTopicSample();
        _getTopicSample = new GetTopicSample();
    }

    [Fact]
    public void DeleteTopic()
    {
        string topicId = "testTopicForDeleteTopic" + _pubsubFixture.RandomName();

        _pubsubFixture.CreateTopic(topicId);
        _deleteTopicSample.DeleteTopic(_pubsubFixture.ProjectId, topicId);

        _pubsubFixture.TempTopicIds.Remove(topicId);  // We already deleted it.

        Exception ex = Assert.Throws<Grpc.Core.RpcException>(() =>
            _getTopicSample.GetTopic(_pubsubFixture.ProjectId, topicId));
    }
}
