# Google Cloud Pub/Sub Sample

This C# sample demonstrates how to call the
[Google Cloud Pub/Sub API](https://cloud.google.com/pubsub/docs) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/pubsub/api).

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=pubsub.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Stackdriver Cloud Pub/Sub API.

7. Edit [QuickStart.cs](QuickStart/QuickStart.cs), and replace YOUR-PROJECT-ID with the id of the project you created in step 1.

9.  From a Powershell command line, run the QuickStart sample:
    ```
    PS C:\...\dotnet-docs-samples\pubsub\api\QuickStart> dotnet restore
    PS C:\...\dotnet-docs-samples\pubsub\api\QuickStart> dotnet run
    Topic projects/YOUR-PROJECT-ID/topics/my-new-topic created.
    ```

10. And run the Pubsub sample to see a list of subcommands like createTopic and
createSubscription:
    ```
    PS C:\...\dotnet-docs-samples\pubsub\api\PubsubSample> dotnet restore
    PS C:\...\dotnet-docs-samples\pubsub\api\PubsubSample> dotnet run
    Pubsub 1.0.0.0
    Copyright c Google Inc. 2017

    ERROR(S):
      No verb selected.

      createTopic                 Create a pubsub topic in this project.

      createSubscription          Create a pubsub subscription in this project.

      createTopicMessage          Create a pubsub topic message in this project.

      pullTopicMessages           Pull pubsub messages in this project.

      getTopic                    Get the details of a pubsub topic in this project.

      getSubscription             Get the details of a pubsub subscription in this project.

      getTopicIamPolicy           Get the IAM policy of a topic in this project.

      getSubscriptionIamPolicy    Get the IAM policy of a subscription in this project.

      setTopicIamPolicy           Set the IAM policy of a topic in this project.

      setSubscriptionIamPolicy    Set the IAM policy of a subscription in this project.

      listProjectTopics           List the pubsub topics in this project.

      listSubscriptions           List the pubsub subscriptions in this project.

      deleteSubscription          Delete a pubsub subscription in this project.

      deleteTopic                 Delete a pubsub topic in this project.

      help                        Display more information on a specific command.

      version                     Display version information.
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
