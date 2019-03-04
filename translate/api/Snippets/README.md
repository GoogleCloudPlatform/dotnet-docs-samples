# .NET Cloud Translation API Samples

A collection of samples that demonstrate how to call the 
[Google Cloud Translation API](https://cloud.google.com/translate/) from C#.

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=translate.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Translation API.

9.  Run the test:
    ```
    PS C:\...\dotnet-docs-samples\translate\canon> dotnet test
    Build started, please wait...
    Build completed.

    Test run for C:\Users\rennie\gitrepos\dotnet-docs-samples\translate\canon\bin\Debug\netcoreapp2.1\Translate.dll(.NETCoreApp,Version=v2.1)
    Microsoft (R) Test Execution Command Line Tool Version 15.8.0
    Copyright (c) Microsoft Corporation.  All rights reserved.

    Starting test execution, please wait...

    Total tests: 5. Passed: 5. Failed: 0. Skipped: 0.
    Test Run Successful.
    Test execution time: 2.6078 Seconds    
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
