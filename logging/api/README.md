# Stackdriver Logging Sample

A sample demonstrating how to invoke Google Cloud Stackdriver Logging from C#.

## Links

- [Cloud Logging Reference Docs](https://cloud.google.com/logging/docs/)

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

2.  Open [LoggingSample.sln](LoggingSample.sln) with Microsoft Visual Studio version 2012 or later.

3.  Edit `Program.cs`, and replace YOUR-PROJECT-ID with id
    of the project you created in step 1.

4.  Build the Solution.

5.  From the command line, run LoggingSample.exe to see a list of
    subcommands:

    ```sh
    C:\...\bin\Debug> LoggingSample.exe
    Usage:
LoggingSample create-log-entry log-id new-log-entry-text
LoggingSample list-log-entries log-id
LoggingSample create-sink sink-id log-id
LoggingSample list-sinks
LoggingSample update-sink sink-id log-id
LoggingSample delete-log log-id
LoggingSample delete-sink sink-id
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
