# .NET Cloud DLP API Samples

A collection of samples that demonstrate how to call the
[Google Cloud DLP API](https://cloud.google.com/dlp/) from C#.

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=dlp.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Translation API.

5.  Set the environment variable `GOOGLE_PROJECT_ID` to your Google Cloud
    Project Id.

    ```ps1
    PS > $env:GOOGLE_PROJECT_ID = 'YOUR-GOOGLE-PROJECT-ID'
    ```

9.  Run the test:
    ```ps1
    PS >  dotnet test
    Build started, please wait...
    Build completed.

    Test run for /usr/local/google/home/rennie/gitrepos/dds2/dlp/api/Snippets/bin/Debug/netcoreapp2.1/DlpSnippets.dll(.NETCoreApp,Version=v2.1)
    Microsoft (R) Test Execution Command Line Tool Version 15.7.0
    Copyright (c) Microsoft Corporation.  All rights reserved.

    Starting test execution, please wait...
    Deidentified content: My SSN is *****9127.

    Total tests: 1. Passed: 1. Failed: 0. Skipped: 0.
    Test Run Successful.
    Test execution time: 2.7392 Seconds
    ```

## Designed to demonstrate Google APIs

These code samples are designed to demonstrate simple code that calls Google
APIs.  They're optimized for being embedded into documentation on
[https://cloud.google.com/docs](https://cloud.google.com/docs).
So sometimes, they don't conform to engineering best practices like
"don't copy and paste code."

## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)

## Testing

* See [TESTING.md](../../../TESTING.md)
