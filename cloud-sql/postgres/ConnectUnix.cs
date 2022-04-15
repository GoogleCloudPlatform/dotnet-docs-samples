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

// [START cloud_sql_postgres_dotnet_ado_connect_unix]
using Npgsql;
using System;

namespace CloudSql
{
    public class PostgreSqlUnix
    {
        public static NpgsqlConnectionStringBuilder NewPostgreSqlUnixSocketConnectionString()
        {
            // Equivalent connection string:
            // "Server=<dbSocketDir>/<INSTANCE_CONNECTION_NAME>;Uid=<DB_USER>;Pwd=<DB_PASS>;Database=<DB_NAME>"
            String dbSocketDir = Environment.GetEnvironmentVariable("INSTANCE_UNIX_SOCKET") ?? "/cloudsql";
            String instanceConnectionName = Environment.GetEnvironmentVariable("INSTANCE_CONNECTION_NAME");
            var connectionString = new NpgsqlConnectionStringBuilder()
            {
                // The Cloud SQL proxy provides encryption between the proxy and instance.
                SslMode = SslMode.Disable,
                // Remember - storing secrets in plain text is potentially unsafe. Consider using
                // something like https://cloud.google.com/secret-manager/docs/overview to help keep
                // secrets secret.
                Host = String.Format("{0}/{1}", dbSocketDir, instanceConnectionName),
                Username = Environment.GetEnvironmentVariable("DB_USER"), // e.g. 'my-db-user
                Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
                Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'
            };
            connectionString.Pooling = true;
            // Specify additional properties here.
            return connectionString;
        }
    }
}
// [END cloud_sql_postgres_dotnet_ado_connect_unix]
