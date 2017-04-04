# .NET Cloud Translation API Samples

A collection of samples that demonstrate how to call the 
[Google Cloud Translation API](https://cloud.google.com/translate/) from C#.

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=translate.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Translation API.

6.  Open [Translate.sln](Translate.sln) with Microsoft Visual Studio version 2012 or later.

8.  Build the Solution.

9.  From the command line, run QuickStart.exe:
    ```
	PS C:\..\dotnet-docs-samples\translate\api\QuickStart\bin\Debug> .\QuickStart.exe
	Привет мир.
	```

10. And run Translate to translate text.
    ```
	PS C:\..\dotnet-docs-samples\translate\api\Translate\bin\Debug> .\Translate.exe translate "Old shoes."
	Старые ботинки.
	PS C:\..\dotnet-docs-samples\translate\api\Translate\bin\Debug> .\Translate.exe
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
