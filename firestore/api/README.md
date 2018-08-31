# .NET Cloud Firestore Samples

A collection of samples that demonstrate how to call the
[Google Cloud Firestore API](https://cloud.google.com/firestore/docs/) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=firestore.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Firestore API.

3. Open the [Firebase Console](https://console.firebase.google.com/) and create a new project. (You can't use both Cloud Firestore and Cloud Datastore in the same project, which might affect apps using App Engine. Try using Cloud Firestore with a different project if this is the case).

4. In the Database section, click the 'Get Started' button for Cloud Firestore Beta.

5. When prompted, select 'Start in test mode' and click 'Enable'.

6. Set up 3 indexes in the Firebase console. Visit the [Managing Indexes](https://cloud.google.com/firestore/docs/query-data/indexing) documentation to see how to do this. Create one for the 'cities' collection with 'Name' ascending and 'State' ascending. Create another for the 'cities' collection with 'State' ascending and 'Population' descending. Create a final one for the 'cities' collection with 'State' ascending and 'Population' ascending.

7.  From a Powershell command line, run the Quickstart sample:
    ```
    PS C:\...\dotnet-docs-samples\firestore\api\Quickstart> dotnet run

    Usage:
    C:\> dotnet run command YOUR_PROJECT_ID

    Where command is one of
        initialize-project-id
        add-data-1
        add-data-2
        retrieve-all-documents
    ```

8.  And run the AddData sample to add data to Cloud Firestore:
    ```
    PS C:\...\dotnet-docs-samples\firestore\api\AddData> dotnet run

    Usage:
    C:\> dotnet run command YOUR_PROJECT_ID

    Where command is one of
        add-doc-as-map
        update-create-if-missing
        add-doc-data-types
        add-simple-doc-as-entity
        set-requires-id
        add-doc-data-with-auto-id
        add-doc-data-after-auto-id
        update-doc
        update-nested-fields
        update-server-timestamp
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
