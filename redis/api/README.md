# .NET Cloud Memorystore for Redis Samples

A collection of samples that demonstrate how to call the
[Google Cloud Memorystore for Redis API](https://cloud.google.com/memorystore/docs/redis) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=redis.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Redis API.

3.  Edit `Redis\Redis.cs`, and replace YOUR-PROJECT-ID with id
    of the project you created in step 1. Also, update "LOCATION-ID"
    with the location id.

4.  From a Powershell command line, run the CreateInstance to create instance:
    ```
    PS > dotnet run create instance-name
    ```

5. And run the Redis sample to see a list of subcommands:
    ```
    PS C:\> dotnet run command

    where command is one of

      create              Create a Redis instance based on the specified location.

      get                 Get the details of a specific Redis instance.

      list                List all Redis instances owned by a project in with the specified location (region).

      update              Updates the metadata and configuration of a specific Redis instance.

      delete              Delete a specific Redis instance.
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
