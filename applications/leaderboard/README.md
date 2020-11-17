# .NET Cloud Spanner Sample Leaderboard Application

A leaderboard sample that uses the Spanner commit timestamp feature and demonstrates
how to call the [Google Cloud Spanner API](https://cloud.google.com/spanner/docs/)
from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio](
    https://www.visualstudio.com/), or the command line.

This sample includes extra directories step4, step5, and step6 that contain partial versions of this sample app. These directories are intended to provide guidance as part of a separate Codelab walk-through where the application is built in the following stages
that correspond to the steps in Codelab:

* step4 - Create the sample database along with the tables Players and Scores.
* step5 - Populate the Players and Scores tables with sample data.
* step6 - Run sample queries including sorting the results by timestamp.

If you only want to run the complete sample refer to the application in the Leaderboard directory.


## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=spanner.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Spanner API.

10. From a Powershell command line, run the Spanner Leaderboard sample to see a list of subcommands:
    ```
    PS C:\...\dotnet-docs-samples\applications\leaderboard\Leaderboard> dotnet run
    Leaderboard 1.0.0
    Copyright (C) 2019 Leaderboard
    ERROR(S):
    No verb selected.

    create     Create a sample Cloud Spanner database along with sample 'Players' and 'Scores' tables in your project.

    insert     Insert sample 'players' records or 'scores' records into the database.

    query      Query players with 'Top Ten' scores within a specific timespan from sample Cloud Spanner database table.

    delete     Delete a Spanner database.

    help       Display more information on a specific command.

    version    Display version information.

    ```

    ```
    PS > dotnet run create your-project-id your-instance your-database
    Waiting for operation to complete...
    Operation status: RanToCompletion
    Created sample database your-database on instance your-instance
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
