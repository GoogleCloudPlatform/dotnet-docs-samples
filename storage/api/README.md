# Google Cloud Storage Sample

Sample code to issue several basic Google Cloud Store (GCS) operations
using the Google Client Libraries.
For more information, see documentation for Compute Storage .NET client
https://cloud.google.com/storage/docs/json_api/v1/json-api-dotnet-samples

See our other [Google Cloud Platform github
repos](https://github.com/GoogleCloudPlatform) for sample applications and
scaffolding for other frameworks and use cases.

## Build and Run
0.  On your Windows development machine,
1.  In the [Google Developers Console](https://console.developers.google.com/),
    create a new project or choose an existing project.
2.  In the [Google Developers Console](https://console.developers.google.com/),
    click **APIs & auth**, then click APIs.  Wait for a list of APIs to
    appear, then click Cloud Storage.  If Cloud Storage is not already enabled,
    click the Enable API button.
3.  In the [Google Developers Console](https://console.developers.google.com/),
    under **APIs & auth**, click Credentials.  Click the button to "Generate
    a new JSON key."  Set the environment variable
    `GOOGLE_APPLICATION_CREDENTIALS` to the path of the JSON key you
    downloaded.
3.  Clone this repo with

    ```sh
    git clone https://github.com/GoogleCloudPlatform/csharp-docs-samples
    ```
4.  Open csharp-docs-samples/cloudstorage/cloudstorage.sln with
    Microsoft Visual Studio version 2012 or later.
5.  Build the Solution.
6.  Run it locally by pressing F5 or choosing "Debug -> Start Debugging" from
    Microsoft Visual Studio's Menu.


## Contributing changes

* See [CONTRIBUTING.md](../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../LICENSE)
