# Google Cloud Datastore Admin Sample

These samples demonstrate how to interact with the [Google Cloud Datastore Admin][Datastore] using C# and
the .NET client libraries to call the Datastore Admin API.

The samples requires [.NET Core 3.1][net-core] or later.  That means using
[Visual Studio 2019](https://www.visualstudio.com/), or the command line.

## Setup

1.  Set up a [.NET development environment](https://cloud.google.com/dotnet/docs/setup).

2.  Enable APIs for your project.
    [Click here][enable-api] to visit Cloud Platform Console and enable the Google Stackdriver Cloud Datastore API.

3. Set Environment Variables
   1. GOOGLE_CLOUD_PROJECT - Google Cloud Project Id
   2. CLOUD_STORAGE_BUCKET - Google Cloud Storage Bucket Name

4. Roles to be set in your Service Account and App Engine default service account:
    Datastore Import Export Admin, or Cloud Datastore Owner, or Owner Storage Admin, or Owner.

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)


[Datastore]: https://cloud.google.com/datastore/docs/
[enable-api]: https://console.cloud.google.com/flows/enableapi?apiid=datastore.googleapis.com&showconfirmation=true
[net-core]: https://www.microsoft.com/net/core
