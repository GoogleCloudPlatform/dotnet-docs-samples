namespace Snippets
{
    // [START scc_receive_notifications]
    using System.Collections.Generic;
    using Google.Cloud.PubSub.V1;
    using Google.Protobuf;
    using Grpc.Core;
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class NotificationReceiver
    {
        private NotificationReceiver() {}

        public static async Task<object> ReceiveMessageAsync(String projectId, String subscriptionId, bool acknowledge)
		{
			var subscriptionName = new SubscriptionName(projectId, subscriptionId);

            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);

            // SubscriberClient runs your message handle function on multiple
            // threads to maximize throughput.
            subscriber.StartAsync(
                async (PubsubMessage message, CancellationToken cancel) =>
                {
                    string text = message.Data.ToStringUtf8();
                    await Console.Out.WriteLineAsync(
                        $"Message {message.MessageId}: {text}");
                    return acknowledge ? SubscriberClient.Reply.Ack
                        : SubscriberClient.Reply.Nack;
                });
            // Run for 3 seconds.
            await Task.Delay(3000);
            await subscriber.StopAsync(CancellationToken.None);
            return 0;
        }
	}
    // [END scc_receive_notifications]
}