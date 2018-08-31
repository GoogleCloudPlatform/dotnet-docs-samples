# Google Stackdriver Monitoring Sample

This C# sample demonstrates how to manage Stackdriver Alerts using
[Google Stackdriver Monitoring API](https://cloud.google.com/monitoring/alerts/using-alerting-api) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/monitoring/api).

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=monitoring.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Stackdriver Monitoring API.


11. Run the AlertSample:
    ```
    PS C:\...\dotnet-docs-samples\monitoring\api\AlertSample> dotnet run
    AlertSample 1.0.0
    Copyright (C) 2018 AlertSample

    ERROR(S):
    No verb selected.

    list                List alert policies.

    backup              Save the current list of alert policies to a .json file.

    restore             Restore the list of alert policies from a .json file.

    replace-channels    Set the list of channel for an alert policy.

    enable              Enable alert policies.

    disable             Disable alert policies.

    help                Display more information on a specific command.

    version             Display version information.

    PS C:\...\dotnet-docs-samples\monitoring\api\AlertSample> dotnet run -- list -p my-project-id
    PS C:\...\dotnet-docs-samples\monitoring\api\AlertSample>
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)

## Testing

* See [TESTING.md](../../../TESTING.md)
