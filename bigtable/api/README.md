# .NET Cloud Bigtable Samples

A collection of samples that demonstrate how to call the
[Google Cloud Bigtable API](https://cloud.google.com/bigtable/docs/) from C#.

This sample requires [.NET Core 2.0](https://www.microsoft.com/net/core) or later.  That means using [Visual Studio 2017](https://www.visualstudio.com/), or the command line.

## Build and Run

1. **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=bigtable&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Bigtable API.

## Quick Start

3.  Provisioning an instance 
    Follow the instructions in the [user
    documentation](https://cloud.google.com/bigtable/docs/creating-instance) to
    create a Cloud Bigtable instance if necessary.

4.  Follow the [cbt tutorial](https://cloud.google.com/bigtable/docs/quickstart-cbt) to install the cbt       command line tool. Here are the cbt commands to create a table, column       family and add some data:
    ```
    cbt -project <YOUR-PROJECT-ID> -instance <YOUR-INSTANCE-ID> createtable my-table
    cbt -project <YOUR-PROJECT-ID> -instance <YOUR-INSTANCE-ID> createfamily my-table cf1
    cbt -project <YOUR-PROJECT-ID> -instance <YOUR-INSTANCE-ID> set my-table r1 cf1:c1=test-value
    ```
5.  Edit `QuickStart\QuickStart.cs`, and replace YOUR-PROJECT-ID with id
    of the project you created in step 1. Also replace YOUR-INSTANCE-ID with id of your instance you created in step 3.

6.  From a Powershell command line, execute the following command to run the QuickStart sample to read the row you just wrote using `cbt`:
    ```
    PS <YOUR-PROJECT-DIRECTORY>\dotnet-docs-samples\bigtable\api\QuickStart\donet run
    ```
## Hello World

7.  Edit `HelloWorld\HelloWorld.cs`, and replace YOUR-PROJECT-ID with id
    of the project you created in step 1. Also replace YOUR-INSTANCE-ID with id of your instance you created in step 3.

8.  From a Powershell command line, execute the following command to run the HelloWorld sample:
    
    PS <YOUR-PROJECT-DIRECTORY>\dotnet-docs-samples\bigtable\api\HelloWorld> dotnet run
    
    You will see output resembling the following, interspersed with informational logging
    from the underlying libraries:
    ```
    Create new table: Hello-Bigtable with column family: cf, Instance: dotnet-perf
    Table Hello-Bigtable created succsessfully

    Write some greetings to the table Hello-Bigtable
        Greeting:   -- Hello World!      -- written successfully
        Greeting:   -- Hellow Bigtable!  -- written successfully
        Greeting:   -- Hellow C#!        -- written successfully
    Read the first row
        Row key: greeting0   -- Value: Hello World!       -- Time Stamp: 1529295849363000
    Read all rows using streaming
        Row key: greeting0   -- Value: Hello World!       -- Time Stamp: 1529295849363000
        Row key: greeting1   -- Value: Hellow Bigtable!   -- Time Stamp: 1529295850220000
        Row key: greeting2   -- Value: Hellow C#!         -- Time Stamp: 1529295850223000
    Delete table: Hello-Bigtable
    Table: Hello-Bigtable deleted succsessfully
    ```

## Instance Example

9.  Edit `InstanceAdminExample\InstanceAdmin.cs`, and replace YOUR-PROJECT-ID with id
    of the project you created in step 1.

10. From a Powershell command line, execute the following command to run the InstanceAdmin sample to see a list of subcommands:
    ```
    PS <YOUR-PROJECT-DIRECTORY>\dotnet-docs-samples\bigtable\api\InstanceAdminExample> dotnet run

    InstanceAdmin 1.0.0
    Copyright (C) 2018 InstanceAdmin
    ERROR(S):
    No verb selected.

    createProdInstance    Create a `PRODUCTION` type Instance with SSD storage type in this project.

    createDevInstance     Create a `DEVELOPMENT` type Instance with HDD storage type in this project.

    listInstances         Lists instances in a project.

    getInstance           Gets information about an instance in a project.

    listClusters          Lists clusters in an instance.

    createCluster         Creates an additional replicated cluster within an instance.

    deleteCluster         Deletes a cluster from an instance.

    deleteInstance        Deletes an instance from a project.

    help                  Display more information on a specific command.

    version               Display version information.
    ```
    ### Subcommand example:
    ```
    dotnet run createProdInstance my-instance
    Creating a PRODUCTION instance
    Waiting for operation to complete...
    Instance: my-instance Prod was successfully created in grass-clump-479 project
    --------------------------------------------------
    Printing instance my-instance-prod
    Instance ID:                  my-instance-prod
    Instance Display Name:        my-instance Prod
    Type:                         Production
    State:                        Ready
    Printing instance my-instance-prod
    Label Count:                  1
                            {prod-label : prod-label}
    Listing clusters on instance my-instance-prod
    Waiting for operation to complete...
    Cluster count:                1 clusters on instance my-instance-prod

    Printing cluster ssd-cluster1
    Cluster ID:                 ssd-cluster1
    Storage Type:               Ssd
    Location:                   us-east1-b
    Node Count:                 3
    State:                      Ready
    ```


## Table Example

11. Edit `TableAdminExample\TableAdmin.cs`, and replace YOUR-PROJECT-ID with id
    of the project you created in step 1. Also replace YOUR-INSTANCE-ID with id of your instance you created in step 3.

12. From a Powershell command line, execute the following command to run the InstanceAdmin sample to see a list of subcommands:
    ```
    PS <YOUR-PROJECT-DIRECTORY>\dotnet-docs-samples\bigtable\api\TableAdminExample> dotnet run

    TableAdmin 1.0.0
    Copyright (C) 2018 TableAdmin
    ERROR(S):
    No verb selected.

    createTable                 Creates a table in the Instance.

    listTables                  Lists tables in the Instance.

    getTable                    Gets information about a table.

    createMaxAgeFamily          Creates a column family with max age GC rule.

    createMaxVersionsFamily     Creates a column family with max versions GC rule.

    createUnionFamily           Creates a column family with union GC rule.

    createIntersectionFamily    Creates a column family with intersection GC rule.

    createNestedFamily          Creates a column family with nested GC rules.

    updateFamily                Update the column family metadata to update the GC rule.

    deleteFamily                Deletes a columnFamily.

    deleteTable                 Deletes a table from the Instance.

    help                        Display more information on a specific command.

    version                     Display version information.
    ```
    ### Subcommand example:
    ```
    dotnet run createTable my-table
    Creating table
    Checking if table exists...
    Getting table
    --------------------------------------------------
    Printing table information
    Table ID:                     my-table
    ```
   
9. Cleaning up
 
    To avoid incurring extra charges to your Google Cloud Platform account, remove
    the resources created for this sample.
 
    *  Go to the [Instances page][Instances page] in the Cloud Cloud Platform
 
     [Instances page]:https://console.cloud.google.com/project/_/bigtable/instances
 
    *  Click the isntance name.
 
    *  Click **Delete**.
 
     ![Delete](https://cloud.google.com/bigtable/img/delete-quickstart-instance.png)
 
    * Type the instance ID, then click **Delete** to delete the instance.
## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
