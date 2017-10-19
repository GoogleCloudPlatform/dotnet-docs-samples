# Google Natural Language API Sample

This C# sample demonstrates the use of the [Google Cloud Natural Language API][NL-Docs]
for sentiment, entity, and syntax analysis.

[NL-Docs]: https://cloud.google.com/natural-language/docs/

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/language/api).

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=language.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Natural Language API.

7.  From a Powershell command line, run the QuickStart sample:
    ```
    PS C:\...\dotnet-docs-samples\language\api\QuickStart> dotnet restore
    PS C:\...\dotnet-docs-samples\language\api\QuickStart> dotnet run
    Score: 0.2
    Magnitude: 0.2
    ```

8.  And run the Analyze sample to see a list of possible commands:
    ```
    PS C:\...\dotnet-docs-samples\language\api\Analyze> dotnet restore
    PS C:\...\dotnet-docs-samples\language\api\Analyze> dotnet run

    Usage:
    C:\> dotnet run command text
    C:\> dotnet run command gs://bucketName/objectName

    Where command is one of
        entities
        sentiment
        syntax
        entity-sentiment
        everything
    ```

    Each command will write to STDOUT the json returned from the API for the requested feature.

    For example, if you run:

    ```
    c:\> dotnet run everything Santa Claus Conquers the Martians is a terrible movie. It's so bad, it's good.
    ```

    You will see something like the following returned:

    ```
    Language: en
    Overall document sentiment:
            Score: -0.5
            Magnitude: 1
    Sentence level sentiment:
            Santa Claus Conquers the Martians is a terrible movie.: (-0.8)
            Its so bad, its good.: (-0.1)
    Sentences:
            0: Santa Claus Conquers the Martians is a terrible movie.
            55: Its so bad, its good.
    Tokens:
            Noun Santa
            Noun Claus
            Verb Conquers
            Det the
            Noun Martians
            Verb is
            Det a
            Adj terrible
            Noun movie
            Punct .
            Pron Its
            Adv so
            Adj bad
            Punct ,
            Pron its
            Noun good
            Punct .
    Entities:
            Name: movie
            Type: WorkOfArt
            Salience: 0.4999807
            Mentions:
                    48: movie
            Metadata:
            Name: Santa Claus Conquers the Martians
            Type: WorkOfArt
            Salience: 0.405366
            Mentions:
                    0: Santa Claus Conquers the Martians
            Metadata:
                    mid: /m/0122r8
                    wikipedia_url: https://en.wikipedia.org/wiki/Santa_Claus_Conquers_the_Martians
            Name: good
            Type: Other
            Salience: 0.09465333
            Mentions:
                    71: good
            Metadata:
    Entity Sentiment:
            movie (49%)
                    Score: -0.9
                    Magnitude 0.9
            Santa Claus Conquers the Martians (40%)
                    Score: -0.9
                    Magnitude 0.9
            good (9%)
                    Score: 0
                    Magnitude 0
    ```


## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
