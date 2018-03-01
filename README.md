# Google Cloud Platform .NET Docs Samples

# DO NOT MERGE

A collection of samples that demonstrate how to call some
Google Cloud services from C#.

The [APIs and .NET Libraries page](https://cloud.google.com/dotnet/docs/apis)
lists all the _Cloud_ APIs you can call from .NET.

The samples in this repo cover only _some_ of the total APIs that you can call from .NET.

### Build Status

| Windows | Linux |
|---------|-------|
|![Windows][windows-badge] | ![Linux][linux-badge] |

## Build and Run

To build and run a sample, open the solution file (.sln) with Visual
Studio 2015 or later and build it.

But no sample will work until you:

1.  **Create a project in the Google Cloud Platform Console**.
    If you haven't already created a project, create one now. Projects enable
    you to manage all Google Cloud Platform resources for your app, including
    deployment, access control, billing, and services.
    1.  Open the [Cloud Platform Console](https://console.cloud.google.com/).
    2.  In the drop-down menu at the top, select **Create a project**.
    3.  Click **Show advanced options**. Under App Engine location, select a
        United States location.
    4.  Give your project a name.
    5.  Make a note of the project ID, which might be different from the project
        name. The project ID is used in commands and in configurations.
    6.  In each sample directory, a README.md contains additional steps necessary 
        to run the sample. Don't forget to follow those instructions too.

2.  **Enable billing for your project**.
    If you haven't already enabled billing for your project,
    [enable billing now](https://console.cloud.google.com/project/_/settings).
    Enabling billing allows the application to consume billable resources such
    as running instances and storing data.

3.  **Download credentials**.
    1.  [Visit the APIs & services dashboard](https://console.cloud.google.com/apis/dashboard)
        and click **Credentials**.  
    2.  Click **Create credentials** and choose **Service Account key**.
    3.  Under Service account, choose **Compute Engine default service
        account**, and leave **JSON** selected under Key Type.  Click
        **Create**.  A .json file will be downloaded to your computer.
    4.  Set the environment variable `GOOGLE_APPLICATION_CREDENTIALS`
        to the path of the JSON key that was downloaded.  In powershell,
        the command will like something like this:
        
        ```ps1
        # For this powershell session.
        PS > $env:GOOGLE_APPLICATION_CREDENTIALS = "$env:USERPROFILE\Downloads\your-project-id-dea9fa230eae3.json"
        # For all processes created after this command.
        PS > [Environment]::SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "$env:USERPROFILE\Downloads\your-project-id-dea9fa230eae3.json", "User")
        ```

4.  **Enable APIs.**
    [Visit the APIs & services dashboard](https://console.cloud.google.com/apis/dashboard)
    and click **ENABLE API**.  Enable the APIs you plan to call.
    Are you going to use Cloud Datastore?
    Enable the Cloud Datastore API.  Each API must be enabled
    individually.


6.  **Download or clone this repo** with
    ```sh
    git clone https://github.com/GoogleCloudPlatform/dotnet-docs-samples
    ```
    Browse the directories, find a solution (.sln) file, open it with
    Visual Studio 2015 or later, and run it!


## Contributing changes

* See [CONTRIBUTING.md](CONTRIBUTING.md)

## Licensing

* See [LICENSE](LICENSE)

## Testing

* See [TESTING.md](TESTING.md)

[windows-badge]: https://www.googleapis.com/download/storage/v1/b/silver-python2-kokoro-badges/o/dotnet-docs-samples%2Fsystem_tests-windows.png?alt=media
[linux-badge]: https://www.googleapis.com/download/storage/v1/b/silver-python2-kokoro-badges/o/dotnet-docs-samples%2Fsystem_tests-linux.png?alt=media
