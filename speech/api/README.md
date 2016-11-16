# Google Cloud Speech REST API Samples

These samples show how to use the [Google Cloud Speech API](http://cloud.google.com/speech)
to transcribe audio files, using the REST-based [Google API Client Library for
.NET](https://developers.google.com/api-client-library/dotnet/).

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=speech.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Speech API.

6.  Open [Speech.sln](Speech.sln) with Microsoft Visual Studio version 2012 or later.

7.  Build the Solution.

8.  Run the sample

Each of the samples takes the audio file to transcribe as the first argument.
For example:

```ps1
PS > .\Transcribe\bin\Debug\Transcribe.exe .\resources\audio.raw

PS > .\TranscribeAsync\bin\Debug\TranscribeAsync.exe .\resources\audio.raw
```

You should see a response with the transcription result.