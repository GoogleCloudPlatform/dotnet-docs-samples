# .NET Cloud Translation API Samples

A collection of samples that demonstrate how to call the 
[Google Cloud Translation API](https://cloud.google.com/translate/) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/translate/api).

## Build and Run

1.  **Follow the set-up instructions in the [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=translate.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Translation API.

9.  From a Powershell command line, run the QuickStart sample:
    ```
    PS C:\...\dotnet-docs-samples\monitoring\api\Quickstart> dotnet restore
    PS C:\...\dotnet-docs-samples\monitoring\api\Quickstart> dotnet run
	Привет мир.
	```

10. And run Translate to translate text.
    ```
	PS C:\..\dotnet-docs-samples\translate\api\Translate> dotnet restore
	PS C:\..\dotnet-docs-samples\translate\api\Translate> dotnet run translate "Old shoes."
	Старые туфли.
	PS C:\..\dotnet-docs-samples\translate\api\Translate> dotnet run
	Translate 1.0.0.0
	Copyright ©  2017

	ERROR(S):
	  No verb selected.

	  translate    Translate text.

	  list         List available languages.

	  detect       Detects which language some text is written in.

	  help         Display more information on a specific command.

	  version      Display version information.

	```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
