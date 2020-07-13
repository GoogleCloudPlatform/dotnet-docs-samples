/*
 * Copyright (c) 2017 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using CommandLine;
using Google.Cloud.Spanner.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace GoogleCloudSamples.Spanner
{
    class DefaultOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("createDatabase", HelpText = "Create a Cloud Spanner database in your project.")]
    class CreateDatabaseOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when creating Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the database will be created.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database to create.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("createSampleDatabase", HelpText = "Create a sample Cloud Spanner database along with sample tables in your project.")]
    class CreateSampleDatabaseOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when creating Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database will be created.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the sample database to create.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("createTable", HelpText = "Create a table in your project's Cloud Spanner database.")]
    class CreateTableOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when creating Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the table will be created.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the table will be created.", Required = true)]
        public string databaseId { get; set; }
        [Value(3, HelpText = "The name of the table to create.", Required = true)]
        public string tableName { get; set; }
    }

    [Verb("dropSampleTables", HelpText = "Drops the tables created by createSampleDatabase.")]
    class DropSampleTablesOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when creating Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the tables will be dropped.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the tables will be dropped.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("insertSampleData", HelpText = "Insert sample data into sample Cloud Spanner database table.")]
    class InsertSampleDataOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data will be inserted.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data will be inserted.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("deleteSampleData", HelpText = "Delete sample data from sample Cloud Spanner database table.")]
    class DeleteSampleDataOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data will be removed.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data will be removed.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("querySampleData", HelpText = "Query sample data from sample Cloud Spanner database table.")]
    class QuerySampleDataOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("addIndex", HelpText = "Add an index to the sample Cloud Spanner database table.")]
    class AddIndexOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("addStoringIndex", HelpText = "Add a storing index to the sample Cloud Spanner database table.")]
    class AddStoringIndexOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryDataWithIndex", HelpText = "Query the sample Cloud Spanner database table using an index.")]
    class QueryDataWithIndexOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
        [Value(3, HelpText = "The start of the title index.", Required = false)]
        public string startTitle { get; set; }
        [Value(4, HelpText = "The end of the title index.", Required = false)]
        public string endTitle { get; set; }
    }

    [Verb("queryDataWithStoringIndex", HelpText = "Query the sample Cloud Spanner database table using an storing index.")]
    class QueryDataWithStoringIndexOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
        [Value(3, HelpText = "The start of the title index.", Required = false)]
        public string startTitle { get; set; }
        [Value(4, HelpText = "The end of the title index.", Required = false)]
        public string endTitle { get; set; }
    }

    [Verb("addColumn", HelpText = "Add a column to the sample Cloud Spanner database table.")]
    class AddColumnOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("writeDataToNewColumn", HelpText = "Write data to a newly added column in the sample Cloud Spanner database table.")]
    class WriteDataToNewColumnOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryNewColumn", HelpText = "Query data from a newly added column in the sample Cloud Spanner database table.")]
    class QueryNewColumnOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryDataWithTransaction", HelpText = "Query the sample Cloud Spanner database table using a transaction.")]
    class QueryDataWithTransactionOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }

        [Value(3, HelpText = "The platform code to execute. This should be 'netcore' or 'net45' (the default).", Required = false)]
        public string platform { get; set; } = "net45";
    }

    [Verb("readWriteWithTransaction", HelpText = "Update data in the sample Cloud Spanner database table using a read-write transaction.")]
    class ReadWriteWithTransactionOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }

        [Value(3, HelpText = "The platform code to execute. This should be 'netcore' or 'net45' (the default).", Required = false)]
        public string platform { get; set; } = "net45";
    }

    [Verb("readStaleData", HelpText = "Read data that is ten seconds old.")]
    class ReadStaleDataOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("insertStructSampleData", HelpText = "Insert sample data that can be queried using Spanner structs.")]
    class InsertStructSampleDataOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data will be inserted.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data will be inserted.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryDataWithStruct", HelpText = "Query sample data in the sample Cloud Spanner database table using a Spanner struct.")]
    class QueryDataWithStructOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryDataWithArrayOfStruct", HelpText = "Query sample data in the sample Cloud Spanner database table using an array of Spanner structs.")]
    class QueryDataWithArrayOfStructOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryDataWithStructField", HelpText = "Query data sample data in the sample Cloud Spanner database table using a field in a Spanner struct.")]
    class QueryDataWithStructFieldOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryDataWithNestedStructField", HelpText = "Query data sample data in the sample Cloud Spanner database table using a field in a nested Spanner struct.")]
    class QueryDataWithNestedStructFieldOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("batchInsertRecords", HelpText = "Batch insert sample records into the database.")]
    class BatchInsertOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("batchReadRecords", HelpText = "Batch read sample records from the database.")]
    class BatchReadOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("addCommitTimestamp", HelpText = "Add a commit timestamp column to the sample Cloud Spanner database table.")]
    class AddCommitTimestampOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("updateDataWithTimestamp", HelpText = "Update data with a newly added commit timestamp column in the sample Cloud Spanner database table.")]
    class UpdateDataWithTimestampOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryDataWithTimestamp", HelpText = "Query data with a newly added commit timestamp column in the sample Cloud Spanner database table.")]
    class QueryDataWithTimestampOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("createTableWithTimestamp", HelpText = "Create a new table with a commit timestamp column in the sample Cloud Spanner database table.")]
    class CreateTableWithTimestampOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("writeDataWithTimestamp", HelpText = "Write data into table with a commit timestamp column in the sample Cloud Spanner database table.")]
    class WriteDataWithTimestampOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryNewTableWithTimestamp", HelpText = "Query data from table with a commit timestamp column in the sample Cloud Spanner database table.")]
    class QueryNewTableWithTimestampOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("querySingersTable", HelpText = "Query data from the sample 'Singers' Cloud Spanner database table.")]
    class QuerySingersTableOptions : DefaultOptions
    {
    }

    [Verb("queryAlbumsTable", HelpText = "Query data from the sample 'Albums' Cloud Spanner database table.")]
    class QueryAlbumsTableOptions : DefaultOptions
    {
    }

    [Verb("insertUsingDml", HelpText = "Insert data using a DML statement.")]
    class InsertUsingDmlOptions : DefaultOptions
    {
    }

    [Verb("updateUsingDml", HelpText = "Update data using a DML statement.")]
    class UpdateUsingDmlOptions : DefaultOptions
    {
    }

    [Verb("deleteUsingDml", HelpText = "Delete data using a DML statement.")]
    class DeleteUsingDmlOptions : DefaultOptions
    {
    }

    [Verb("updateUsingDmlWithTimestamp", HelpText = "Update the timestamp value of specifc records using a DML statement.")]
    class UpdateUsingDmlWithTimestampOptions : DefaultOptions
    {
    }

    [Verb("writeAndReadUsingDml", HelpText = "Insert data using a DML statement and then read the inserted data.")]
    class WriteAndReadUsingDmlOptions : DefaultOptions
    {
    }

    [Verb("updateUsingDmlWithStruct", HelpText = "Update data using a DML statement combined with a Spanner struct.")]
    class UpdateUsingDmlWithStructOptions : DefaultOptions
    {
    }

    [Verb("writeUsingDml", HelpText = "Insert multiple records using a DML statement.")]
    class WriteUsingDmlOptions : DefaultOptions
    {
    }

    [Verb("queryWithParameter", HelpText = "Query record inserted using DML with a query parameter.")]
    class QueryWithParameterOptions : DefaultOptions
    {
    }

    [Verb("writeWithTransactionUsingDml", HelpText = "Update data using a DML statement within a read-write transaction.")]
    class WriteWithTransactionUsingDmlOptions : DefaultOptions
    {
    }

    [Verb("updateUsingPartitionedDml", HelpText = "Update multiple records using a partitioned DML statement.")]
    class UpdateUsingPartitionedDmlOptions : DefaultOptions
    {
    }

    [Verb("deleteUsingPartitionedDml", HelpText = "Delete multiple records using a partitioned DML statement.")]
    class DeleteUsingPartitionedDmlOptions : DefaultOptions
    {
    }

    [Verb("updateUsingBatchDml", HelpText = "Updates sample data in the database using Batch DML.")]
    class UpdateUsingBatchDmlOptions : DefaultOptions
    {
    }

    [Verb("createTableWithDatatypes", HelpText = "Create 'Venues' table containing supported datatype columns.")]
    class CreateTableWithDatatypesOptions : DefaultOptions
    {
    }

    [Verb("writeDatatypesData", HelpText = "Write data into 'Venues' table.")]
    class WriteDatatypesDataOptions : DefaultOptions
    {
    }

    [Verb("queryWithArray", HelpText = "Query ARRAY datatype from the 'Venues' table.")]
    class QueryWithArrayOptions : DefaultOptions
    {
    }

    [Verb("queryWithBool", HelpText = "Query BOOL datatype from the 'Venues' table.")]
    class QueryWithBoolOptions : DefaultOptions
    {
    }

    [Verb("queryWithBytes", HelpText = "Query BYTES datatype from the Venues' table.")]
    class QueryWithBytesOptions : DefaultOptions
    {
    }

    [Verb("queryWithDate", HelpText = "Query DATE datatype from the 'Venues' table.")]
    class QueryWithDateOptions : DefaultOptions
    {
    }

    [Verb("queryWithFloat", HelpText = "Query FLOAT64 datatype from the 'Venues' table.")]
    class QueryWithFloatOptions : DefaultOptions
    {
    }

    [Verb("queryWithInt", HelpText = "Query INT64 datatype from the 'Venues' table.")]
    class QueryWithIntOptions : DefaultOptions
    {
    }

    [Verb("queryWithString", HelpText = "Query STRING datatype from the 'Venues' table.")]
    class QueryWithStringOptions : DefaultOptions
    {
    }

    [Verb("queryWithTimestamp", HelpText = "Query TIMESTAMP datatype from the 'Venues' table.")]
    class QueryWithTimestampOptions : DefaultOptions
    {
    }


    [Verb("listDatabaseTables", HelpText = "List all the user-defined tables in the database.")]
    class ListDatabaseTablesOptions : DefaultOptions
    {
    }

    [Verb("deleteDatabase", HelpText = "Delete a Spanner database.")]
    class DeleteOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database to delete.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("createBackup", HelpText = "Create a backup of a Spanner database.")]
    class CreateBackupOptions : DefaultOptions
    {
        [Value(3, HelpText = "The ID of the backup to create.", Required = true)]
        public string backupId { get; set; }
    }

    class DefaultBackupOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the backups reside.", Required = true)]
        public string instanceId { get; set; }
    }


    [Verb("restoreDatabase", HelpText = "Restore a Spanner database from a backup.")]
    class RestoreDatabaseOptions : DefaultBackupOptions
    {
        [Value(2, HelpText = "The ID of the database to create.", Required = true)]
        public string databaseId { get; set; }
        [Value(3, HelpText = "The ID of the backup to restore from.", Required = true)]
        public string backupId { get; set; }
    }

    [Verb("getBackupOperations", HelpText = "Get a list of Spanner database backup operations.")]
    class GetBackupOperationsOptions : DefaultBackupOptions
    {
        [Value(2, HelpText = "The ID of the database to filter backups on.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("getDatabaseOperations", HelpText = "Get a list of Spanner database operations.")]
    class GetDatabaseOperationsOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the operations reside.", Required = true)]
        public string instanceId { get; set; }
    }

    [Verb("updateBackup", HelpText = "Update a Spanner database backup.")]
    class UpdateBackupOptions : DefaultBackupOptions
    {
        [Value(2, HelpText = "The ID of the backup to update.", Required = true)]
        public string backupId { get; set; }
    }

    [Verb("deleteBackup", HelpText = "Delete a Spanner database backup.")]
    class DeleteBackupOptions : DefaultBackupOptions
    {
        [Value(2, HelpText = "The ID of the backup to delete.", Required = true)]
        public string backupId { get; set; }
    }

    [Verb("getBackups", HelpText = "Get a list of Spanner database backups.")]
    class GetBackupsOptions : DefaultBackupOptions
    {
        [Value(2, HelpText = "The ID of the database to filter backups on.", Required = true)]
        public string databaseId { get; set; }
        [Value(3, HelpText = "The ID of the backup to filter backups on.", Required = true)]
        public string backupId { get; set; }
    }

    [Verb("cancelBackupOperation", HelpText = "Cancel a Spanner database backup creation operation.")]
    class CancelBackupOperationOptions : CreateBackupOptions { }

    [Verb("createConnectionWithQueryOptions", HelpText = "Creates a connection with query options set and queries the 'Albums' table.")]
    class CreateConnectionWithQueryOptionsOptions : DefaultOptions
    {
    }

    [Verb("runCommandWithQueryOptions", HelpText = "Query 'Albums' table with query options set.")]
    class RunCommandWithQueryOptionsOptions : DefaultOptions
    {
    }

    // [START spanner_retry_strategy]
    public class RetryRobot
    {
        public TimeSpan FirstRetryDelay { get; set; } = TimeSpan.FromSeconds(1000);
        public float DelayMultiplier { get; set; } = 2;
        public int MaxTryCount { get; set; } = 7;
        public Func<Exception, bool> ShouldRetry { get; set; }

        /// <summary>
        /// Retry action when assertion fails.
        /// </summary>
        /// <param name="func"></param>
        public T Eventually<T>(Func<T> func)
        {
            TimeSpan delay = FirstRetryDelay;
            for (int i = 0; ; ++i)
            {
                try
                {
                    return func();
                }
                catch (Exception e)
                when (ShouldCatch(e) && i < MaxTryCount)
                {
                    Thread.Sleep(delay);
                    delay *= (int)DelayMultiplier;
                }
            }
        }

        private bool ShouldCatch(Exception e)
        {
            return ShouldRetry != null && ShouldRetry(e);
        }
    }
    // [END spanner_retry_strategy]

    public class Program
    {
        static readonly ILog s_logger = LogManager.GetLogger(typeof(Program));
        private static readonly string s_netCorePlatform = "netcore";

        enum ExitCode : int
        {
            Success = 0,
            InvalidParameter = 1,
        }

        // [START spanner_insert_data]
        public class Singer
        {
            public int SingerId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Album
        {
            public int SingerId { get; set; }
            public int AlbumId { get; set; }
            public string AlbumTitle { get; set; }
        }
        // [END spanner_insert_data]

        public class Performance
        {
            public int SingerId { get; set; }
            public int VenueId { get; set; }
            public DateTime EventDate { get; set; }
            public long Revenue { get; set; }
        }

        public class Venue
        {
            public int VenueId { get; set; }
            public string VenueName { get; set; }
            public byte[] VenueInfo { get; set; }
            public int Capacity { get; set; }
            public List<DateTime> AvailableDates { get; set; }
            public DateTime LastContactDate { get; set; }
            public bool OutdoorVenue { get; set; }
            public float PopularityScore { get; set; }
        }

        public static async Task CreateSampleDatabaseAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_create_database]
            // Initialize request connection string for database creation.
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}";
            // Make the request.
            using (var connection = new SpannerConnection(connectionString))
            {
                string createStatement = $"CREATE DATABASE `{databaseId}`";
                var cmd = connection.CreateDdlCommand(createStatement);
                await cmd.ExecuteNonQueryAsync();
            }
            // Update connection string with Database ID for table creation.
            connectionString = connectionString + $"/databases/{databaseId}";
            using (var connection = new SpannerConnection(connectionString))
            {
                // Define create table statement for table #1.
                string createTableStatement =
               @"CREATE TABLE Singers (
                     SingerId INT64 NOT NULL,
                     FirstName    STRING(1024),
                     LastName STRING(1024),
                     ComposerInfo   BYTES(MAX)
                 ) PRIMARY KEY (SingerId)";
                // Make the request.
                var cmd = connection.CreateDdlCommand(createTableStatement);
                await cmd.ExecuteNonQueryAsync();
                // Define create table statement for table #2.
                createTableStatement =
                @"CREATE TABLE Albums (
                     SingerId     INT64 NOT NULL,
                     AlbumId      INT64 NOT NULL,
                     AlbumTitle   STRING(MAX)
                 ) PRIMARY KEY (SingerId, AlbumId),
                 INTERLEAVE IN PARENT Singers ON DELETE CASCADE";
                // Make the request.
                cmd = connection.CreateDdlCommand(createTableStatement);
                await cmd.ExecuteNonQueryAsync();
            }
            // [END spanner_create_database]
        }

        public static async Task AddIndexAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_create_index]
            // Initialize request argument(s).
            string connectionString =
                $"Data Source=projects/{projectId}/instances/"
                + $"{instanceId}/databases/{databaseId}";
            string createStatement =
                "CREATE INDEX AlbumsByAlbumTitle ON Albums(AlbumTitle)";
            // Make the request.
            using (var connection = new SpannerConnection(connectionString))
            {
                var createCmd = connection.CreateDdlCommand(createStatement);
                await createCmd.ExecuteNonQueryAsync();
            }
            Console.WriteLine("Added the AlbumsByAlbumTitle index.");
            // [END spanner_create_index]
        }

        public static async Task AddStoringIndexAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_create_storing_index]
            // Initialize request argument(s).
            string connectionString =
                $"Data Source=projects/{projectId}/instances/"
                + $"{instanceId}/databases/{databaseId}";
            string createStatement =
                "CREATE INDEX AlbumsByAlbumTitle2 ON Albums(AlbumTitle) "
                + "STORING (MarketingBudget)";
            // Make the request.
            using (var connection = new SpannerConnection(connectionString))
            {
                var createCmd = connection.CreateDdlCommand(createStatement);
                await createCmd.ExecuteNonQueryAsync();
            }
            Console.WriteLine("Added the AlbumsByAlbumTitle2 index.");
            // [END spanner_create_storing_index]
        }

        public static async Task AddColumnAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_add_column]
            // Initialize request argument(s).
            string connectionString =
                $"Data Source=projects/{projectId}/instances/"
                + $"{instanceId}/databases/{databaseId}";
            string alterStatement =
                "ALTER TABLE Albums ADD COLUMN MarketingBudget INT64";
            // Make the request.
            using (var connection = new SpannerConnection(connectionString))
            {
                var updateCmd = connection.CreateDdlCommand(alterStatement);
                await updateCmd.ExecuteNonQueryAsync();
            }
            Console.WriteLine("Added the MarketingBudget column.");
            // [END spanner_add_column]
        }

        public static async Task QuerySampleDataAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_query_data]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT SingerId, AlbumId, AlbumTitle FROM Albums");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine("SingerId : "
                            + reader.GetFieldValue<string>("SingerId")
                            + " AlbumId : "
                            + reader.GetFieldValue<string>("AlbumId")
                            + " AlbumTitle : "
                            + reader.GetFieldValue<string>("AlbumTitle"));
                    }
                }
            }
            // [END spanner_query_data]
        }

        public static async Task QueryDataWithIndexAsync(
            string projectId, string instanceId, string databaseId,
            string startTitle = "Aardvark", string endTitle = "Goo")
        {
            // [START spanner_query_data_with_index]
            // [START spanner_read_data_with_index]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT AlbumId, AlbumTitle, MarketingBudget FROM Albums@ "
                    + "{FORCE_INDEX=AlbumsByAlbumTitle} "
                    + $"WHERE AlbumTitle >= @startTitle "
                    + $"AND AlbumTitle < @endTitle",
                    new SpannerParameterCollection {
                        {"startTitle", SpannerDbType.String},
                        {"endTitle", SpannerDbType.String} });
                cmd.Parameters["startTitle"].Value = startTitle;
                cmd.Parameters["endTitle"].Value = endTitle;
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var marketingBudget = reader.IsDBNull(
                            reader.GetOrdinal("MarketingBudget")) ?
                            "" :
                            reader.GetFieldValue<string>("MarketingBudget");
                        Console.WriteLine("AlbumId : "
                        + reader.GetFieldValue<string>("AlbumId")
                        + " AlbumTitle : "
                        + reader.GetFieldValue<string>("AlbumTitle")
                        + " MarketingBudget : "
                        + marketingBudget);
                    }
                }
            }
            // [END spanner_read_data_with_index]
            // [END spanner_query_data_with_index]
        }

        public static async Task QueryDataWithStoringIndexAsync(
            string projectId, string instanceId, string databaseId,
            string startTitle = "Aardvark", string endTitle = "Goo")
        {
            // [START spanner_read_data_with_storing_index]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT AlbumId, AlbumTitle, MarketingBudget FROM Albums@ "
                    + "{FORCE_INDEX=AlbumsByAlbumTitle2} "
                    + $"WHERE AlbumTitle >= @startTitle "
                    + $"AND AlbumTitle < @endTitle",
                    new SpannerParameterCollection {
                        {"startTitle", SpannerDbType.String},
                        {"endTitle", SpannerDbType.String} });
                cmd.Parameters["startTitle"].Value = startTitle;
                cmd.Parameters["endTitle"].Value = endTitle;
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine("AlbumId : "
                        + reader.GetFieldValue<string>("AlbumId")
                        + " AlbumTitle : "
                        + reader.GetFieldValue<string>("AlbumTitle")
                        + " MarketingBudget : "
                        + reader.GetFieldValue<string>("MarketingBudget"));
                    }
                }
            }
            // [END spanner_read_data_with_storing_index]
        }

        public static async Task QueryDataWithTransactionCoreAsync(
            string projectId, string instanceId, string databaseId)
        {
            Console.WriteLine(".NetCore API sample.");

            // [START spanner_read_only_transaction_core]
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                // Open a new read only transaction.
                using (var transaction =
                    await connection.BeginReadOnlyTransactionAsync())
                {
                    var cmd = connection.CreateSelectCommand(
                        "SELECT SingerId, AlbumId, AlbumTitle FROM Albums");
                    cmd.Transaction = transaction;

                    // Read #1.
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine("SingerId : "
                                + reader.GetFieldValue<string>("SingerId")
                                + " AlbumId : "
                                + reader.GetFieldValue<string>("AlbumId")
                                + " AlbumTitle : "
                                + reader.GetFieldValue<string>("AlbumTitle"));
                        }
                    }
                    // Read #2. Even if changes occur in-between the reads,
                    // the transaction ensures that Read #1 and Read #2
                    // return the same data.
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine("SingerId : "
                                + reader.GetFieldValue<string>("SingerId")
                                + " AlbumId : "
                                + reader.GetFieldValue<string>("AlbumId")
                                + " AlbumTitle : "
                                + reader.GetFieldValue<string>("AlbumTitle"));
                        }
                    }
                }
            }
            Console.WriteLine("Transaction complete.");
            // [END spanner_read_only_transaction_core]
        }

        public static async Task<object> ReadStaleDataAsync(
            string projectId, string instanceId, string databaseId)
        {
            Console.WriteLine(".NetCore API sample.");

            // [START spanner_read_stale_data]
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                // Open a new read only transaction.
                var staleness = TimestampBound.OfExactStaleness(
                    TimeSpan.FromSeconds(15));
                using (var transaction =
                    await connection.BeginReadOnlyTransactionAsync(staleness))
                {
                    var cmd = connection.CreateSelectCommand(
                        "SELECT SingerId, AlbumId, AlbumTitle FROM Albums");
                    cmd.Transaction = transaction;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine("SingerId : "
                                + reader.GetFieldValue<string>("SingerId")
                                + " AlbumId : "
                                + reader.GetFieldValue<string>("AlbumId")
                                + " AlbumTitle : "
                                + reader.GetFieldValue<string>("AlbumTitle"));
                        }
                    }
                }
            }
            // [END spanner_read_stale_data]
            return 0;
        }


        public static async Task QueryDataWithTransactionAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_read_only_transaction]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Gets a transaction object that captures the database state
            // at a specific point in time.
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeAsyncFlowOption.Enabled))
            {
                // Create connection to Cloud Spanner.
                using (var connection = new SpannerConnection(connectionString))
                {
                    // Open the connection, making the implicitly created
                    // transaction read only when it connects to the outer
                    // transaction scope.
                    await connection.OpenAsReadOnlyAsync()
                        .ConfigureAwait(false);
                    var cmd = connection.CreateSelectCommand(
                        "SELECT SingerId, AlbumId, AlbumTitle FROM Albums");
                    // Read #1.
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine("SingerId : "
                                + reader.GetFieldValue<string>("SingerId")
                                + " AlbumId : "
                                + reader.GetFieldValue<string>("AlbumId")
                                + " AlbumTitle : "
                                + reader.GetFieldValue<string>("AlbumTitle"));
                        }
                    }
                    // Read #2. Even if changes occur in-between the reads,
                    // the transaction ensures that Read #1 and Read #2
                    // return the same data.
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine("SingerId : "
                                + reader.GetFieldValue<string>("SingerId")
                                + " AlbumId : "
                                + reader.GetFieldValue<string>("AlbumId")
                                + " AlbumTitle : "
                                + reader.GetFieldValue<string>("AlbumTitle"));
                        }
                    }
                }
                scope.Complete();
                Console.WriteLine("Transaction complete.");
            }
            // [END spanner_read_only_transaction]
        }

        public static async Task WriteDataToNewColumnAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_update_data]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateUpdateCommand("Albums",
                    new SpannerParameterCollection {
                        {"SingerId", SpannerDbType.Int64},
                        {"AlbumId", SpannerDbType.Int64},
                        {"MarketingBudget", SpannerDbType.Int64},
                    });
                var cmdLookup =
                    connection.CreateSelectCommand("SELECT * FROM Albums");
                using (var reader = await cmdLookup.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.GetFieldValue<int>("SingerId") == 1
                            && reader.GetFieldValue<int>("AlbumId") == 1)
                        {
                            cmd.Parameters["SingerId"].Value =
                                reader.GetFieldValue<int>("SingerId");
                            cmd.Parameters["AlbumId"].Value =
                                reader.GetFieldValue<int>("AlbumId");
                            cmd.Parameters["MarketingBudget"].Value = 100000;
                            await cmd.ExecuteNonQueryAsync();
                        }
                        if (reader.GetInt64(0) == 2 && reader.GetInt64(1) == 2)
                        {
                            cmd.Parameters["SingerId"].Value =
                                reader.GetFieldValue<int>("SingerId");
                            cmd.Parameters["AlbumId"].Value =
                                reader.GetFieldValue<int>("AlbumId");
                            cmd.Parameters["MarketingBudget"].Value = 500000;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            Console.WriteLine("Updated data.");
            // [END spanner_update_data]
        }

        public static async Task QueryNewColumnAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_query_data_with_new_column]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd =
                    connection.CreateSelectCommand("SELECT * FROM Albums");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string budget = string.Empty;
                        if (reader["MarketingBudget"] != DBNull.Value)
                        {
                            budget = reader.GetFieldValue<string>("MarketingBudget");
                        }
                        Console.WriteLine("SingerId : "
                        + reader.GetFieldValue<string>("SingerId")
                        + " AlbumId : "
                        + reader.GetFieldValue<string>("AlbumId")
                        + $" MarketingBudget : {budget}");
                    }
                }
            }
            // [END spanner_query_data_with_new_column]
        }

        public static async Task ListDatabaseTablesAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_list_database_tables]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd =
                    connection.CreateSelectCommand(
                    "SELECT t.table_name FROM information_schema.tables AS t "
                    + "WHERE t.table_catalog = '' and t.table_schema = ''");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(
                            reader.GetFieldValue<string>("table_name"));
                    }
                }
            }
            // [END spanner_list_database_tables]
        }

        // [START spanner_read_write_transaction_core]
        public static async Task ReadWriteWithTransactionCoreAsync(
            string projectId,
            string instanceId,
            string databaseId)
        {
            // This sample transfers 200,000 from the MarketingBudget
            // field of the second Album to the first Album. Make sure to run
            // the addColumn and writeDataToNewColumn samples first,
            // in that order.
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            decimal transferAmount = 200000;
            decimal secondBudget = 0;
            decimal firstBudget = 0;

            Console.WriteLine(".NetCore API sample.");

            // Create connection to Cloud Spanner.
            using (var connection =
                new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                // Create a readwrite transaction that we'll assign
                // to each SpannerCommand.
                using (var transaction =
                        await connection.BeginTransactionAsync())
                {
                    // Create statement to select the second album's data.
                    var cmdLookup = connection.CreateSelectCommand(
                     "SELECT * FROM Albums WHERE SingerId = 2 AND AlbumId = 2");
                    cmdLookup.Transaction = transaction;
                    // Excecute the select query.
                    using (var reader = await cmdLookup.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Read the second album's budget.
                            secondBudget =
                               reader.GetFieldValue<decimal>("MarketingBudget");
                            // Confirm second Album's budget is sufficient and
                            // if not raise an exception. Raising an exception
                            // will automatically roll back the transaction.
                            if (secondBudget < transferAmount)
                            {
                                throw new Exception("The second album's "
                                        + $"budget {secondBudget} "
                                        + "contains less than the "
                                        + "amount to transfer.");
                            }
                        }
                    }
                    // Read the first album's budget.
                    cmdLookup = connection.CreateSelectCommand(
                     "SELECT * FROM Albums WHERE SingerId = 1 and AlbumId = 1");
                    cmdLookup.Transaction = transaction;
                    using (var reader = await cmdLookup.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            firstBudget =
                              reader.GetFieldValue<decimal>("MarketingBudget");
                        }
                    }

                    // Specify update command parameters.
                    var cmd = connection.CreateUpdateCommand("Albums",
                        new SpannerParameterCollection
                        {
                            {"SingerId", SpannerDbType.Int64},
                            {"AlbumId", SpannerDbType.Int64},
                            {"MarketingBudget", SpannerDbType.Int64},
                        });
                    cmd.Transaction = transaction;
                    // Update second album to remove the transfer amount.
                    secondBudget -= transferAmount;
                    cmd.Parameters["SingerId"].Value = 2;
                    cmd.Parameters["AlbumId"].Value = 2;
                    cmd.Parameters["MarketingBudget"].Value = secondBudget;
                    await cmd.ExecuteNonQueryAsync();
                    // Update first album to add the transfer amount.
                    firstBudget += transferAmount;
                    cmd.Parameters["SingerId"].Value = 1;
                    cmd.Parameters["AlbumId"].Value = 1;
                    cmd.Parameters["MarketingBudget"].Value = firstBudget;
                    await cmd.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                Console.WriteLine("Transaction complete.");
            }
        }
        // [END spanner_read_write_transaction_core]

        // [START spanner_read_write_transaction]
        public static async Task ReadWriteWithTransactionAsync(
            string projectId,
            string instanceId,
            string databaseId)
        {
            // This sample transfers 200,000 from the MarketingBudget
            // field of the second Album to the first Album. Make sure to run
            // the addColumn and writeDataToNewColumn samples first,
            // in that order.

            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeAsyncFlowOption.Enabled))
            {
                decimal transferAmount = 200000;
                decimal secondBudget = 0;
                decimal firstBudget = 0;

                // Create connection to Cloud Spanner.
                using (var connection =
                    new SpannerConnection(connectionString))
                {
                    // Create statement to select the second album's data.
                    var cmdLookup = connection.CreateSelectCommand(
                    "SELECT * FROM Albums WHERE SingerId = 2 AND AlbumId = 2");
                    // Excecute the select query.
                    using (var reader = await cmdLookup.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Read the second album's budget.
                            secondBudget =
                              reader.GetFieldValue<decimal>("MarketingBudget");
                            // Confirm second Album's budget is sufficient and
                            // if not raise an exception. Raising an exception
                            // will automatically roll back the transaction.
                            if (secondBudget < transferAmount)
                            {
                                throw new Exception("The second album's "
                                    + $"budget {secondBudget} "
                                    + "is less than the "
                                    + "amount to transfer.");
                            }
                        }
                    }
                    // Read the first album's budget.
                    cmdLookup = connection.CreateSelectCommand(
                    "SELECT * FROM Albums WHERE SingerId = 1 and AlbumId = 1");
                    using (var reader = await cmdLookup.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            firstBudget =
                              reader.GetFieldValue<decimal>("MarketingBudget");
                        }
                    }

                    // Specify update command parameters.
                    var cmd = connection.CreateUpdateCommand("Albums",
                        new SpannerParameterCollection {
                        {"SingerId", SpannerDbType.Int64},
                        {"AlbumId", SpannerDbType.Int64},
                        {"MarketingBudget", SpannerDbType.Int64},
                    });
                    // Update second album to remove the transfer amount.
                    secondBudget -= transferAmount;
                    cmd.Parameters["SingerId"].Value = 2;
                    cmd.Parameters["AlbumId"].Value = 2;
                    cmd.Parameters["MarketingBudget"].Value = secondBudget;
                    await cmd.ExecuteNonQueryAsync();
                    // Update first album to add the transfer amount.
                    firstBudget += transferAmount;
                    cmd.Parameters["SingerId"].Value = 1;
                    cmd.Parameters["AlbumId"].Value = 1;
                    cmd.Parameters["MarketingBudget"].Value = firstBudget;
                    await cmd.ExecuteNonQueryAsync();
                    scope.Complete();
                    Console.WriteLine("Transaction complete.");
                }
            }
        }
        // [END spanner_read_write_transaction]

        // [START spanner_dml_standard_insert]
        public static async Task InsertUsingDmlCoreAsync(
            string projectId,
            string instanceId,
            string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection =
                new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                SpannerCommand cmd = connection.CreateDmlCommand(
                   "INSERT Singers (SingerId, FirstName, LastName) "
                   + "VALUES (10, 'Virginia', 'Watson')");
                int rowCount = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"{rowCount} row(s) inserted...");
            }
        }
        // [END spanner_dml_standard_insert]

        // [START spanner_dml_standard_update]
        public static async Task UpdateUsingDmlCoreAsync(
            string projectId,
            string instanceId,
            string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection =
                new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                SpannerCommand cmd = connection.CreateDmlCommand(
                   "UPDATE Albums SET MarketingBudget = MarketingBudget * 2 "
                   + "WHERE SingerId = 1 and AlbumId = 1");
                int rowCount = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"{rowCount} row(s) updated...");
            }
        }
        // [END spanner_dml_standard_update]

        // [START spanner_dml_standard_delete]
        public static async Task DeleteUsingDmlCoreAsync(
            string projectId,
            string instanceId,
            string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection =
                new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                SpannerCommand cmd = connection.CreateDmlCommand(
                   "DELETE FROM Singers WHERE FirstName = 'Alice'");
                int rowCount = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"{rowCount} row(s) deleted...");
            }
        }
        // [END spanner_dml_standard_delete]


        // [START spanner_dml_standard_update_with_timestamp]
        public static async Task UpdateUsingDmlWithTimestampCoreAsync(
            string projectId,
            string instanceId,
            string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection =
                new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                SpannerCommand cmd = connection.CreateDmlCommand(
                   "UPDATE Albums "
                   + "SET LastUpdateTime = PENDING_COMMIT_TIMESTAMP() "
                   + "WHERE SingerId = 1");
                int rowCount = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"{rowCount} row(s) updated...");
            }
        }
        // [END spanner_dml_standard_update_with_timestamp]

        // [START spanner_dml_write_then_read]
        public static async Task WriteAndReadUsingDmlCoreAsync(
            string projectId,
            string instanceId,
            string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection =
                new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                SpannerCommand cmd = connection.CreateDmlCommand(
                   @"INSERT Singers (SingerId, FirstName, LastName)
                      VALUES (11, 'Timothy', 'Campbell')");
                int rowCount = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"{rowCount} row(s) inserted...");
                // Read newly inserted record.
                cmd = connection.CreateSelectCommand(
                    @"SELECT FirstName, LastName FROM Singers
                      WHERE SingerId = 11");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(
                            reader.GetFieldValue<string>("FirstName") + " "
                            + reader.GetFieldValue<string>("LastName"));
                    }
                }
            }
        }
        // [END spanner_dml_write_then_read]

        // [START spanner_dml_structs]
        public static async Task UpdateUsingDmlWithStructCoreAsync(
            string projectId,
            string instanceId,
            string databaseId)
        {
            var nameStruct = new SpannerStruct
            {
                { "FirstName", SpannerDbType.String, "Timothy" },
                { "LastName", SpannerDbType.String, "Campbell" },
            };
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection =
                new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                SpannerCommand cmd = connection.CreateDmlCommand(
                    "UPDATE Singers SET LastName = 'Grant' "
                   + "WHERE STRUCT<FirstName STRING, LastName STRING>"
                   + "(FirstName, LastName) = @name");
                cmd.Parameters.Add("name", nameStruct.GetSpannerDbType(), nameStruct);
                int rowCount = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"{rowCount} row(s) updated...");
            }
        }
        // [END spanner_dml_structs]

        // [START spanner_dml_getting_started_insert]
        public static async Task WriteUsingDmlCoreAsync(
            string projectId,
            string instanceId,
            string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection =
                new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                SpannerCommand cmd = connection.CreateDmlCommand(
                    "INSERT Singers (SingerId, FirstName, LastName) VALUES "
                       + "(12, 'Melissa', 'Garcia'), "
                       + "(13, 'Russell', 'Morales'), "
                       + "(14, 'Jacqueline', 'Long'), "
                       + "(15, 'Dylan', 'Shaw')");
                int rowCount = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"{rowCount} row(s) inserted...");
            }
        }
        // [END spanner_dml_getting_started_insert]

        public static async Task QueryWithParameterAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_query_with_parameter]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT SingerId, FirstName, LastName FROM Singers "
                    + $"WHERE LastName = @lastName",
                    new SpannerParameterCollection {
                        {"lastName", SpannerDbType.String}});
                cmd.Parameters["lastName"].Value = "Garcia";
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine("SingerId : "
                        + reader.GetFieldValue<string>("SingerId")
                        + " FirstName : "
                        + reader.GetFieldValue<string>("FirstName")
                        + " LastName : "
                        + reader.GetFieldValue<string>("LastName"));
                    }
                }
            }
            // [END spanner_query_with_parameter]
        }

        // [START spanner_dml_getting_started_update]
        public static async Task WriteWithTransactionUsingDmlCoreAsync(
            string projectId,
            string instanceId,
            string databaseId)
        {
            // This sample transfers 200,000 from the MarketingBudget
            // field of the second Album to the first Album. Make sure to run
            // the addColumn and writeDataToNewColumn samples first,
            // in that order.
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            decimal transferAmount = 200000;
            decimal secondBudget = 0;
            decimal firstBudget = 0;

            // Create connection to Cloud Spanner.
            using (var connection =
                new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                // Create a readwrite transaction that we'll assign
                // to each SpannerCommand.
                using (var transaction =
                        await connection.BeginTransactionAsync())
                {
                    // Create statement to select the second album's data.
                    var cmdLookup = connection.CreateSelectCommand(
                     "SELECT * FROM Albums WHERE SingerId = 2 AND AlbumId = 2");
                    cmdLookup.Transaction = transaction;
                    // Excecute the select query.
                    using (var reader = await cmdLookup.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Read the second album's budget.
                            secondBudget =
                               reader.GetFieldValue<decimal>("MarketingBudget");
                            // Confirm second Album's budget is sufficient and
                            // if not raise an exception. Raising an exception
                            // will automatically roll back the transaction.
                            if (secondBudget < transferAmount)
                            {
                                throw new Exception("The first album's "
                                        + $"budget {secondBudget} "
                                        + "is less than the "
                                        + "amount to transfer.");
                            }
                        }
                    }
                    // Read the first album's budget.
                    cmdLookup = connection.CreateSelectCommand(
                     "SELECT * FROM Albums WHERE SingerId = 1 and AlbumId = 1");
                    cmdLookup.Transaction = transaction;
                    using (var reader = await cmdLookup.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            firstBudget =
                              reader.GetFieldValue<decimal>("MarketingBudget");
                        }
                    }

                    // Update second album to remove the transfer amount.
                    secondBudget -= transferAmount;
                    SpannerCommand cmd = connection.CreateDmlCommand(
                        "UPDATE Albums SET MarketingBudget = @MarketingBudget "
                        + "WHERE SingerId = 2 and AlbumId = 2");
                    cmd.Parameters.Add("MarketingBudget", SpannerDbType.Int64, secondBudget);
                    cmd.Transaction = transaction;
                    await cmd.ExecuteNonQueryAsync();
                    // Update first album to add the transfer amount.
                    firstBudget += transferAmount;
                    cmd = connection.CreateDmlCommand(
                        "UPDATE Albums SET MarketingBudget = @MarketingBudget "
                        + "WHERE SingerId = 1 and AlbumId = 1");
                    cmd.Parameters.Add("MarketingBudget", SpannerDbType.Int64, firstBudget);
                    cmd.Transaction = transaction;
                    await cmd.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                Console.WriteLine("Transaction complete.");
            }
        }
        // [END spanner_dml_getting_started_update]

        // [START spanner_dml_partitioned_update]
        public static async Task UpdateUsingPartitionedDmlCoreAsync(
            string projectId,
            string instanceId,
            string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection =
                new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                SpannerCommand cmd = connection.CreateDmlCommand(
                    "UPDATE Albums SET MarketingBudget = 100000 WHERE SingerId > 1"
                );
                long rowCount = await cmd.ExecutePartitionedUpdateAsync();
                Console.WriteLine($"{rowCount} row(s) updated...");
            }
        }
        // [END spanner_dml_partitioned_update]

        // [START spanner_dml_partitioned_delete]
        public static async Task DeleteUsingPartitionedDmlCoreAsync(
            string projectId,
            string instanceId,
            string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection =
                new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                SpannerCommand cmd = connection.CreateDmlCommand(
                    "DELETE FROM Singers WHERE SingerId > 10"
                );
                long rowCount = await cmd.ExecutePartitionedUpdateAsync();
                Console.WriteLine($"{rowCount} row(s) deleted...");
            }
        }
        // [END spanner_dml_partitioned_delete]

        // [START spanner_dml_batch_update]
        public static async Task UpdateUsingBatchDmlCoreAsync(
            string projectId,
            string instanceId,
            string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection =
                new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                SpannerBatchCommand cmd = connection.CreateBatchDmlCommand();
                //var cmd = new SpannerBatchCommand(connection);

                cmd.Add("INSERT INTO Albums "
                    + "(SingerId, AlbumId, AlbumTitle, MarketingBudget) "
                    + "VALUES (1, 3, 'Test Album Title', 10000)");

                cmd.Add("UPDATE Albums "
                    + "SET MarketingBudget = MarketingBudget * 2 "
                    + "WHERE SingerId = 1 and AlbumId = 3");

                IEnumerable<long> affectedRows = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine(
                    $"Executed {affectedRows.Count()} "
                    + "SQL statements using Batch DML.");
            }
        }
        // [END spanner_dml_batch_update]

        // [START spanner_delete_data]
        public static async Task DeleteIndividualRowsAsync(
            string projectId, string instanceId, string databaseId)
        {
            const int singerId = 2;
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";
            List<Album> albums = new List<Album>
            {
                new Album { SingerId = singerId, AlbumId = 1, AlbumTitle = "Green" },
                new Album { SingerId = singerId, AlbumId = 3, AlbumTitle = "Terrified" },
            };
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                // Delete individual rows from the UpcomingAlbums table.
                await Task.WhenAll(albums.Select(album =>
                {
                    var cmd = connection.CreateDeleteCommand(
                        "UpcomingAlbums",
                        new SpannerParameterCollection
                        {
                            { "SingerId", SpannerDbType.Int64, album.SingerId },
                            { "AlbumId", SpannerDbType.Int64, album.AlbumId }
                        }
                    );
                    return cmd.ExecuteNonQueryAsync();
                }));

                Console.WriteLine("Deleted individual rows in UpcomingAlbums.");
            }
        }

        public static async Task DeleteRangeOfRowsAsync(
            string projectId, string instanceId, string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                // Delete a range of rows from the UpcomingSingers table where the column key is >=3 and <5.
                var cmd = connection.CreateDmlCommand(
                   "DELETE FROM UpcomingSingers WHERE SingerId >= 3 AND SingerId < 5");
                int rowCount = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"{rowCount} row(s) deleted from UpcomingSingers.");
            }
        }

        public static async Task DeleteAllRowsAsync(
            string projectId, string instanceId, string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                // Delete remaining UpcomingSingers rows, which will also delete the remaining
                // UpcomingAlbums rows since it was defined with ON DELETE CASCADE.
                var cmd = connection.CreateDmlCommand(
                   "DELETE FROM UpcomingSingers WHERE true");
                int rowCount = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"{rowCount} row(s) deleted from UpcomingSingers.");
            }
        }
        // [END spanner_delete_data]

        // [START spanner_insert_data]
        public static async Task InsertSampleDataAsync(
            string projectId, string instanceId, string databaseId)
        {
            const int firstSingerId = 1;
            const int secondSingerId = 2;
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            List<Singer> singers = new List<Singer>
            {
                new Singer { SingerId = firstSingerId, FirstName = "Marc",
                    LastName = "Richards" },
                new Singer { SingerId = secondSingerId, FirstName = "Catalina",
                    LastName = "Smith" },
                new Singer { SingerId = 3, FirstName = "Alice",
                    LastName = "Trentor" },
                new Singer { SingerId = 4, FirstName = "Lea",
                    LastName = "Martin" },
                new Singer { SingerId = 5, FirstName = "David",
                    LastName = "Lomond" },
            };
            List<Album> albums = new List<Album>
            {
                new Album { SingerId = firstSingerId, AlbumId = 1,
                    AlbumTitle = "Total Junk" },
                new Album { SingerId = firstSingerId, AlbumId = 2,
                    AlbumTitle = "Go, Go, Go" },
                new Album { SingerId = secondSingerId, AlbumId = 1,
                    AlbumTitle = "Green" },
                new Album { SingerId = secondSingerId, AlbumId = 2,
                    AlbumTitle = "Forever Hold your Peace" },
                new Album { SingerId = secondSingerId, AlbumId = 3,
                    AlbumTitle = "Terrified" },
            };
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                // Insert rows into the Singers table.
                var cmd = connection.CreateInsertCommand("Singers",
                    new SpannerParameterCollection
                    {
                        { "SingerId", SpannerDbType.Int64 },
                        { "FirstName", SpannerDbType.String },
                        { "LastName", SpannerDbType.String }
                    });
                await Task.WhenAll(singers.Select(singer =>
                {
                    cmd.Parameters["SingerId"].Value = singer.SingerId;
                    cmd.Parameters["FirstName"].Value = singer.FirstName;
                    cmd.Parameters["LastName"].Value = singer.LastName;
                    return cmd.ExecuteNonQueryAsync();
                }));

                // Insert rows into the Albums table.
                cmd = connection.CreateInsertCommand("Albums",
                    new SpannerParameterCollection
                    {
                        { "SingerId", SpannerDbType.Int64 },
                        { "AlbumId", SpannerDbType.Int64 },
                        { "AlbumTitle", SpannerDbType.String }
                    });
                await Task.WhenAll(albums.Select(album =>
                {
                    cmd.Parameters["SingerId"].Value = album.SingerId;
                    cmd.Parameters["AlbumId"].Value = album.AlbumId;
                    cmd.Parameters["AlbumTitle"].Value = album.AlbumTitle;
                    return cmd.ExecuteNonQueryAsync();
                }));
                Console.WriteLine("Inserted data.");
            }
        }
        // [END spanner_insert_data]

        public static async Task CreateDeleteSampleTableAsync(
            string projectId, string instanceId, string databaseId)
        {
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            using (var connection = new SpannerConnection(connectionString))
            {
                // Define create table statement for table #1.
                string createTableStatement =
               @"CREATE TABLE UpcomingSingers (
                     SingerId INT64 NOT NULL,
                     FirstName    STRING(1024),
                     LastName STRING(1024),
                     ComposerInfo   BYTES(MAX)
                 ) PRIMARY KEY (SingerId)";
                // Make the request.
                var cmd = connection.CreateDdlCommand(createTableStatement);
                await cmd.ExecuteNonQueryAsync();
                // Define create table statement for table #2.
                createTableStatement =
                @"CREATE TABLE UpcomingAlbums (
                     SingerId     INT64 NOT NULL,
                     AlbumId      INT64 NOT NULL,
                     AlbumTitle   STRING(MAX)
                 ) PRIMARY KEY (SingerId, AlbumId),
                 INTERLEAVE IN PARENT UpcomingSingers ON DELETE CASCADE";
                // Make the request.
                cmd = connection.CreateDdlCommand(createTableStatement);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task InsertDeleteSampleDataAsync(
            string projectId, string instanceId, string databaseId)
        {
            const int firstSingerId = 1;
            const int secondSingerId = 2;
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            List<Singer> singers = new List<Singer>
            {
                new Singer { SingerId = firstSingerId, FirstName = "Marc",
                    LastName = "Richards" },
                new Singer { SingerId = secondSingerId, FirstName = "Catalina",
                    LastName = "Smith" },
                new Singer { SingerId = 3, FirstName = "Alice",
                    LastName = "Trentor" },
                new Singer { SingerId = 4, FirstName = "Lea",
                    LastName = "Martin" },
                new Singer { SingerId = 5, FirstName = "David",
                    LastName = "Lomond" },
            };
            List<Album> albums = new List<Album>
            {
                new Album { SingerId = firstSingerId, AlbumId = 1,
                    AlbumTitle = "Total Junk" },
                new Album { SingerId = firstSingerId, AlbumId = 2,
                    AlbumTitle = "Go, Go, Go" },
                new Album { SingerId = secondSingerId, AlbumId = 1,
                    AlbumTitle = "Green" },
                new Album { SingerId = secondSingerId, AlbumId = 2,
                    AlbumTitle = "Forever Hold your Peace" },
                new Album { SingerId = secondSingerId, AlbumId = 3,
                    AlbumTitle = "Terrified" },
            };
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                // Insert rows into the Singers table.
                var cmd = connection.CreateInsertCommand("UpcomingSingers",
                    new SpannerParameterCollection
                    {
                        { "SingerId", SpannerDbType.Int64 },
                        { "FirstName", SpannerDbType.String },
                        { "LastName", SpannerDbType.String }
                    });
                await Task.WhenAll(singers.Select(singer =>
                {
                    cmd.Parameters["SingerId"].Value = singer.SingerId;
                    cmd.Parameters["FirstName"].Value = singer.FirstName;
                    cmd.Parameters["LastName"].Value = singer.LastName;
                    return cmd.ExecuteNonQueryAsync();
                }));

                // Insert rows into the Albums table.
                cmd = connection.CreateInsertCommand("UpcomingAlbums",
                    new SpannerParameterCollection
                    {
                        { "SingerId", SpannerDbType.Int64 },
                        { "AlbumId", SpannerDbType.Int64 },
                        { "AlbumTitle", SpannerDbType.String }
                    });
                await Task.WhenAll(albums.Select(album =>
                {
                    cmd.Parameters["SingerId"].Value = album.SingerId;
                    cmd.Parameters["AlbumId"].Value = album.AlbumId;
                    cmd.Parameters["AlbumTitle"].Value = album.AlbumTitle;
                    return cmd.ExecuteNonQueryAsync();
                }));
                Console.WriteLine("Inserted data.");
            }
        }

        public static async Task<object> DropDeleteSampleTables(
            string projectId, string instanceId, string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";
            // Make the request.
            using (var connection = new SpannerConnection(connectionString))
            {
                await DeleteTableAsync(connection, "UpcomingAlbums");
                await DeleteTableAsync(connection, "UpcomingSingers");
                return 0;
            }
            async Task DeleteTableAsync(SpannerConnection connection, string table)
            {
                try
                {
                    await connection.CreateDdlCommand($"DROP TABLE {table}")
                        .ExecuteNonQueryAsync();
                }
                catch (SpannerException e) when (e.ErrorCode == ErrorCode.NotFound)
                {
                    // Table does not exist.  Not a problem.
                }
            }
        }

        public static async Task CreateDatabaseAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_create_custom_database]
            // Initialize request connection string for database creation.
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}";
            // Make the request.
            using (var connection = new SpannerConnection(connectionString))
            {
                string createStatement = $"CREATE DATABASE `{databaseId}`";
                var cmd = connection.CreateDdlCommand(createStatement);
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (SpannerException e) when (e.ErrorCode == ErrorCode.AlreadyExists)
                {
                    // Database Already Exists.
                }
            }
            // [END spanner_create_custom_database]
        }

        public static async Task<object> DropSampleTables(
            string projectId, string instanceId, string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";
            // Make the request.
            using (var connection = new SpannerConnection(connectionString))
            {
                foreach (string table in new[] { "Albums", "Singers" })
                {
                    try
                    {
                        await connection.CreateDdlCommand($"DROP TABLE {table}")
                            .ExecuteNonQueryAsync();
                    }
                    catch (SpannerException e) when (e.ErrorCode == ErrorCode.NotFound)
                    {
                        // Table does not exist.  Not a problem.
                    }
                }
                return 0;
            }
        }

        public static object CreateSampleDatabase(string projectId,
            string instanceId, string databaseId)
        {
            var response =
                CreateSampleDatabaseAsync(projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            Console.WriteLine($"Created sample database {databaseId} on "
                + $"instance {instanceId}");
            return ExitCode.Success;
        }

        public static object InsertSampleData(string projectId,
            string instanceId, string databaseId)
        {
            var response = InsertSampleDataAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object DeleteSampleData(string projectId,
            string instanceId, string databaseId)
        {
            var response = CreateDeleteSampleTableAsync(projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");

            response = InsertDeleteSampleDataAsync(projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");

            response = DeleteIndividualRowsAsync(projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");

            response = DeleteRangeOfRowsAsync(projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");

            response = DeleteAllRowsAsync(projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");

            response = DropDeleteSampleTables(projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");

            return ExitCode.Success;
        }

        public static object QuerySampleData(string projectId,
            string instanceId, string databaseId)
        {
            var response = QuerySampleDataAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object QueryDataWithIndex(string projectId,
            string instanceId, string databaseId,
            string startTitle, string endTitle)
        {
            if (string.IsNullOrEmpty(startTitle) ^ string.IsNullOrWhiteSpace(endTitle))
            {
                // Only one Title provided: show message that user must
                // provide both startTitle and endTitle or exclude them
                // from their command.
                Console.WriteLine("Please provide values for both the start "
                    + "of the title index and the end of the title index.");
                Console.WriteLine("Or you can exclude both to run the "
                    + "query with default values.");
                return ExitCode.InvalidParameter;
            }
            // Call method QueryDataWithIndexAsync() based on whether
            // user provided startTitle and endTitle values are empty or null.
            Task response = string.IsNullOrEmpty(startTitle) ?
                // startTitle not provided, exclude startTitle and endTitle from
                // method call to use default values for both.
                QueryDataWithIndexAsync(projectId, instanceId, databaseId) :
                // Both startTitle and endTitle provided, include them both in
                // the method call to QueryDataWithIndexAsync.
                QueryDataWithIndexAsync(projectId, instanceId, databaseId,
                        startTitle, endTitle);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object CreateDatabase(string projectId,
            string instanceId, string databaseId)
        {
            var response = CreateDatabaseAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            Console.WriteLine($"Created database {databaseId} on instance "
                + $"{instanceId}");
            return ExitCode.Success;
        }

        public static object AddIndex(string projectId,
            string instanceId, string databaseId)
        {
            var response = AddIndexAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }
        public static object AddStoringIndex(string projectId,
            string instanceId, string databaseId)
        {
            var response = AddStoringIndexAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static object QueryDataWithStoringIndex(
            string projectId, string instanceId,
            string databaseId, string startTitle, string endTitle)
        {
            var response = new Task(() => { });
            // Call method QueryDataWithStoringIndexAsync() based on whether
            // user provided startTitle and endTitle values are empty or null.
            if (string.IsNullOrEmpty(startTitle))
            {
                // startTitle not provided, exclude startTitle and endTitle from
                // method call to use default values for both.
                response = QueryDataWithStoringIndexAsync(
                        projectId, instanceId, databaseId);
            }
            else if (!string.IsNullOrEmpty(startTitle)
                && string.IsNullOrEmpty(endTitle))
            {
                // Only startTitle provided, show message that user must
                // provide both startTitle and endTitle or exclude them
                // from their command.
                Console.WriteLine("Please provide values for both the start "
                    + "of the title index and the end of the title index.");
                Console.WriteLine("Or you can exclude both to run the "
                    + "query with default values.");
                return ExitCode.InvalidParameter;
            }
            else if (!string.IsNullOrEmpty(startTitle)
                && !string.IsNullOrEmpty(endTitle))
            {
                // Both startTitle and endTitle provided, include them both in
                // the method call to QueryDataWithIndexAsync.
                response = QueryDataWithStoringIndexAsync(
                        projectId, instanceId, databaseId,
                        startTitle, endTitle);
            }
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object AddColumn(string projectId,
            string instanceId, string databaseId)
        {
            var response = AddColumnAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static object WriteDataToNewColumn(string projectId,
            string instanceId, string databaseId)
        {
            var response = WriteDataToNewColumnAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static object QueryNewColumn(string projectId,
            string instanceId, string databaseId)
        {
            var response = QueryNewColumnAsync(projectId,
                                instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static object ListDatabaseTables(string projectId,
            string instanceId, string databaseId)
        {
            var response = ListDatabaseTablesAsync(projectId,
                    instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static object QueryDataWithTransaction(
            string projectId, string instanceId,
            string databaseId, string platform)
        {
            var retryRobot = new RetryRobot { MaxTryCount = 3, DelayMultiplier = 2, ShouldRetry = (e) => e.IsTransientSpannerFault() };

            var response = platform ==
                s_netCorePlatform
                ? retryRobot.Eventually(() =>
                    QueryDataWithTransactionCoreAsync(projectId,
                        instanceId, databaseId))
                : retryRobot.Eventually(() =>
                    QueryDataWithTransactionAsync(projectId,
                        instanceId, databaseId));

            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object ReadWriteWithTransaction(
            string projectId, string instanceId,
            string databaseId, string platform)
        {
            s_logger.Info("Waiting for operation to complete...");
            var response = platform == s_netCorePlatform
                ? Task.Run(async () =>
                {
                    var retryRobot = new RetryRobot
                    {
                        MaxTryCount = 3,
                        DelayMultiplier = 2,
                        ShouldRetry = (e) => e.IsTransientSpannerFault()
                    };

                    await retryRobot.Eventually(() =>
                        ReadWriteWithTransactionCoreAsync(
                            projectId, instanceId, databaseId));
                })
                : Task.Run(async () =>
                {
                    // [START spanner_read_write_retry]
                    var retryRobot = new RetryRobot
                    {
                        MaxTryCount = 3,
                        DelayMultiplier = 2,
                        ShouldRetry = (e) => e.IsTransientSpannerFault()
                    };

                    await retryRobot.Eventually(() =>
                        ReadWriteWithTransactionAsync(
                            projectId, instanceId, databaseId));
                    // [END spanner_read_write_retry]
                });
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static object BatchInsertRecords(string projectId,
            string instanceId, string databaseId)
        {
            var responseTask =
                BatchInsertRecordsAsync(projectId, instanceId, databaseId);
            Console.WriteLine("Waiting for operation to complete...");
            responseTask.Wait();
            Console.WriteLine($"Operation status: {responseTask.Status}");
            Console.WriteLine($"Inserted records into sample database "
                + $"{databaseId} on instance {instanceId}");
            return ExitCode.Success;
        }

        private static async Task BatchInsertRecordsAsync(string projectId,
            string instanceId, string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";

            // Get current max SingerId, start at a minimum SingerId of 20.
            Int64 maxSingerId = 20;
            using (var connection = new SpannerConnection(connectionString))
            {
                // Execute a SQL statement to get current MAX() of SingerId.
                var cmd = connection.CreateSelectCommand(
                    @"SELECT MAX(SingerId) as SingerId FROM Singers");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        long parsedValue;
                        bool result = Int64.TryParse(
                            reader.GetFieldValue<string>("SingerId"), out parsedValue);
                        if (result)
                        {
                            maxSingerId += parsedValue;
                        }
                    }
                }
            }

            // Batch insert 249,900 singer records into the Singers table.
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                for (int i = 0; i < 100; i++)
                {
                    // For details on transaction isolation, see the "Isolation" section in:
                    // https://cloud.google.com/spanner/docs/transactions#read-write_transactions
                    using (var tx = await connection.BeginTransactionAsync())
                    using (var cmd = connection.CreateInsertCommand("Singers", new SpannerParameterCollection
                    {
                        { "SingerId", SpannerDbType.String },
                        { "FirstName", SpannerDbType.String },
                        { "LastName", SpannerDbType.String }
                    }))
                    {
                        cmd.Transaction = tx;
                        for (var x = 1; x < 2500; x++)
                        {
                            maxSingerId++;
                            string nameSuffix = Guid.NewGuid().ToString().Substring(0, 8);
                            cmd.Parameters["SingerId"].Value = maxSingerId;
                            cmd.Parameters["FirstName"].Value = $"FirstName-{nameSuffix}";
                            cmd.Parameters["LastName"].Value = $"LastName-{nameSuffix}";
                            cmd.ExecuteNonQuery();
                        }
                        await tx.CommitAsync();
                    }
                }
            }
            Console.WriteLine("Done inserting sample records...");
        }

        // [START spanner_batch_client]
        private static int s_partitionId;
        private static int s_rowsRead;
        public static object BatchReadRecords(string projectId,
             string instanceId, string databaseId)
        {
            var responseTask =
                DistributedReadAsync(projectId, instanceId, databaseId);
            Console.WriteLine("Waiting for operation to complete...");
            responseTask.Wait();
            Console.WriteLine($"Operation status: {responseTask.Status}");
            return ExitCode.Success;
        }

        private static async Task DistributedReadAsync(string projectId,
            string instanceId, string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var transaction =
                    await connection.BeginReadOnlyTransactionAsync())
                using (var cmd =
                    connection.CreateSelectCommand(
                        "SELECT SingerId, FirstName, LastName FROM Singers"))
                {
                    transaction.DisposeBehavior =
                        DisposeBehavior.CloseResources;
                    cmd.Transaction = transaction;
                    var partitions = await cmd.GetReaderPartitionsAsync();
                    var transactionId = transaction.TransactionId;
                    await Task.WhenAll(partitions.Select(
                            x => DistributedReadWorkerAsync(x, transactionId)))
                                .ConfigureAwait(false);
                }
                Console.WriteLine($"Done reading!  Total rows read: "
                    + $"{s_rowsRead:N0} with {s_partitionId} partition(s)");
            }
        }

        private static async Task DistributedReadWorkerAsync(
            CommandPartition readPartition, TransactionId id)
        {
            var localId = Interlocked.Increment(ref s_partitionId);
            using (var connection = new SpannerConnection(id.ConnectionString))
            using (var transaction = connection.BeginReadOnlyTransaction(id))
            {
                using (var cmd = connection.CreateCommandWithPartition(
                    readPartition, transaction))
                {
                    using (var reader =
                        await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync())
                        {
                            Interlocked.Increment(ref s_rowsRead);
                            Console.WriteLine($"Partition ({localId}) "
                                + $"{reader.GetFieldValue<string>("SingerId")}"
                                + $" {reader.GetFieldValue<string>("FirstName")}"
                                + $" {reader.GetFieldValue<string>("LastName")}");
                        }
                    }
                }
                Console.WriteLine($"Done with single reader {localId}.");
            }
        }
        // [END spanner_batch_client]

        public static object AddCommitTimestamp(string projectId,
              string instanceId, string databaseId)
        {
            var response = AddCommitTimestampAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task AddCommitTimestampAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_add_timestamp_column]
            // Initialize request argument(s).
            string connectionString =
                $"Data Source=projects/{projectId}/instances/"
                + $"{instanceId}/databases/{databaseId}";
            string alterStatement =
                "ALTER TABLE Albums ADD COLUMN LastUpdateTime TIMESTAMP "
                + " OPTIONS (allow_commit_timestamp=true)";
            // Make the request.
            using (var connection = new SpannerConnection(connectionString))
            {
                var updateCmd = connection.CreateDdlCommand(alterStatement);
                await updateCmd.ExecuteNonQueryAsync();
            }
            Console.WriteLine("Added LastUpdateTime as a commit timestamp column in Albums table.");
            // [END spanner_add_timestamp_column]
        }

        public static object UpdateDataWithTimestampColumn(string projectId,
            string instanceId, string databaseId)
        {
            var response = UpdateDataWithTimestampColumnAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task UpdateDataWithTimestampColumnAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_update_data_with_timestamp_column]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateUpdateCommand("Albums",
                    new SpannerParameterCollection {
                        {"SingerId", SpannerDbType.Int64},
                        {"AlbumId", SpannerDbType.Int64},
                        {"MarketingBudget", SpannerDbType.Int64},
                        {"LastUpdateTime", SpannerDbType.Timestamp},
                    });
                var cmdLookup =
                    connection.CreateSelectCommand("SELECT * FROM Albums");
                using (var reader = await cmdLookup.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.GetFieldValue<int>("SingerId") == 1
                            && reader.GetFieldValue<int>("AlbumId") == 1)
                        {
                            cmd.Parameters["SingerId"].Value =
                                reader.GetFieldValue<int>("SingerId");
                            cmd.Parameters["AlbumId"].Value =
                                reader.GetFieldValue<int>("AlbumId");
                            cmd.Parameters["MarketingBudget"].Value = 1000000;
                            cmd.Parameters["LastUpdateTime"].Value =
                                SpannerParameter.CommitTimestamp;
                            await cmd.ExecuteNonQueryAsync();
                        }
                        if (reader.GetInt64(0) == 2 && reader.GetInt64(1) == 2)
                        {
                            cmd.Parameters["SingerId"].Value =
                                reader.GetFieldValue<int>("SingerId");
                            cmd.Parameters["AlbumId"].Value =
                                reader.GetFieldValue<int>("AlbumId");
                            cmd.Parameters["MarketingBudget"].Value = 750000;
                            cmd.Parameters["LastUpdateTime"].Value =
                                SpannerParameter.CommitTimestamp;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            Console.WriteLine("Updated data.");
            // [END spanner_update_data_with_timestamp_column]
        }

        public static object QueryDataWithTimestampColumn(string projectId,
            string instanceId, string databaseId)
        {
            var response = QueryDataWithTimestampColumnAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryDataWithTimestampColumnAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_query_data_with_timestamp_column]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd =
                    connection.CreateSelectCommand(
                        "SELECT SingerId, AlbumId, "
                        + "MarketingBudget, LastUpdateTime FROM Albums");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string budget = string.Empty;
                        if (reader["MarketingBudget"] != DBNull.Value)
                        {
                            budget = reader.GetFieldValue<string>("MarketingBudget");
                        }
                        string timestamp = string.Empty;
                        if (reader["LastUpdateTime"] != DBNull.Value)
                        {
                            timestamp = reader.GetFieldValue<string>("LastUpdateTime");
                        }
                        Console.WriteLine("SingerId : "
                        + reader.GetFieldValue<string>("SingerId")
                        + " AlbumId : "
                        + reader.GetFieldValue<string>("AlbumId")
                        + $" MarketingBudget : {budget}"
                        + $" LastUpdateTime : {timestamp}");
                    }
                }
            }
            // [END spanner_query_data_with_timestamp_column]
        }

        public static object CreateTableWithTimestampColumn(string projectId,
            string instanceId, string databaseId)
        {
            var response = CreateTableWithTimestampColumnAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task CreateTableWithTimestampColumnAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_create_table_with_timestamp_column]
            // Initialize request connection string for database creation.
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";
            using (var connection = new SpannerConnection(connectionString))
            {
                // Define create table statement for table with
                // commit timestamp column.
                string createTableStatement =
                @"CREATE TABLE Performances (
                    SingerId       INT64 NOT NULL,
                    VenueId        INT64 NOT NULL,
                    EventDate      Date,
                    Revenue        INT64,
                    LastUpdateTime TIMESTAMP NOT NULL OPTIONS (allow_commit_timestamp=true)
                ) PRIMARY KEY (SingerId, VenueId, EventDate),
                    INTERLEAVE IN PARENT Singers ON DELETE CASCADE";
                // Make the request.
                var cmd = connection.CreateDdlCommand(createTableStatement);
                await cmd.ExecuteNonQueryAsync();
            }
            // [END spanner_create_table_with_timestamp_column]
        }

        public static object WriteDataWithTimestampColumn(string projectId,
            string instanceId, string databaseId)
        {
            var response = WriteDataWithTimestampAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task WriteDataWithTimestampAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_insert_data_with_timestamp_column]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            List<Performance> performances = new List<Performance> {
                new Performance {SingerId = 1, VenueId = 4, EventDate = DateTime.Parse("2017-10-05"),
                    Revenue = 11000},
                new Performance {SingerId = 1, VenueId = 19, EventDate = DateTime.Parse("2017-11-02"),
                    Revenue = 15000},
                new Performance {SingerId = 2, VenueId = 42, EventDate = DateTime.Parse("2017-12-23"),
                    Revenue = 7000},
            };
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();
                // Insert rows into the Performances table.
                var cmd = connection.CreateInsertCommand("Performances",
                    new SpannerParameterCollection {
                        {"SingerId", SpannerDbType.Int64},
                        {"VenueId", SpannerDbType.Int64},
                        {"EventDate", SpannerDbType.Date},
                        {"Revenue", SpannerDbType.Int64},
                        {"LastUpdateTime", SpannerDbType.Timestamp},
                });
                await Task.WhenAll(performances.Select(performance =>
                {
                    cmd.Parameters["SingerId"].Value = performance.SingerId;
                    cmd.Parameters["VenueId"].Value = performance.VenueId;
                    cmd.Parameters["EventDate"].Value = performance.EventDate;
                    cmd.Parameters["Revenue"].Value = performance.Revenue;
                    cmd.Parameters["LastUpdateTime"].Value = SpannerParameter.CommitTimestamp;
                    return cmd.ExecuteNonQueryAsync();
                }));
                Console.WriteLine("Inserted data.");
            }
            // [END spanner_insert_data_with_timestamp_column]
        }

        public static object QueryNewTableWithTimestampColumn(string projectId,
            string instanceId, string databaseId)
        {
            var response = QueryDataWithTimestampAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryDataWithTimestampAsync(
            string projectId, string instanceId, string databaseId)
        {
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT SingerId, VenueId, EventDate, Revenue, "
                    + "LastUpdateTime FROM Performances "
                    + "ORDER BY LastUpdateTime DESC");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine("SingerId : "
                            + reader.GetFieldValue<string>("SingerId")
                            + " VenueId : "
                            + reader.GetFieldValue<string>("VenueId")
                            + " EventDate : "
                            + reader.GetFieldValue<string>("EventDate")
                            + " Revenue : "
                            + reader.GetFieldValue<string>("Revenue")
                            + " LastUpdateTime : "
                            + reader.GetFieldValue<string>("LastUpdateTime")
                            );
                    }
                }
            }
        }

        public static object QuerySingersTable(string projectId,
            string instanceId, string databaseId)
        {
            var response = QuerySingersTableAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QuerySingersTableAsync(
            string projectId, string instanceId, string databaseId)
        {
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT SingerId, FirstName, LastName FROM Singers");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(reader.GetFieldValue<string>("SingerId")
                            + " " + reader.GetFieldValue<string>("FirstName")
                            + " " + reader.GetFieldValue<string>("LastName")
                        );
                    }
                }
            }
        }

        public static object QueryAlbumsTable(string projectId,
            string instanceId, string databaseId)
        {
            var response = QueryAlbumsTableAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryAlbumsTableAsync(
            string projectId, string instanceId, string databaseId)
        {
            // This query expects the presence of the MarketingBudgetColumn
            // which is added to the Albums table by running
            // the addColumn and writeDataToNewColumn commands, in that order.
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";

            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT SingerId, AlbumId, MarketingBudget FROM Albums");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string budget = string.Empty;
                        if (reader["MarketingBudget"] != DBNull.Value)
                        {
                            budget = reader.GetFieldValue<string>("MarketingBudget");
                        }
                        Console.WriteLine(reader.GetFieldValue<string>("SingerId")
                            + " " + reader.GetFieldValue<string>("AlbumId")
                            + " " + budget
                        );
                    }
                }
            }
        }

        public static object CreateTableWithDatatypes(string projectId,
            string instanceId, string databaseId)
        {
            var response = CreateTableWithDatatypesAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task CreateTableWithDatatypesAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_create_table_with_datatypes]
            // Initialize request connection string for database creation.
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";
            using (var connection = new SpannerConnection(connectionString))
            {
                // Define create table statement for table with 
                // supported datatypes columns.
                string createTableStatement =
                @"CREATE TABLE Venues (
                    VenueId INT64 NOT NULL,
                    VenueName STRING(100),
                    VenueInfo BYTES(MAX),
                    Capacity INT64,                    
                    AvailableDates ARRAY<DATE>,
                    LastContactDate DATE,
                    OutdoorVenue BOOL,
                    PopularityScore FLOAT64,
                    LastUpdateTime TIMESTAMP NOT NULL 
                        OPTIONS (allow_commit_timestamp=true)
                ) PRIMARY KEY (VenueId)";
                // Make the request.
                var cmd = connection.CreateDdlCommand(createTableStatement);
                await cmd.ExecuteNonQueryAsync();
            }
            // [END spanner_create_table_with_datatypes]
        }

        public static object WriteDatatypesData(string projectId,
            string instanceId, string databaseId)
        {
            var response = WriteDatatypesDataAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task WriteDatatypesDataAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_insert_datatypes_data]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            byte[] exampleBytes1 = Encoding.UTF8.GetBytes("Hello World 1");
            byte[] exampleBytes2 = Encoding.UTF8.GetBytes("Hello World 2");
            byte[] exampleBytes3 = Encoding.UTF8.GetBytes("Hello World 3");
            var availableDates1 = new List<DateTime>();
            availableDates1.InsertRange(0, new DateTime[] {
                DateTime.Parse("2020-12-01"),
                DateTime.Parse("2020-12-02"),
                DateTime.Parse("2020-12-03")});
            var availableDates2 = new List<DateTime>();
            availableDates2.InsertRange(0, new DateTime[] {
                DateTime.Parse("2020-11-01"),
                DateTime.Parse("2020-11-05"),
                DateTime.Parse("2020-11-15")});
            var availableDates3 = new List<DateTime>();
            availableDates3.InsertRange(0, new DateTime[] {
                DateTime.Parse("2020-10-01"),
                DateTime.Parse("2020-10-07")});
            List<Venue> venues = new List<Venue> {
                new Venue {VenueId = 4, VenueName = "Venue 4", VenueInfo = exampleBytes1,
                    Capacity = 1800, AvailableDates = availableDates1,
                    LastContactDate = DateTime.Parse("2018-09-02"),
                    OutdoorVenue = false, PopularityScore = 0.85543f},
                new Venue {VenueId = 19, VenueName = "Venue 19", VenueInfo = exampleBytes2,
                    Capacity = 6300, AvailableDates = availableDates2,
                    LastContactDate = DateTime.Parse("2019-01-15"),
                    OutdoorVenue = true, PopularityScore = 0.98716f},
                new Venue {VenueId = 42, VenueName = "Venue 42", VenueInfo = exampleBytes3,
                    Capacity = 3000, AvailableDates = availableDates3,
                    LastContactDate = DateTime.Parse("2018-10-01"),
                    OutdoorVenue = false, PopularityScore = 0.72598f},
            };
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();
                // Insert rows into the Venues table.
                var cmd = connection.CreateInsertCommand("Venues",
                    new SpannerParameterCollection {
                        {"VenueId", SpannerDbType.Int64},
                        {"VenueName", SpannerDbType.String},
                        {"VenueInfo", SpannerDbType.Bytes},
                        {"Capacity", SpannerDbType.Int64},
                        {"AvailableDates", SpannerDbType.ArrayOf(SpannerDbType.Date)},
                        {"LastContactDate", SpannerDbType.Date},
                        {"OutdoorVenue", SpannerDbType.Bool},
                        {"PopularityScore", SpannerDbType.Float64},
                        {"LastUpdateTime", SpannerDbType.Timestamp},
                });
                await Task.WhenAll(venues.Select(venue =>
                {
                    cmd.Parameters["VenueId"].Value = venue.VenueId;
                    cmd.Parameters["VenueName"].Value = venue.VenueName;
                    cmd.Parameters["VenueInfo"].Value = venue.VenueInfo;
                    cmd.Parameters["Capacity"].Value = venue.Capacity;
                    cmd.Parameters["AvailableDates"].Value =
                        venue.AvailableDates;
                    cmd.Parameters["LastContactDate"].Value =
                        venue.LastContactDate;
                    cmd.Parameters["OutdoorVenue"].Value =
                        venue.OutdoorVenue;
                    cmd.Parameters["PopularityScore"].Value =
                        venue.PopularityScore;
                    cmd.Parameters["LastUpdateTime"].Value =
                        SpannerParameter.CommitTimestamp;
                    return cmd.ExecuteNonQueryAsync();
                }));
                Console.WriteLine("Inserted data.");
            }
            // [END spanner_insert_datatypes_data]
        }

        public static object QueryWithArray(string projectId,
            string instanceId, string databaseId)
        {
            var response = QueryWithArrayAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryWithArrayAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_query_with_array_parameter]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            // Create a list array of dates to use for querying.
            var exampleArray = new List<DateTime>();
            exampleArray.InsertRange(0, new DateTime[] {
                DateTime.Parse("2020-10-01"),
                DateTime.Parse("2020-11-01")
                });

            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT VenueId, VenueName, AvailableDate FROM Venues v, "
                    + "UNNEST(v.AvailableDates) as AvailableDate "
                    + "WHERE AvailableDate in UNNEST(@ExampleArray)");
                cmd.Parameters.Add("ExampleArray",
                    SpannerDbType.ArrayOf(SpannerDbType.Date), exampleArray);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(
                            reader.GetFieldValue<string>("VenueId")
                            + " " + reader.GetFieldValue<string>("VenueName")
                            + " " +
                            reader.GetFieldValue<string>("AvailableDate"));
                    }
                }
            }
            // [END spanner_query_with_array_parameter]
        }

        public static object QueryWithBool(string projectId,
            string instanceId, string databaseId)
        {
            var response = QueryWithBoolAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryWithBoolAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_query_with_bool_parameter]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            // Create a Boolean to use for querying.
            Boolean exampleBool = true;
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT VenueId, VenueName, OutdoorVenue FROM Venues "
                    + "WHERE OutdoorVenue = @ExampleBool");
                cmd.Parameters.Add("ExampleBool",
                    SpannerDbType.Bool, exampleBool);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(
                            reader.GetFieldValue<string>("VenueId")
                            + " " + reader.GetFieldValue<string>("VenueName")
                            + " " + reader.
                                GetFieldValue<string>("OutdoorVenue"));
                    }
                }
            }
            // [END spanner_query_with_bool_parameter]
        }

        public static object QueryWithBytes(string projectId,
            string instanceId, string databaseId)
        {
            var response = QueryWithBytesAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryWithBytesAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_query_with_bytes_parameter]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            // Create a Bytes array to use for querying.
            string sampleText = "Hello World 1";
            byte[] exampleBytes = Encoding.UTF8.GetBytes(sampleText);
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT VenueId, VenueName FROM Venues "
                    + "WHERE VenueInfo = @ExampleBytes");
                cmd.Parameters.Add("ExampleBytes",
                    SpannerDbType.Bytes, exampleBytes);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(
                            reader.GetFieldValue<string>("VenueId")
                            + " " + reader.GetFieldValue<string>("VenueName"));
                    }
                }
            }
            // [END spanner_query_with_bytes_parameter]
        }

        public static object QueryWithDate(string projectId,
            string instanceId, string databaseId)
        {
            var response = QueryWithDateAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryWithDateAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_query_with_date_parameter]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            // Create a Date object to use for querying.
            DateTime exampleDate = new DateTime(2019, 01, 01);
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT VenueId, VenueName, LastContactDate FROM Venues "
                    + "WHERE LastContactDate < @ExampleDate");
                cmd.Parameters.Add("ExampleDate",
                    SpannerDbType.Date, exampleDate);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(
                            reader.GetFieldValue<string>("VenueId")
                            + " " + reader.GetFieldValue<string>("VenueName")
                            + " " + reader.
                                GetFieldValue<string>("LastContactDate"));
                    }
                }
            }
            // [END spanner_query_with_date_parameter]
        }

        public static object QueryWithFloat(string projectId,
            string instanceId, string databaseId)
        {
            var response = QueryWithFloatAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryWithFloatAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_query_with_float_parameter]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            // Create a Float object to use for querying.
            float exampleFloat = 0.8f;
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT VenueId, VenueName, PopularityScore FROM Venues "
                    + "WHERE PopularityScore > @ExampleFloat");
                cmd.Parameters.Add("ExampleFloat",
                    SpannerDbType.Float64, exampleFloat);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(
                            reader.GetFieldValue<string>("VenueId")
                            + " " + reader.GetFieldValue<string>("VenueName")
                            + " " + reader.
                                GetFieldValue<string>("PopularityScore"));
                    }
                }
            }
            // [END spanner_query_with_float_parameter]
        }

        public static object QueryWithInt(string projectId,
            string instanceId, string databaseId)
        {
            var response = QueryWithIntAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryWithIntAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_query_with_int_parameter]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            // Create a Int64 object to use for querying.
            Int64 exampleInt = 3000;
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT VenueId, VenueName, Capacity FROM Venues "
                    + "WHERE Capacity >= @ExampleInt");
                cmd.Parameters.Add("ExampleInt",
                    SpannerDbType.Int64, exampleInt);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(
                            reader.GetFieldValue<string>("VenueId")
                            + " " + reader.GetFieldValue<string>("VenueName")
                            + " " + reader.GetFieldValue<string>("Capacity"));
                    }
                }
            }
            // [END spanner_query_with_int_parameter]
        }

        public static object QueryWithString(string projectId,
            string instanceId, string databaseId)
        {
            var response = QueryWithStringAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryWithStringAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_query_with_string_parameter]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            // Create a String object to use for querying.
            String exampleString = "Venue 42";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT VenueId, VenueName FROM Venues "
                    + "WHERE VenueName = @ExampleString");
                cmd.Parameters.Add("ExampleString",
                    SpannerDbType.String, exampleString);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(
                            reader.GetFieldValue<string>("VenueId")
                            + " " + reader.GetFieldValue<string>("VenueName"));
                    }
                }
            }
            // [END spanner_query_with_string_parameter]
        }

        public static object QueryWithTimestamp(string projectId,
            string instanceId, string databaseId)
        {
            var response = QueryWithTimestampAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryWithTimestampAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_query_with_timestamp_parameter]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            // Create a DateTime timestamp object to use for querying.
            DateTime exampleTimestamp = DateTime.Now;
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT VenueId, VenueName, LastUpdateTime FROM Venues "
                    + "WHERE LastUpdateTime < @ExampleTimestamp");
                cmd.Parameters.Add("ExampleTimestamp",
                    SpannerDbType.Timestamp, exampleTimestamp);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string timestamp = string.Empty;
                        if (reader["LastUpdateTime"] != DBNull.Value)
                        {
                            timestamp = reader.GetFieldValue<string>("LastUpdateTime");
                        }
                        Console.WriteLine("VenueId : "
                        + reader.GetFieldValue<string>("VenueId")
                        + " VenueName : "
                        + reader.GetFieldValue<string>("VenueName")
                        + $" LastUpdateTime : {timestamp}");
                    }
                }
            }
            // [END spanner_query_with_timestamp_parameter]
        }

        public static object InsertStructSampleData(
            string projectId, string instanceId, string databaseId)
        {
            var response = InsertStructSampleDataAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task InsertStructSampleDataAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_write_data_for_struct_queries]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            List<Singer> singers = new List<Singer> {
                new Singer {SingerId = 6, FirstName = "Elena",
                    LastName = "Campbell"},
                new Singer {SingerId = 7, FirstName = "Gabriel",
                    LastName = "Wright"},
                new Singer {SingerId = 8, FirstName = "Benjamin",
                    LastName = "Martinez"},
                new Singer {SingerId = 9, FirstName = "Hannah",
                    LastName = "Harris"},
            };
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();

                // Insert rows into the Singers table.
                var cmd = connection.CreateInsertCommand("Singers",
                    new SpannerParameterCollection {
                        {"SingerId", SpannerDbType.Int64},
                        {"FirstName", SpannerDbType.String},
                        {"LastName", SpannerDbType.String}
                });
                await Task.WhenAll(singers.Select(singer =>
                {
                    cmd.Parameters["SingerId"].Value = singer.SingerId;
                    cmd.Parameters["FirstName"].Value = singer.FirstName;
                    cmd.Parameters["LastName"].Value = singer.LastName;
                    return cmd.ExecuteNonQueryAsync();
                }));
                Console.WriteLine("Inserted struct data.");
            }
            // [END spanner_write_data_for_struct_queries]
        }

        public static object QueryDataWithStruct(
            string projectId, string instanceId, string databaseId)
        {
            var response = QueryDataWithStructAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryDataWithStructAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_create_struct_with_data]
            var nameStruct = new SpannerStruct
            {
                { "FirstName", SpannerDbType.String, "Elena" },
                { "LastName", SpannerDbType.String, "Campbell" },
            };
            // [END spanner_create_struct_with_data]

            // [START spanner_query_data_with_struct]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            using (var connection = new SpannerConnection(connectionString))
            {
                using (var cmd = connection.CreateSelectCommand(
                    "SELECT SingerId FROM Singers "
                    + "WHERE STRUCT<FirstName STRING, LastName STRING>"
                    + "(FirstName, LastName) = @name"))
                {
                    cmd.Parameters.Add("name", nameStruct.GetSpannerDbType(), nameStruct);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine(
                                reader.GetFieldValue<string>("SingerId"));
                        }
                    }
                }
            }
            // [END spanner_query_data_with_struct]
        }

        public static object QueryDataWithArrayOfStruct(
            string projectId, string instanceId, string databaseId)
        {
            var response = QueryDataWithArrayOfStructAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryDataWithArrayOfStructAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_create_user_defined_struct]
            var nameType = new SpannerStruct {
                { "FirstName", SpannerDbType.String, null},
                { "LastName", SpannerDbType.String, null}
            };
            // [END spanner_create_user_defined_struct]

            // [START spanner_create_array_of_struct_with_data]
            var bandMembers = new List<SpannerStruct>
            {
                new SpannerStruct { { "FirstName", SpannerDbType.String, "Elena" },
                    { "LastName", SpannerDbType.String, "Campbell" } },
                new SpannerStruct { { "FirstName", SpannerDbType.String, "Gabriel" },
                    { "LastName", SpannerDbType.String, "Wright" } },
                new SpannerStruct { { "FirstName", SpannerDbType.String, "Benjamin" },
                    { "LastName", SpannerDbType.String, "Martinez" } },
            };
            // [END spanner_create_array_of_struct_with_data]


            // [START spanner_query_data_with_array_of_struct]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            using (var connection = new SpannerConnection(connectionString))
            {
                using (var cmd = connection.CreateSelectCommand(
                    "SELECT SingerId FROM Singers "
                    + "WHERE STRUCT<FirstName STRING, LastName STRING>"
                    + "(FirstName, LastName) IN UNNEST(@names)"))
                {
                    cmd.Parameters.Add("names",
                        SpannerDbType.ArrayOf(nameType.GetSpannerDbType()),
                            bandMembers);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine(
                                reader.GetFieldValue<string>("SingerId"));
                        }
                    }
                }
            }
            // [END spanner_query_data_with_array_of_struct]
        }

        public static object QueryDataWithStructField(
            string projectId, string instanceId, string databaseId)
        {
            var response = QueryDataWithStructFieldAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }


        public static async Task QueryDataWithStructFieldAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_field_access_on_struct_parameters]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            var structParam = new SpannerStruct
            {
                { "FirstName", SpannerDbType.String, "Elena" },
                { "LastName", SpannerDbType.String, "Campbell" },
            };
            using (var connection = new SpannerConnection(connectionString))
            {
                using (var cmd = connection.CreateSelectCommand(
                    "SELECT SingerId FROM Singers WHERE FirstName = @name.FirstName"))
                {
                    cmd.Parameters.Add("name", structParam.GetSpannerDbType(), structParam);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine(
                                reader.GetFieldValue<string>("SingerId"));
                        }
                    }
                }
            }
            // [END spanner_field_access_on_struct_parameters]
        }

        public static object QueryDataWithNestedStructField(
            string projectId, string instanceId, string databaseId)
        {
            var response = QueryDataWithNestedStructFieldAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        public static async Task QueryDataWithNestedStructFieldAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_field_access_on_nested_struct_parameters]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            SpannerStruct name1 = new SpannerStruct
            {
                { "FirstName", SpannerDbType.String, "Elena" },
                { "LastName", SpannerDbType.String, "Campbell" }
            };
            SpannerStruct name2 = new SpannerStruct
            {
                { "FirstName", SpannerDbType.String, "Hannah" },
                { "LastName", SpannerDbType.String, "Harris" }
            };
            SpannerStruct songInfo = new SpannerStruct
            {
                { "song_name", SpannerDbType.String, "Imagination" },
                { "artistNames", SpannerDbType.ArrayOf(name1.GetSpannerDbType()), new[] { name1, name2 } }
            };

            using (var connection = new SpannerConnection(connectionString))
            {
                using (var cmd = connection.CreateSelectCommand(
                    "SELECT SingerId, @song_info.song_name "
                    + "FROM Singers WHERE STRUCT<FirstName STRING, LastName STRING>(FirstName, LastName) "
                    + "IN UNNEST(@song_info.artistNames)"))
                {
                    cmd.Parameters.Add("song_info",
                        songInfo.GetSpannerDbType(),
                            songInfo);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine(
                                reader.GetFieldValue<string>("SingerId"));
                            Console.WriteLine(
                                reader.GetFieldValue<string>(1));
                        }
                    }
                }
            }
            // [END spanner_field_access_on_nested_struct_parameters]
        }

        public static object DeleteDatabase(string projectId,
            string instanceId, string databaseId)
        {
            var response =
                DeleteDatabaseAsync(projectId, instanceId, databaseId);
            Console.WriteLine("Waiting for operation to complete...");
            response.Wait();
            Console.WriteLine($"Operation status: {response.Status}");
            Console.WriteLine($"Deleted sample database {databaseId} on "
                + $"instance {instanceId}");
            return ExitCode.Success;
        }

        private static async Task DeleteDatabaseAsync(string projectId,
            string instanceId, string databaseId)
        {
            // The IAM Role "Cloud Spanner Database Admin" is required for
            // performing Administration operations on the database.
            // For more info see https://cloud.google.com/spanner/docs/iam#roles
            string AdminConnectionString = $"Data Source=projects/{projectId}/"
                + $"instances/{instanceId}";
            using (var connection = new SpannerConnection(AdminConnectionString))
            using (var cmd = connection.CreateDdlCommand(
                $@"DROP DATABASE {databaseId}"))
            {
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            Console.WriteLine($"Done deleting database: {databaseId}");
        }

        public static object InsertUsingDml(string projectId,
            string instanceId, string databaseId)
        {
            var response = InsertUsingDmlCoreAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object UpdateUsingDml(string projectId,
            string instanceId, string databaseId)
        {
            var response = UpdateUsingDmlCoreAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object DeleteUsingDml(string projectId,
            string instanceId, string databaseId)
        {
            var response = DeleteUsingDmlCoreAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object UpdateUsingDmlWithTimestamp(string projectId,
            string instanceId, string databaseId)
        {
            var response = UpdateUsingDmlWithTimestampCoreAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object WriteAndReadUsingDml(string projectId,
            string instanceId, string databaseId)
        {
            var response = WriteAndReadUsingDmlCoreAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object UpdateUsingDmlWithStruct(string projectId,
            string instanceId, string databaseId)
        {
            var response = UpdateUsingDmlWithStructCoreAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object WriteUsingDml(string projectId,
            string instanceId, string databaseId)
        {
            var response = WriteUsingDmlCoreAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object QueryWithParameter(string projectId,
            string instanceId, string databaseId)
        {
            var response = QueryWithParameterAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object WriteWithTransactionUsingDml(string projectId,
            string instanceId, string databaseId)
        {
            var response = WriteWithTransactionUsingDmlCoreAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object UpdateUsingPartitionedDml(string projectId,
            string instanceId, string databaseId)
        {
            var response = UpdateUsingPartitionedDmlCoreAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object DeleteUsingPartitionedDml(string projectId,
            string instanceId, string databaseId)
        {
            var response = DeleteUsingPartitionedDmlCoreAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object UpdateUsingBatchDml(string projectId,
            string instanceId, string databaseId)
        {
            var response = UpdateUsingBatchDmlCoreAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Operation status: {response.Status}");
            return ExitCode.Success;
        }

        public static object CreateConnectionWithQueryOptions(
            string projectId, string instanceId, string databaseId)
        {
            var response = CreateConnectionWithQueryOptionsAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        private static async Task CreateConnectionWithQueryOptionsAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_create_client_with_query_options]
            var builder = new SpannerConnectionStringBuilder
            {
                DataSource = $"projects/{projectId}/instances/{instanceId}/databases/{databaseId}"
            };
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(builder))
            {
                // Set query options on the connection.
                connection.QueryOptions = QueryOptions.Empty.WithOptimizerVersion("1");
                var cmd = connection.CreateSelectCommand(
                    "SELECT SingerId, AlbumId, AlbumTitle FROM Albums");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine("SingerId : "
                        + reader.GetFieldValue<string>("SingerId")
                        + " AlbumId : "
                        + reader.GetFieldValue<string>("AlbumId")
                        + " AlbumTitle : "
                        + reader.GetFieldValue<string>("AlbumTitle"));
                    }
                }
            }
            // [END spanner_create_client_with_query_options]
        }

        public static object RunCommandWithQueryOptions(
            string projectId, string instanceId, string databaseId)
        {
            var response = RunCommandWithQueryOptionsAsync(
                projectId, instanceId, databaseId);
            s_logger.Info("Waiting for operation to complete...");
            response.Wait();
            s_logger.Info($"Response status: {response.Status}");
            return ExitCode.Success;
        }

        private static async Task RunCommandWithQueryOptionsAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START spanner_query_with_query_options]
            var builder = new SpannerConnectionStringBuilder
            {
                DataSource = $"projects/{projectId}/instances/{instanceId}/databases/{databaseId}"
            };
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(builder))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT SingerId, AlbumId, AlbumTitle FROM Albums");
                // Set query options just for this command.
                cmd.QueryOptions = QueryOptions.Empty.WithOptimizerVersion("1");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine("SingerId : "
                        + reader.GetFieldValue<string>("SingerId")
                        + " AlbumId : "
                        + reader.GetFieldValue<string>("AlbumId")
                        + " AlbumTitle : "
                        + reader.GetFieldValue<string>("AlbumTitle"));
                    }
                }
            }
            // [END spanner_query_with_query_options]
        }

        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<object>();
            verbMap
                .Add((CreateSampleDatabaseOptions opts) =>
                    CreateSampleDatabase(opts.projectId, opts.instanceId,
                        opts.databaseId))
                .Add((CreateDatabaseOptions opts) => CreateDatabase(
                    opts.projectId, opts.instanceId, opts.databaseId))
                .Add((DeleteSampleDataOptions opts) => DeleteSampleData(
                        opts.projectId, opts.instanceId, opts.databaseId))
                .Add((InsertSampleDataOptions opts) => InsertSampleData(
                    opts.projectId, opts.instanceId, opts.databaseId))
                .Add((QuerySampleDataOptions opts) => QuerySampleData(
                    opts.projectId, opts.instanceId, opts.databaseId))
                .Add((AddColumnOptions opts) => AddColumn(
                    opts.projectId, opts.instanceId, opts.databaseId))
                .Add((WriteDataToNewColumnOptions opts) => WriteDataToNewColumn(
                    opts.projectId, opts.instanceId, opts.databaseId))
                .Add((QueryNewColumnOptions opts) => QueryNewColumn(
                    opts.projectId, opts.instanceId, opts.databaseId))
                .Add((QueryDataWithTransactionOptions opts) =>
                    QueryDataWithTransaction(
                        opts.projectId, opts.instanceId, opts.databaseId,
                        opts.platform))
                .Add((ReadWriteWithTransactionOptions opts) =>
                    ReadWriteWithTransaction(
                        opts.projectId, opts.instanceId, opts.databaseId,
                        opts.platform))
                .Add((AddIndexOptions opts) => AddIndex(
                    opts.projectId, opts.instanceId, opts.databaseId))
                .Add((AddStoringIndexOptions opts) => AddStoringIndex(
                    opts.projectId, opts.instanceId, opts.databaseId))
                .Add((QueryDataWithIndexOptions opts) => QueryDataWithIndex(
                    opts.projectId, opts.instanceId, opts.databaseId,
                    opts.startTitle, opts.endTitle))
                .Add((QueryDataWithStoringIndexOptions opts) =>
                    QueryDataWithStoringIndex(
                        opts.projectId, opts.instanceId, opts.databaseId,
                        opts.startTitle, opts.endTitle))
                .Add((ReadStaleDataOptions opts) =>
                    ReadStaleDataAsync(opts.projectId, opts.instanceId,
                        opts.databaseId).Result)
                .Add((InsertStructSampleDataOptions opts) =>
                    InsertStructSampleData(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryDataWithStructOptions opts) =>
                    QueryDataWithStruct(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryDataWithArrayOfStructOptions opts) =>
                    QueryDataWithArrayOfStruct(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryDataWithStructFieldOptions opts) =>
                    QueryDataWithStructField(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryDataWithNestedStructFieldOptions opts) =>
                    QueryDataWithNestedStructField(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((BatchInsertOptions opts) =>
                    BatchInsertRecords(opts.projectId, opts.instanceId,
                        opts.databaseId))
                .Add((BatchReadOptions opts) =>
                    BatchReadRecords(opts.projectId, opts.instanceId,
                        opts.databaseId))
                .Add((AddCommitTimestampOptions opts) =>
                    AddCommitTimestamp(opts.projectId, opts.instanceId,
                        opts.databaseId))
                .Add((UpdateDataWithTimestampOptions opts) =>
                    UpdateDataWithTimestampColumn(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryDataWithTimestampOptions opts) =>
                    QueryDataWithTimestampColumn(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((CreateTableWithTimestampOptions opts) =>
                    CreateTableWithTimestampColumn(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((WriteDataWithTimestampOptions opts) =>
                    WriteDataWithTimestampColumn(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryNewTableWithTimestampOptions opts) =>
                    QueryNewTableWithTimestampColumn(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QuerySingersTableOptions opts) =>
                    QuerySingersTable(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryAlbumsTableOptions opts) =>
                    QueryAlbumsTable(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((InsertUsingDmlOptions opts) =>
                    InsertUsingDml(opts.projectId, opts.instanceId,
                    opts.databaseId))
                .Add((UpdateUsingDmlOptions opts) =>
                    UpdateUsingDml(opts.projectId, opts.instanceId,
                    opts.databaseId))
                .Add((DeleteUsingDmlOptions opts) =>
                    DeleteUsingDml(opts.projectId, opts.instanceId,
                    opts.databaseId))
                .Add((UpdateUsingDmlWithTimestampOptions opts) =>
                    UpdateUsingDmlWithTimestamp(opts.projectId, opts.instanceId,
                    opts.databaseId))
                .Add((WriteAndReadUsingDmlOptions opts) =>
                    WriteAndReadUsingDml(opts.projectId, opts.instanceId,
                    opts.databaseId))
                .Add((UpdateUsingDmlWithStructOptions opts) =>
                    UpdateUsingDmlWithStruct(opts.projectId, opts.instanceId,
                    opts.databaseId))
                .Add((WriteUsingDmlOptions opts) =>
                    WriteUsingDml(opts.projectId, opts.instanceId,
                    opts.databaseId))
                .Add((QueryWithParameterOptions opts) =>
                    QueryWithParameter(opts.projectId, opts.instanceId,
                    opts.databaseId))
                .Add((WriteWithTransactionUsingDmlOptions opts) =>
                    WriteWithTransactionUsingDml(opts.projectId, opts.instanceId,
                    opts.databaseId))
                .Add((UpdateUsingPartitionedDmlOptions opts) =>
                    UpdateUsingPartitionedDml(opts.projectId, opts.instanceId,
                    opts.databaseId))
                .Add((DeleteUsingPartitionedDmlOptions opts) =>
                    DeleteUsingPartitionedDml(opts.projectId, opts.instanceId,
                    opts.databaseId))
                .Add((UpdateUsingBatchDmlOptions opts) =>
                    UpdateUsingBatchDml(opts.projectId, opts.instanceId,
                    opts.databaseId))
                .Add((CreateTableWithDatatypesOptions opts) =>
                    CreateTableWithDatatypes(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((WriteDatatypesDataOptions opts) =>
                    WriteDatatypesData(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryWithArrayOptions opts) =>
                    QueryWithArray(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryWithBoolOptions opts) =>
                    QueryWithBool(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryWithBytesOptions opts) =>
                    QueryWithBytes(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryWithDateOptions opts) =>
                    QueryWithDate(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryWithFloatOptions opts) =>
                    QueryWithFloat(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryWithIntOptions opts) =>
                    QueryWithInt(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryWithStringOptions opts) =>
                    QueryWithString(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((QueryWithTimestampOptions opts) =>
                    QueryWithTimestamp(opts.projectId,
                        opts.instanceId, opts.databaseId))
                .Add((ListDatabaseTablesOptions opts) =>
                    ListDatabaseTables(opts.projectId, opts.instanceId,
                        opts.databaseId))
                .Add((DeleteOptions opts) =>
                    DeleteDatabase(opts.projectId, opts.instanceId,
                        opts.databaseId))
                .Add((DropSampleTablesOptions opts) =>
                    DropSampleTables(opts.projectId, opts.instanceId,
                    opts.databaseId).Result)
                .Add((CreateBackupOptions opts) =>
                    CreateBackup.SpannerCreateBackup(
                        opts.projectId, opts.instanceId, opts.databaseId,
                        opts.backupId))
                .Add((CancelBackupOperationOptions opts) =>
                    CancelBackupOperation.SpannerCancelBackupOperation(
                        opts.projectId, opts.instanceId, opts.databaseId,
                        opts.backupId))
                .Add((GetBackupsOptions opts) =>
                    GetBackups.SpannerGetBackups(
                        opts.projectId, opts.instanceId, opts.databaseId,
                        opts.backupId))
                .Add((RestoreDatabaseOptions opts) =>
                    RestoreDatabase.SpannerRestoreDatabase(
                        opts.projectId, opts.instanceId, opts.databaseId,
                        opts.backupId))
                .Add((UpdateBackupOptions opts) =>
                    UpdateBackup.SpannerUpdateBackup(
                        opts.projectId, opts.instanceId, opts.backupId))
                .Add((GetDatabaseOperationsOptions opts) =>
                    GetDatabaseOperations.SpannerGetDatabaseOperations(
                        opts.projectId, opts.instanceId))
                .Add((GetBackupOperationsOptions opts) =>
                    GetBackupOperations.SpannerGetBackupOperations(
                        opts.projectId, opts.instanceId, opts.databaseId))
                .Add((DeleteBackupOptions opts) =>
                    DeleteBackup.SpannerDeleteBackup(
                        opts.projectId, opts.instanceId, opts.backupId))
                .Add((CreateConnectionWithQueryOptionsOptions opts) =>
                    CreateConnectionWithQueryOptions(opts.projectId,
                    opts.instanceId, opts.databaseId))
                .Add((RunCommandWithQueryOptionsOptions opts) =>
                    RunCommandWithQueryOptions(opts.projectId,
                    opts.instanceId, opts.databaseId))
                .NotParsedFunc = (err) => 1;
            return (int)verbMap.Run(args);
        }

        /// <summary>
        /// Returns true if an AggregateException contains a SpannerException
        /// with the given error code.
        /// </summary>
        /// <param name="e">The exception to examine.</param>
        /// <param name="errorCode">The error code to look for.</param>
        /// <returns></returns>
        public static bool ContainsError(AggregateException e, params ErrorCode[] errorCode)
        {
            foreach (var innerException in e.InnerExceptions)
            {
                SpannerException spannerException = innerException as SpannerException;
                if (spannerException != null && errorCode.Contains(spannerException.ErrorCode))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if an AggregateException contains a Grpc.Core.RpcException
        /// with the given error code.
        /// </summary>
        /// <param name="e">The exception to examine.</param>
        /// <param name="errorCode">The error code to look for.</param>
        /// <returns></returns>
        public static bool ContainsGrpcError(AggregateException e,
            Grpc.Core.StatusCode errorCode)
        {
            foreach (var innerException in e.InnerExceptions)
            {
                Grpc.Core.RpcException grpcException = innerException
                    as Grpc.Core.RpcException;
                if (grpcException != null &&
                    grpcException.Status.StatusCode == errorCode)
                    return true;
            }
            return false;
        }
    }
}
