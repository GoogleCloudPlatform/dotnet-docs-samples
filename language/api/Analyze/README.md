# Google Cloud Natural Language API Sample

This C# sample demonstrates the use of the [Google Cloud Natural Language API][NL-Docs]
for sentiment, entity, and syntax analysis.

[NL-Docs]: https://cloud.google.com/natural-language/docs/

## Setup

Please follow the [Set Up Your Project](https://cloud.google.com/natural-language/docs/getting-started#set_up_your_project)
steps in the Quickstart doc to create a project and enable the
Cloud Natural Language API. Following those steps, make sure that you
[Set Up a Service Account](https://cloud.google.com/natural-language/docs/common/auth#set_up_a_service_account),
and export the following environment variable:

```
C:\>set GOOGLE_APPLICATION_CREDENTIALS=/path/to/your-project-credentials.json
```

## Run the sample

With Visual Studio 2013 or later, open Analyze.sln and build the solution.


The script will write to STDOUT the json returned from the API for the requested feature.

For example, if you run:

```
c:\> Analyze everything Santa Claus Conquers the Martians is a terrible movie. It's so bad, it's good.
```

You will see something like the following returned:

```
Language: en
Polarity: -1
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
