# .NET Cloud Video Intelligence Samples

A collection of samples that demonstrate how to call the
[Google Cloud Video Intelligence API](https://cloud.google.com/video-intelligence/docs/) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/video/api).

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=videointelligence.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Video Intelligence API.

9.  From a Powershell command line, run the QuickStart sample:
    ```
    PS C:\...\dotnet-docs-samples\video\api\QuickStart> dotnet restore
    PS C:\...\dotnet-docs-samples\video\api\QuickStart> dotnet run
    animal
    cat
    cat like mammal
    domestic long haired cat
    kitten
    maine coon
    nature
    pet
    small to medium sized cats
    tabby cat
    whiskers
    ```

10. And run Analyze to analyze videos:
    ```
    PS C:\...\dotnet-docs-samples\video\api\Analyze> dotnet restore
    PS C:\...\dotnet-docs-samples\video\api\Analyze> dotnet run
    Analyze 1.0.0
    Copyright (C) 2017 Analyze

    ERROR(S):
    No verb selected.

    shots         Print a list shot changes.

    safesearch    Analyze the content of the video.

    labels        Print a list of labels found in the video.

    help          Display more information on a specific command.

    version       Display version information.
    ```

    ```
    PS C:\...Analyze> dotnet run shots "gs://cloudmleap/video/next/fox-snatched.mp4"
    Start Time Offset: 0    End Time Offset: 963550
    Start Time Offset: 998320       End Time Offset: 5971534
    Start Time Offset: 6006304      End Time Offset: 6969854
    Start Time Offset: 7004624      End Time Offset: 8002944
    ...
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
