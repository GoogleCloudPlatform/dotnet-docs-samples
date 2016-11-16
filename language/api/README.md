# Google Natural Language API Sample

This C# sample demonstrates the use of the [Google Cloud Natural Language API][NL-Docs]
for sentiment, entity, and syntax analysis.

[NL-Docs]: https://cloud.google.com/natural-language/docs/

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=language.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Natural Language API.

6.  Open [Analyze.sln](Analyze/Analyze.sln) with Microsoft Visual Studio
    version 2015 or later.

7.  Build the Solution.

8.  Run.

    The script will write to STDOUT the json returned from the API for the requested feature.
    
    For example, if you run:
    
    ```
    c:\> Analyze everything Santa Claus Conquers the Martians is a terrible movie. It's so bad, it's good.
    ```
    
    You will see something like the following returned:
    
    ```
    Language: en
    Score: -1
    Magnitude: 1.6
    Sentences:
            0: Santa Claus Conquers the Martians is a terrible movie.
            55: It's so bad, it's good.
    Entities:
            Name: Santa Claus Conquers the Martians
            Type: ORGANIZATION
            Salience: 0.5174748
            Mentions:
                    0: Santa Claus Conquers the Martians
            Metadata:
                    wikipedia_url: http://en.wikipedia.org/wiki/Santa_Claus_Conquers_the_Martians
    ```

	
## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
