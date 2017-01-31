# Google Cloud Speech API Samples

These samples show how to use the [Google Cloud Speech API](http://cloud.google.com/speech)
to transcribe audio files, using the [Google API Client Library for
.NET](https://developers.google.com/api-client-library/dotnet/).

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=speech.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Speech API.

6.  Open [Speech.sln](Speech.sln) with Microsoft Visual Studio version 2012 or later.

7.  Build the Solution.

8.  From the command line, run QuickStart.exe:
    ```
    PS C:\...\dotnet-docs-samples\speech\api> cd .\QuickStart\bin\Debug\
    PS C:\...\dotnet-docs-samples\speech\api\QuickStart\bin\Debug> .\QuickStart.exe
    how old is the Brooklyn Bridge
    ```
    
9.  And run Recognize for more examples:
    ```
    PS C:\...\dotnet-docs-samples\speech\api\Recognize\bin\Debug> .\Recognize.exe
    Recognize 1.0.0.0
    Copyright c Google Inc 2017
    
    ERROR(S):
      No verb selected.
    
      sync       Detects speech in an audio file.
    
      async      Creates a job to detect speech in an audio file, and waits for the job to complete.
    
      stream     Detects speech in an audio file by streaming it to the Speech API.
    
      listen     Detects speech in a microphone input stream.
    
      help       Display more information on a specific command.
    
      version    Display version information.
    
    PS C:\...\dotnet-docs-samples\speech\api\Recognize\bin\Debug> .\Recognize.exe listen 3
    Speak now.
    test
    testing
    testing one
    testing
     one
    testing
     one two
    testing one
     two
    testing
     1 2 3
    testing 1 2 3
    PS C:\...\dotnet-docs-samples\speech\api\Recognize\bin\Debug>
    ```
