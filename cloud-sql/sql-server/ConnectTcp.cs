/*
 * Copyright (c) 2022 Google LLC.
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

// [START cloud_sql_sqlserver_dotnet_ado_connect_tcp]
// [START cloud_sql_sqlserver_dotnet_ado_connect_tcp_sslcerts]
using Microsoft.Data.SqlClient;
using System;

namespace CloudSql
{
    public class SqlServerTcp
    {
        public static SqlConnectionStringBuilder NewSqlServerTCPConnectionString()
        {
            // Equivalent connection string:
            // "User Id=<DB_USER>;Password=<DB_PASS>;Server=<INSTANCE_HOST>;Database=<DB_NAME>;"
            var connectionString = new SqlConnectionStringBuilder()
            {
                // Note: Saving credentials in environment variables is convenient, but not
                // secure - consider a more secure solution such as
                // Cloud Secret Manager (https://cloud.google.com/secret-manager) to help
                // keep secrets safe.
                DataSource = Environment.GetEnvironmentVariable("INSTANCE_HOST"), // e.g. '127.0.0.1'
                // Set Host to 'cloudsql' when deploying to App Engine Flexible environment
                UserID = Environment.GetEnvironmentVariable("DB_USER"),         // e.g. 'my-db-user'
                Password = Environment.GetEnvironmentVariable("DB_PASS"),       // e.g. 'my-db-password'
                InitialCatalog = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'

                // [END cloud_sql_sqlserver_dotnet_ado_connect_tcp_sslcerts]
                // The Cloud SQL proxy provides encryption between the proxy and instance
                Encrypt = false,
                // [START cloud_sql_sqlserver_dotnet_ado_connect_tcp_sslcerts]
            };
            // [END cloud_sql_sqlserver_dotnet_ado_connect_tcp]
            // For deployments that connect directly to a Cloud SQL for SQL Server instance
            // that has "Allow only SSL connections" enabled, without using the Cloud SQL Proxy,
            // configure encryption as Mandatory.
            if (Environment.GetEnvironmentVariable("PRIVATE_IP") != null)
            {
                connectionString.Encrypt = SqlConnectionEncryptOption.Mandatory;
            }
            // [START cloud_sql_sqlserver_dotnet_ado_connect_tcp]
            connectionString.Pooling = true;
            // Specify additional properties here.
            return connectionString;
        }
    }
}
// [END cloud_sql_sqlserver_dotnet_ado_connect_tcp_sslcerts]
// [END cloud_sql_sqlserver_dotnet_ado_connect_tcp]
