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

// [START cloud_sql_mysql_dotnet_ado_connect_unix]
using MySql.Data.MySqlClient;
using System;

namespace CloudSql
{
    public class MySqlUnix
    {
        public static MySqlConnectionStringBuilder NewMysqlUnixSocketConnectionString()
        {
            // Equivalent connection string:
            // "Server=<INSTANCE_UNIX_SOCKET>;Uid=<DB_USER>;Pwd=<DB_PASS>;Database=<DB_NAME>;Protocol=unix"
            var connectionString = new MySqlConnectionStringBuilder()
            {
                // The Cloud SQL proxy provides encryption between the proxy and instance.
                SslMode = MySqlSslMode.Disabled,

                // Note: Saving credentials in environment variables is convenient, but not
                // secure - consider a more secure solution such as
                // Cloud Secret Manager (https://cloud.google.com/secret-manager) to help
                // keep secrets safe.
                Server = Environment.GetEnvironmentVariable("INSTANCE_UNIX_SOCKET"), // e.g. '/cloudsql/project:region:instance'
                UserID = Environment.GetEnvironmentVariable("DB_USER"),   // e.g. 'my-db-user
                Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
                Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'
                ConnectionProtocol = MySqlConnectionProtocol.UnixSocket
            };
            connectionString.Pooling = true;
            // Specify additional properties here.
            return connectionString;
        }
    }
}
// [END cloud_sql_mysql_dotnet_ado_connect_unix]
