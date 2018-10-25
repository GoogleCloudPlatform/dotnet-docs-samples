# Stackdriver Logging Sample

A sample that demonstrates how to write logs to
[Stackdriver](https://cloud.google.com/logging/)
from an ASP.NET core application.

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=logging.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Translation API.

3.  Edit [`Startup.cs`](./Startup.cs) and replace `YOUR-PROJECT-ID` with the
    name of your Google Cloud Project id.

4.  Run the sample:
    ```
    PS C:\...\dotnet-docs-samples\logging\api\AspNetCore> dotnet run
    Hosting environment: Production
    Content root path: C:\...\dotnet-docs-samples\logging\api\AspNetCore
    Now listening on: http://localhost:5000
    Now listening on: https://localhost:5001
    Application started. Press Ctrl+C to shut down.
	```

5.  View your logs in [Google Cloud Console](https://console.cloud.google.com/logs/viewer?resource=global).

## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)

## Testing

* See [TESTING.md](../../../TESTING.md)
