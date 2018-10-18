# .NET Cloud Firestore Samples

A collection of samples that demonstrate how to call the
[Google Cloud Firestore API](https://cloud.google.com/firestore/docs/) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2. Visit the [Cloud Console](https://console.cloud.google.com) and create a new project. (You can't use both Cloud Firestore and Cloud Datastore in the same project, which might affect apps using App Engine. Try using Cloud Firestore with a different project if this is the case).

3. Visit the [Cloud Firestore Viewer](https://console.cloud.google.com/firestore). From the 'Select a database service' screen, choose 'Cloud Firestore in Native mode'.

4. Select a [Cloud Firestore location](https://cloud.google.com/firestore/docs/locations). Click 'Create Database'. When you create a Cloud Firestore database, it enables the API for you.

5. Set up authentication by following these [tutorial instructions](https://cloud.google.com/firestore/docs/quickstart-servers#set_up_authentication).

6.  From a Powershell command line, run the Quickstart sample:
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

7.  And run the AddData sample to add data to Cloud Firestore:
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
        update-document-array
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
