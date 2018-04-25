# .NET Cloud Spanner Sample Leaderboard Application

A leaderboard sample that uses the Spanner commit timestamp feature and demonstrates
how to call the [Google Cloud Spanner API](https://cloud.google.com/spanner/docs/)
from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.

This sample includes extra directories step1, step2, step3, step4 and step5 that contain partial versions of this sample app. These directories are intended to provide guidance as part of a separate Codelab walk-through where the application is built in the following stages:

* step1 - Create the sample database along with the tables Players and Scores.
* step2 - Populate the Players tables with sample data.
* step3 - Populate the Scores table with sample data.
* step4 - Run sample queries including sorting the results by timestamp.
* step5 - Delete the sample database.

If you only want to run the complete sample refer to the application in the Leaderboard directory.


## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=spanner.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Spanner API.

10. From a Powershell command line, run the Spanner Leaderboard sample to see a list of subcommands:
    ```
    PS C:\...\dotnet-docs-samples\applications\leaderboard\Leaderboard> dotnet run
    Leaderboard 1.0.0
    Copyright (C) 2018 Leaderboard

    ERROR(S):
    No verb selected.

    createSampleDatabase       Create a sample Cloud Spanner database along with sample tables in your project.

    insertPlayers              Insert 100 sample Player records into the database.

    insertScores               Insert sample score data into Scores sample Cloud Spanner database table.

    queryTopTenAllTime         Query players with top ten scores of all time from sample Cloud Spanner database table.

    queryTopTenWithTimespan    Query players with top ten scores within a specific timespan from sample Cloud Spanner database table.

    deleteDatabase             Delete a Spanner database.

    help                       Display more information on a specific command.

    version                    Display version information.

    ```

    ```
    PS > dotnet run createSampleDatabase your-project-id my-instance my-database
    Waiting for operation to complete...
    Operation status: RanToCompletion
    Created sample database my-database on instance my-instance
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
