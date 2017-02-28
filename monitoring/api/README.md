# Google Stackdriver Monitoring Sample

This C# sample demonstrates how to call the
[Google Stackdriver Monitoring API](https://cloud.google.com/monitoring/docs) from C#.

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=monitoring.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Stackdriver Monitoring API.

6.  Open [Monitoring.sln](Monitoring.sln) with Microsoft Visual Studio version 2012 or later.

7. Edit [QuickStart.cs](QuickStart/QuickStart.cs), and replace YOUR-PROJECT-ID with the id of the project you created in step 1.

8.  Build the Solution.

9.  From the command line, run QuickStart.exe:
    ```
    PS C:\...\dotnet-docs-samples\monitoring\api\Quickstart\bin\Debug> .\QuickStart.exe
    Done writing time series data.
    ```

10. And run Monitoring.exe to see a list of subcommands:
    ```
    PS C:\...dotnet-docs-samples\monitoring\api\bin\Debug> .\Monitoring.exe
    Monitoring 1.0.0.0
    Copyright c Google Inc 2017

    ERROR(S):
      No verb selected.

      create           Create a metric descriptor for this project.

      list             List metric descriptors for this project.

      get              Get a metric descriptor's details.

      write            Write TimeSeries data to custom metric.

      read             Read TimeSeries data from a metric.

      readFields       Reads headers of time series data.

      readAggregate    Aggregates time series data that matches the specified metric type.

      readReduce       Reduces time series data that matches the specified metric type.

      delete           Delete a metric descriptor for this project.

      listResources    List monitored resources for this project.

      getResource      Get a monitored resource descriptor.

      listGroups       List metric groups for this project.

      help             Display more information on a specific command.

      version          Display version information.
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
