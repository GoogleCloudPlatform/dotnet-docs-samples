# Stackdriver Error Reporting Sample

A sample demonstrating how to invoke Stackdriver Error Reporting from C#.

## Links

- [Stackdriver Error Reporting API Reference Docs](https://cloud.google.com/error-reporting/reference/rest/)

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=clouderrorreporting.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Stackdriver Error Reporting API.

6.  Open [ErrorReporting.sln](ErrorReporting.sln) with Microsoft Visual Studio version 2012 or later.

7.  Build the Solution.

8.  From the command line, set environment variables and run:

    ```sh
    C:\...\bin\Debug> set GOOGLE_PROJECT_ID=your project id displayed on the Google Developers Console.
    C:\...\bin\Debug> ErrorReporting
    ```

9.  View your error in the [Cloud Console UI](https://console.cloud.google.com/errors)
	
## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
