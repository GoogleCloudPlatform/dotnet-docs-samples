# Google Cloud IAM Access Management Sample

This sample demonstrates how to manage user access using the Google Cloud IAM
APIs in C#.

To learn more, see
[Granting, Changing, and Revoking Access to Resources](https://cloud.google.com/iam/docs/granting-changing-revoking-access).

This sample requires [.NET Core 2.0](https://www.microsoft.com/net/core) or
later.  That means using [Visual Studio 2017](https://www.visualstudio.com/), 
or the command line.

## Links

- [Cloud IAM Docs](https://cloud.google.com/iam/docs/)

## Build and Run

1. **Follow the instructions in the [root README](../../../README.md)**.

1. Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=iam.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud IAM API.

1. Add a reference to the project in your application.

1. Call `GetIamPolicy()` and `SetIamPolicy()` to get and set policy for a
	resource.

## Contributing changes

- See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

- See [LICENSE](../../../LICENSE)