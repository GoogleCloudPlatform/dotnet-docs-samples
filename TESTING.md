# Testing

## Running tests
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
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=bigquery,storage_api&showconfirmation=true)
    to visit Cloud Platform Console and enable the APIs.

5.  Download or clone this repo with

    ```sh
    git clone https://github.com/GoogleCloudPlatform/dotnet-docs-samples
    ```

6.  Set the environment variables:
 - GOOGLE_PROJECT_ID = your project id displayed on the Google Developers Console.
 - GOOGLE_BUCKET = the name of the Google Cloud Storage bucket you created.

7.  Add the following tools to your path:
 - MSBuild
 - MSTest
 - Nuget


8.  Run the test script:

    ```
    C:\...\dotnet-docs-samples> powershell .\buildAndRunTests.ps1
    ```