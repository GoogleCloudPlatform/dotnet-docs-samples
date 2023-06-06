# .NET Cloud DLP API Samples

A collection of samples that demonstrate how to call the
[Google Cloud DLP API](https://cloud.google.com/dlp/) from C#.

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=dlp.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Translation API.

3.  Set the environment variable `GOOGLE_PROJECT_ID` to your Google Cloud
    Project Id.

    ```ps1
    PS > $env:GOOGLE_PROJECT_ID = 'YOUR-GOOGLE-PROJECT-ID'
    ```

4. Set the `DLP_DIED_WRAPPED_KEY` environment variable to an AES-256 key encrypted ('wrapped') [with a Cloud Key Management Service (KMS) key](https://cloud.google.com/kms/docs/encrypt-decrypt).

5. Set the `DLP_DEID_KEY_NAME` environment variable to the path-name of the Cloud KMS key you wrapped `DLP_DEID_WRAPPED_KEY` with. 

6. Copy paste the below data into a CSV file and upload it to the Google Cloud Storage. Use this data for `inspect_gcs_with_sampling` code sample.
   ```
    Sr.No,Name,Mobile No,Email,Location
    1,Gary Jenkins,(425) 555-1212,gary.jenkins@gmail.com,USA
    2,David,(425) 555-1213,david@gmail.com,ENG
    3,Justin,(425) 555-1214,justin@gmail.com,UK
   ```
    - Set the environment variable `DLP_GCS_FILE_URI` obtained from above step.
        ```ps1
        PS > $env:DLP_GCS_FILE_URI = 'YOUR-GCS-FILE-URI'
        ```

7. [Create a Pub/Sub Topic](https://cloud.google.com/pubsub/docs/create-topic) and set the environment variable `DLP_TOPIC_ID` to your Pub/Sub Topic ID.

    ```ps1
    PS > $env:DLP_TOPIC_ID = 'YOUR-TOPIC-ID'
    ```

8.  [Create a Pub/Sub Subscription](https://cloud.google.com/pubsub/docs/create-subscription) using the Topic Id created in above step and set the environment variable `DLP_SUBSCRIPTION_ID` to your Pub/Sub Subscription ID.

    ```ps1
    PS > $env:DLP_SUBSCRIPTION_ID = 'YOUR-SUBSCRIPTION-ID'
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
