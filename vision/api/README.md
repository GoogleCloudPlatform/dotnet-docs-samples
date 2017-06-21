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
    ```
    PS C:\...\dotnet-docs-samples\vision\api\QuickStart\bin\Debug> .\QuickStart.exe
    cat
    mammal
    whiskers
    small to medium sized cats
    cat like mammal
    ```

10. And run Detect.exe to detect various features in images:
    ```
    PS C:\...\dotnet-docs-samples\vision\api\Detect\bin\Debug> .\Detect.exe
    Detect 1.0.0.0
    Copyright c Google Inc 2017

    ERROR(S):
      No verb selected.

      labels         Detect labels.

      safe-search    Detect safe-search.

      properties     Detect properties.

      faces          Detect faces.

      text           Detect text.

      logos          Detect logos.

      landmarks      Detect landmarks.

      crop-hint      Detect crop hint in a local image file.

      web            Find web pages with matching images.

      doc-text       Detect text in a document image.

      help           Display more information on a specific command.

      version        Display version information.

    PS C:\...\dotnet-docs-samples\vision\api\Detect\bin\Debug> .\Detect text ..\..\..\VisionTest\d
    ata\bonito.gif
    Bonito: dried and
    fermented for months, but
    Still lively on your plate.

    Bonito:
    dried
    and
    fermented
    for
    months,
    but
    Still
    lively
    on
    your
    plate.
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
