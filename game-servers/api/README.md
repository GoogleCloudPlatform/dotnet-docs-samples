# .NET Cloud Vision Samples

A collection of samples that demonstrate how to call the
[Google Cloud Vision API](https://cloud.google.com/vision/docs/) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/vision/api).

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=vision.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Vision API.

9.  From a Powershell command line, run the QuickStart sample:
    ```
    PS C:\...\dotnet-docs-samples\vision\api\QuickStart> dotnet run
    cat
    mammal
    whiskers
    small to medium sized cats
    cat like mammal
    ```

10. And run the Detect sample to detect various features in images:
    ```
    PS C:\...\dotnet-docs-samples\vision\api\Detect> dotnet run
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

    PS C:\...\dotnet-docs-samples\vision\api\Detect> dotnet run text ..\VisionTest\data\bonito.gif
    Bonito: dried and
    fermented for months, but
    still ively on your plate.

    Bonito:
    dried
    and
    fermented
    for
    months,
    but
    still
    ively
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
