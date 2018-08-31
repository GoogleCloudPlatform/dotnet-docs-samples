# BigQuery Sample

A sample demonstrating how to invoke BigQuery from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/bigquery/api).

## Links

- [What is BigQuery?](https://cloud.google.com/bigquery/what-is-bigquery)
- [BigQuery Reference Docs](https://developers.google.com/api-client-library/dotnet/apis/bigquery/v2)

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=bigquery&showconfirmation=true)
    to visit Cloud Platform Console and enable the BigQuery API.

5. Edit [QuickStart.cs](QuickStart/Program.cs), and replace YOUR-PROJECT-ID with the id of the project you created in step 1.

6.  From a Powershell command line, run the QuickStart sample:

    ```ps1
    PS C:\...\dotnet-docs-samples\bigquery\api\QuickStart> dotnet run
    Dataset my_new_dataset created.
    ```

8.  And run the Bigquery sample with your project id to perform a sample query:

    ```ps1
    PS C:\...\dotnet-docs-samples\bigquery\api\BigquerySample> dotnet run YOUR-PROJECT-ID
    
    Query Results:
    ------------
    hamlet: 5318
    kinghenryv: 5104
    cymbeline: 4875
    troilusandcressida: 4795
    kinglear: 4784
    kingrichardiii: 4713
    2kinghenryvi: 4683
    coriolanus: 4653
    2kinghenryiv: 4605
    antonyandcleopatra: 4582
    1kinghenryvi: 4441
    winterstale: 4404
    1kinghenryiv: 4317
    othello: 4284
    kinghenryviii: 4236
    loveslabourslost: 4236
    romeoandjuliet: 4230
    kingrichardii: 4200
    3kinghenryvi: 4076
    kingjohn: 4024
    rapeoflucrece: 3960
    allswellthatendswell: 3949
    titusandronicus: 3869
    macbeth: 3833
    timonofathens: 3795
    measureforemeasure: 3786
    periclesprinceoftyre: 3754
    tamingoftheshrew: 3699
    sonnets: 3677
    merchantofvenice: 3677
    asyoulikeit: 3676
    merrywivesofwindsor: 3653
    tempest: 3636
    twelfthnight: 3534
    midsummersnightsdream: 3406
    muchadoaboutnothing: 3369
    juliuscaesar: 3361
    twogentlemenofverona: 3122
    venusandadonis: 2926
    comedyoferrors: 2870
    various: 1349
    loverscomplaint: 1195

    Press any key...    
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
