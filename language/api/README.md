# Google Natural Language API Sample

This C# sample demonstrates the use of the [Google Cloud Natural Language API][NL-Docs]
for sentiment, entity, and syntax analysis.

[NL-Docs]: https://cloud.google.com/natural-language/docs/

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=language.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Natural Language API.

6.  Open the solution file "Analyze.sln" with Microsoft Visual Studio version 2015 or later:
    * Google.Cloud.Language.V1 - [Analyze.sln](v1/Analyze/Analyze.sln)
    * Google.Cloud.Language.V1.Experimental - [Analyze.sln](v1Beta2/Analyze/Analyze.sln)

7.  Build the Solution.

8.  Run.

    The script will write to STDOUT the json returned from the API for the requested feature.
    
    For example, if you run:
    
    ```
    c:\> Analyze everything Santa Claus Conquers the Martians is a terrible movie. It's so bad, it's good.
    ```
    
    You will see something like the following returned:
    
    ```
    Overall document sentiment:
            Score: -0.4
            Magnitude: 0.9
    Sentence level sentiment:
            Santa Claus Conquers the Martians is a terrible movie.: (-0.7)
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
            Name: Santa Claus Conquers the Martians
            Type: WorkOfArt
            Salience: 0.9728492
            Mentions:
                    0: Santa Claus Conquers the Martians
                    48: movie
            Metadata:
                    mid: /m/0122r8
                    wikipedia_url: http://en.wikipedia.org/wiki/Santa_Claus_Conquers_the_Martians
            Name: good
            Type: Other
            Salience: 0.02715084
            Mentions:
                    71: good
            Metadata:
    ```

	
## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
