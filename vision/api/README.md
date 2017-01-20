# .NET Cloud Vision Samples

A collection of samples that demonstrate how to call the 
[Google Cloud Vision API](https://cloud.google.com/vision/docs/) from C#.

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=vision.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Speech API.

6.  Open [Vision.sln](Vision.sln) with Microsoft Visual Studio version 2012 or later.

8.  Build the Solution.

9.  From the command line, run QuickStart.exe:
    ```sh
    PS C:\...\dotnet-docs-samples\vision\api\QuickStart\bin\Debug> .\QuickStart.exe
    cat
    mammal
    whiskers
    small to medium sized cats
    cat like mammal
    ```

10. And run Detect.exe to detect various features in images:
    '''sh
	PS C:\...\dotnet-docs-samples\vision\api\Detect\bin\Debug> .\Detect.exe
	Detect 1.0.0.0
	Copyright c Google Inc 2017

	ERROR(S):
	  No verb selected.

	  labels             Detect labels in a local image file.

	  labels-gcs         Detect labels in an image stored in Google Cloud Storage.

	  safe-search        Detect safe-search in a local image file.

	  safe-search-gcs    Detect safe-search in an image stored in Google Cloud Storage.

	  properties         Detect properties in a local image file.

	  properties-gcs     Detect properties in an image stored in Google Cloud Storage.

	  faces              Detect faces in a local image file.

	  faces-gcs          Detect faces in an image stored in Google Cloud Storage.

	  text               Detect text in a local image file.

	  text-gcs           Detect text in an image stored in Google Cloud Storage.

	  logos              Detect logos in a local image file.

	  logos-gcs          Detect logos in an image stored in Google Cloud Storage.

	  landmarks          Detect landmarks in a local image file.

	  landmarks-gcs      Detect landmarks in an image stored in Google Cloud Storage.

	  help               Display more information on a specific command.

	  version            Display version information.

	PS C:\...\dotnet-docs-samples\vision\api\Detect\bin\Debug> .\Detect text ..\..\..\VisionTest\d
	ata\bonito.gif
	Bonito: dried and
	fermented for months, but
	Still lively on your plate.
    '''

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
