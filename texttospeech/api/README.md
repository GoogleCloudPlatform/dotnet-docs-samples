# .NET Cloud TextToSpeech API Samples

A collection of samples that demonstrate how to call the 
[Google Cloud TextToSpeech API](https://cloud.google.com/text-to-speech/) from C#.

This sample requires [.NET Core 2.0](https://www.microsoft.com/net/core) or
later.  That means using
[Visual Studio 2017](https://www.visualstudio.com/), or the command line.
## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=texttospeech.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud TextToSpeech API.

9.  From a Powershell command line, run the QuickStart sample:
    ```
    PS C:\...\dotnet-docs-samples\texttospeech\api\Quickstart> dotnet restore
    PS C:\...\dotnet-docs-samples\texttospeech\api\Quickstart> dotnet run
    # Creates 'sample.mp3' file in project folder
    ```

10. And run TextToSpeech to synthesize text to audio.
    ```
    PS C:\..\dotnet-docs-samples\translate\api\Translate> dotnet restore
    PS C:\..\dotnet-docs-samples\translate\api\Translate> dotnet run synthesize "This is synthetic speech."
    # Creates 'output.mp3' file in project folder
    PS C:\..\dotnet-docs-samples\translate\api\Translate> dotnet run
    TextToSpeech 1.0.0
    Copyright (C) 2018 TextToSpeech

    ERROR(S):
      No verb selected.

      list               List available voices.

      synthesize         Synthesize input to audio

      synthesize-file    Synthesize a file to audio

      help               Display more information on a specific command.

      version            Display version information.
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)