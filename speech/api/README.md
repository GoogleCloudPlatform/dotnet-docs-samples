# Google Cloud Speech REST API Samples

These samples show how to use the [Google Cloud Speech API](http://cloud.google.com/speech)
to transcribe audio files, using the REST-based [Google API Client Library for
.NET](https://developers.google.com/api-client-library/dotnet/).

## Prerequisites

### [Visual Studio 2013](https://www.visualstudio.com/) or later.

### Enable the Speech API

If you have not already done so, [enable the Google Cloud Speech
API][console-speech] for your project.

[console-speech]: https://console.cloud.google.com/apis/api/speech.googleapis.com/overview?project=_

### Authentication

These samples use service accounts for authentication.

* Visit the [Cloud Console][cloud-console], and navigate to:

    `API Manager > Credentials > Create credentials > Service account key > New
    service account`.
* Create a new service account, and download the json credentials file.
* Set the `GOOGLE_APPLICATION_CREDENTIALS` environment variable to point to your
  downloaded service account credentials:

      > set GOOGLE_APPLICATION_CREDENTIALS=/path/to/your/credentials-key.json

  If you do not do this, the REST api will return a 403.

See the [Cloud Platform Auth Guide][auth-guide] for more information.

[cloud-console]: https://console.cloud.google.com
[auth-guide]: https://cloud.google.com/docs/authentication#developer_workflow

### Setup

* [Download this repo](https://github.com/GoogleCloudPlatform/dotnet-docs-samples/archive/master.zip).

* Open `speech\api\Speech.sln` with Visual Studio.
  Build the solution.

## Run the sample

Each of the samples takes the audio file to transcribe as the first argument.
For example:

```sh
> .\Transcribe\bin\Debug\Transcribe.exe .\resources\audio.raw
```

You should see a response with the transcription result.