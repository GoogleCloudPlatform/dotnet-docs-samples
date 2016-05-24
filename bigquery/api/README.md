# BigQuery Sample

A sample demonstrating how to invoke BigQuery from C#.

## Links

- [What is BigQuery?](https://cloud.google.com/bigquery/what-is-bigquery)
- [BigQuery Reference Docs](https://developers.google.com/api-client-library/dotnet/apis/bigquery/v2)

## Build and Run

1.  **Create a project in the Google Cloud Platform Console**.
    If you haven't already created a project, create one now. Projects enable
    you to manage all Google Cloud Platform resources for your app, including
    deployment, access control, billing, and services.
    1.  Open the [Cloud Platform Console](https://console.cloud.google.com/).
    2.  In the drop-down menu at the top, select Create a project.
    3.  Click Show advanced options. Under App Engine location, select a
        United States location.
    4.  Give your project a name.
    5.  Make a note of the project ID, which might be different from the project
        name. The project ID is used in commands and in configurations.

2.  **Enable billing for your project**.
    If you haven't already enabled billing for your project,
    [enable billing now](https://console.cloud.google.com/project/_/settings).
    Enabling billing allows the application to consume billable resources such
    as running instances and storing data.

3.  **Install the Google Cloud SDK**.
    If you haven't already installed the Google Cloud SDK, [install and
    initialize the Google Cloud SDK](https://cloud.google.com/sdk/docs/) now.
    The SDK contains tools and libraries that enable you to create and manage
    resources on Google Cloud Platform.

4.  **Enable APIs for your project**.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=bigquery&showconfirmation=true)
    to visit Cloud Platform Console and enable the BigQuery API.

5.  Download or clone this repo with

    ```sh
    git clone https://github.com/GoogleCloudPlatform/dotnet-docs-samples
    ```

6.  Open [GettingStarted/BigquerySample.sln](GettingStarted/BigquerySample.sln) with
    Microsoft Visual Studio version 2012 or later.
7.  Build the Solution.
8.  Run it from the command line with your project id:

    ```sh
    C:\...\bin\Debug> BigquerySample my_project_id
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
