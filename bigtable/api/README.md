# .NET Cloud Bigtable Samples

A collection of samples that demonstrate how to call the
[Google Cloud Bigtable API](https://cloud.google.com/bigtable/docs/) from C#.

This sample requires [.NET Core 2.0](https://www.microsoft.com/net/core) or later.  That means using [Visual Studio 2017](https://www.visualstudio.com/), or the command line.

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=bigtable&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Spanner API.

5.  Provisioning an instance 
    Follow the instructions in the [user
    documentation](https://cloud.google.com/bigtable/docs/creating-instance) to
    create a Cloud Bigtable instance if necessary.

7.  Edit `HelloWorld\HelloWorld.cs`, and replace YOUR-PROJECT-ID with id
    of the project you created in step 1. 
    Also replace YOUR-INSTANCE-ID with id of your instance you created in step 5.

9.  From a Powershell command line, run the HelloWorld sample:
    ```
    PS C:\...\dotnet-docs-samples\bigtable\api\HelloWorld> dotnet run
    ```
    You will see output resembling the following, interspersed with informational logging
    from the underlying libraries:
        Create new table: Hello-Bigtable with column family: cf, Instance: dotnet-perf
        Table Hello-Bigtable created succsessfully

        Write some greetings to the table Hello-Bigtable
            Greeting: --Hello World!-- written successfully
            Greeting: --Hellow Bigtable!-- written successfully
            Greeting: --Hellow C#!-- written successfully
        Read the first row
            Row key: greeting0, Value: Hello World!
        Read all rows using streaming
            Row key: greeting0, Value: Hello World!
            Row key: greeting1, Value: Hellow Bigtable!
            Row key: greeting2, Value: Hellow C#!
        Delete table: Hello-Bigtable
        Table: Hello-Bigtable deleted succsessfully

10. And run the Bigtable sample to see a list of subcommands:
   
11. Cleaning up
 
    To avoid incurring extra charges to your Google Cloud Platform account, remove
    the resources created for this sample.
 
    *  Go to the [Instances page][Instances page] in the Cloud Cloud Platform
 
     [Instances page]:https://console.cloud.google.com/project/_/bigtable/instances
 
    *  Click the isntance name.
 
    *  Click **Delete**.
 
     ![Delete]( https://cloud.google.com/bigtable/img/delete-HelloWorld-instance.png)
 
    * Type the instance ID, then click **Delete** to delete the instance.
## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
