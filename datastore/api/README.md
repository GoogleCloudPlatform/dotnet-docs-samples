# Cloud Storage Sample

A sample demonstrating how to invoke Google Cloud Datastore from C#.

## Links

- [Cloud Datastore Docs](https://cloud.google.com/datastore/docs/)

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=datastore.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Datastore API.

6.  Open [Datastore.sln](Datastore.sln) with Microsoft Visual Studio version 2015 or later.

8.  Build the Solution.

9.  From the command line, run TaskList.exe to see a list of 
    subcommands:

    ```ps1
    PS C:\...\TaskList\bin\Debug> .\tasklist
    Cloud Datastore Task List
    
    Usage:
      new <description>  Adds a task with a description <description>
      done <task-id>     Marks a task as done
      list               Lists all tasks by creation time
      delete <task-id>   Deletes a task
    >    
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
