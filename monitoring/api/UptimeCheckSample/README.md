# Google Stackdriver Monitoring Sample

This C# sample demonstrates how to manage uptime checks with the 
[Google Stackdriver Monitoring API](https://cloud.google.com/monitoring/uptime-checks/) from C#.

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

4.  Run the sample:
    ```
    PS C:\...\dotnet-docs-samples\monitoring\api\UptimeCheckSample> dotnet run
    UptimeCheck 1.0.0
    Copyright (C) 2018 UptimeCheck

    ERROR(S):
    No verb selected.

    create      Create an uptime check.

    delete      Delete an uptime check.

    list        List metric descriptors for this project.

    list-ips    List IP addresses of uptime-check servers.

    get         Get details for an uptime check.

    update      Update an uptime check.

    help        Display more information on a specific command.

    version     Display version information.

    PS C:\...\dotnet-docs-samples\monitoring\api\UptimeCheckSample> dotnet run -- list -p my-project-id
    PS C:\...\dotnet-docs-samples\monitoring\api\UptimeCheckSample> dotnet run -- create -p my-project-id
    projects/my-project-id/uptimeCheckConfigs/new-uptime-check-3
    PS C:\...\dotnet-docs-samples\monitoring\api\UptimeCheckSample> dotnet run -- get projects/my-project-id/uptimeCheckConfigs/new-uptime-check-3
    Name: projects/my-project-id/uptimeCheckConfigs/new-uptime-check-3
    Display Name: New uptime check
    Http Path: /
    PS C:\...\dotnet-docs-samples\monitoring\api\UptimeCheckSample>
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)

## Testing

* See [TESTING.md](../../../TESTING.md)
