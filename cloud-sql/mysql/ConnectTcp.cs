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

// [START cloud_sql_mysql_dotnet_ado_connect_tcp]
// [START cloud_sql_mysql_dotnet_ado_connect_tcp_sslcerts]
using MySql.Data.MySqlClient;
using System;

namespace CloudSql
{
    public class MySqlTcp
    {
        public static MySqlConnectionStringBuilder NewMysqlTCPConnectionString()
        {
            // Equivalent connection string:
            // "Uid=<DB_USER>;Pwd=<DB_PASS>;Host=<INSTANCE_HOST>;Database=<DB_NAME>;"
            var connectionString = new MySqlConnectionStringBuilder()
            {
                // Note: Saving credentials in environment variables is convenient, but not
                // secure - consider a more secure solution such as
                // Cloud Secret Manager (https://cloud.google.com/secret-manager) to help
                // keep secrets safe.
                Server = Environment.GetEnvironmentVariable("INSTANCE_HOST"),   // e.g. '127.0.0.1'
                // Set Host to 'cloudsql' when deploying to App Engine Flexible environment
                UserID = Environment.GetEnvironmentVariable("DB_USER"),   // e.g. 'my-db-user'
                Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
                Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'

                // [END cloud_sql_mysql_dotnet_ado_connect_tcp_sslcerts]
                // The Cloud SQL proxy provides encryption between the proxy and instance.
                SslMode = MySqlSslMode.Disabled,
                // [START cloud_sql_mysql_dotnet_ado_connect_tcp_sslcerts]
            };
            // [END cloud_sql_mysql_dotnet_ado_connect_tcp]
            // For deployments that connect directly to a Cloud SQL instance without
            // using the Cloud SQL Proxy, configuring SSL certificates will ensure the
            // connection is encrypted.
            if (Environment.GetEnvironmentVariable("DB_CERT") != null)
            {
                connectionString.SslMode = MySqlSslMode.VerifyCA;
                connectionString.CertificateFile = 
                    Environment.GetEnvironmentVariable("DB_CERT"); // e.g. 'certs/client.pfx'
            }
            // [START cloud_sql_mysql_dotnet_ado_connect_tcp]
            connectionString.Pooling = true;
            // Specify additional properties here.
            return connectionString;

        }
    }
}
// [END cloud_sql_mysql_dotnet_ado_connect_tcp_sslcerts]
// [END cloud_sql_mysql_dotnet_ado_connect_tcp]
