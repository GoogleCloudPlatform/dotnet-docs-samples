## Authentication Sample

A sample demonstrating how to authenticate with Google's APIs using Application Default Credentials
[Application Default Credentials](https://developers.google.com/identity/protocols/application-default-credentials)
from C#. The sample authenticates and then makes an API request to the Cloud Storage API.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/auth).

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

3.  **Install the Google Cloud SDK**.
    If you haven't already installed the Google Cloud SDK, [install and
    initialize the Google Cloud SDK](https://cloud.google.com/sdk/docs/) now.
    The SDK contains tools and libraries that enable you to create and manage
    resources on Google Cloud Platform.

4.  **Enable APIs for your project**.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=storage_api&showconfirmation=true)
    to visit Cloud Platform Console and enable the Cloud Storage API.

5.  **Create a Cloud Storage bucket for your project**.
    Cloud Storage allows you to store and serve binary data.
    A bucket is a high-level container for binary objects.
    At a Powershell  prompt, type:

    ```ps1
    PS > gsutil mb gs://<your-project-id>
    PS > gsutil defacl set public-read gs://<your-project-id>
    ```

6.  ```ps1
    PS > $env:GOOGLE_PROJECT_ID="your project id displayed on the Google Developers Console."
    PS > dotnet restore
    PS > dotnet run
    AuthSample 1.0.0
    Copyright (C) 2017 AuthSample

    ERROR(S):
    No verb selected.

    cloud      Authenticate using the Google.Cloud.Storage library.  The preferred way of authenticating.

    api        Authenticate using the Google.Apis.Storage library.

    http       Authenticate using and make a rest HTTP call.

    help       Display more information on a specific command.

    version    Display version information.

    PS > dotnet run -- cloud
    your-bucket1
    your-bucket2
    ...
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
