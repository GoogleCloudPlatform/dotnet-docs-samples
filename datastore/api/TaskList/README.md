# Google Cloud Datastore Sample

A sample demonstrating how to invoke Google Cloud Datastore from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/datastore/api).

## Links

- [Cloud Datastore Docs](https://cloud.google.com/datastore/docs/)

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=datastore.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Datastore API.

9.  From the command line:
    ```ps1
    PS > dotnet restore
    PS > dotnet run
    Using project your-project-id
    Cloud Datastore Task List

    Usage:
    new <description>  Adds a task with a description <description>
    done <task-id>     Marks a task as done
    list               Lists all tasks by creation time
    delete <task-id>   Deletes a task
    > new "Wash dishes."
    task added
    > list
    found 1 tasks
    task ID : description
    ---------------------
    5734055144325120 : "Wash dishes." (created 10/13/2017 9:40:55 PM)
    >
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)
