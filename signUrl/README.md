# Google Cloud Signing URL Sample

This sample shows how to create Cloud CDN signed Url.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

2.  Follow the instructions here
    [https://cloud-dot-devsite.googleplex.com/cdn/docs/signed-urls#creatingkeys](https://cloud-dot-devsite.googleplex.com/cdn/docs/signed-urls#creatingkeys)
    to create the keys that will be used to sign the URLs, upload them and give relevant permissions.

3.  From a Powershell command line, run to see command line arguments.
    ```
   PS C:\dev\dotnet-docs-samples\signUrl\SignUrlSample> dotnet run
   Using launch settings from C:\dev\dotnet-docs-samples\signUrl\SignUrlSample\Properties\launchSettings.json...
   SignUrlSample 1.0.0
   Copyright (C) 2018 SignUrlSample
   ERROR(S):
   A required value not bound to option name is missing.

  --name          The name of the key to use when encrypting url

  --path          The path to the key to use when encrypting url

  -e              Url expiration in UTC formatted as: YYYY-MM-DDThh:mm:ss

  --help          Display this help screen.

  --version       Display version information.

  value pos. 0    Required. Url to sign
    ```
    
4.  Pass your own values:
    ```
    PS > dotnet run https://google.com --name my-key --path c:\key.orig -e 2019-05-17T20:15:50
    Signed URL: https://google.com?Expires=1558124150&KeyName=my-key&Signature=QBbSmT1iyLeiBpqca4wHXIdJHMY=
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
