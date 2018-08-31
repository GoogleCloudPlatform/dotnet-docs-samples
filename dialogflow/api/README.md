# .NET Dialogflow Enterprise Edition Samples

A collection of samples that demonstrate how to call the
[Dialogflow Enterprise Edition API](https://cloud.google.com/dialogflow-enterprise/docs/) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/video/api).

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

1.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=dialogflow.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Dialogflow API.

1.  (Optional) Import sample Dialogflow agent using [these instructions](https://cloud.google.com/dialogflow-enterprise/docs/quickstart-client-libraries#import-the-sample-dialogflow-agent)

1.  From a Powershell command line, run the samples:
    ```
    PS C:\...\dotnet-docs-samples\dialogflow\api\DialogflowSamples> dotnet restore
    PS C:\...\dotnet-docs-samples\dialogflow\api\DialogflowSamples> dotnet run
    DialogflowSamples 0.0.0.0
    Copyright (C) 1 author

      contexts:create                Create new Context

      contexts:list                  Print list of entities for given Context

      contexts:delete                Delete specified Context

      intents:create                 Create new Intent

      intents:list                   Print list of entities for given Intent

      intents:delete                 Delete specified Intent

      entities:create                Create new entity type

      entities:list                  Print list of entities for given EntityType

      entities:delete                Delete specified EntityType

      entity-types:create            Create new entity type

      entity-types:list              Print list of all entity types

      entity-types:delete            Delete specified EntityType

      session-entity-types:create    Create new session entity type

      session-entity-types:list      Print list of all session entity types

      session-entity-types:delete    Delete specified SessionEntityType

      help                           Display more information on a specific command.

      version                        Display version information.
    ```

    ```
    PS C:\...DialogflowSamples> dotnet run entity-types:list --projectId "MY-PROJECT-ID"
    EntityType name: projects/rebecca-gcp/agent/entityTypes/9e399c95-10f4-48f3-a419-9d4aab7c6721
    EntityType display name: room
    Number of entities: 3
    Entity values:
    A
    B
    C
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
