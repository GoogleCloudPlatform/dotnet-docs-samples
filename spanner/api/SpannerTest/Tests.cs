// Copyright(c) 2017 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using Google.Cloud.Spanner.Data;
using System;
using Xunit;
using Xunit.Sdk;

namespace GoogleCloudSamples.Spanner
{
    public class QuickStartTests
    {
        [Fact]
        public void TestQuickStart()
        {
            CommandLineRunner runner = new CommandLineRunner()
            {
                VoidMain = QuickStart.Main,
                Command = "QuickStart"
            };
            var result = runner.Run();
            Assert.Equal(0, result.ExitCode);
        }
    }

    public class SpannerTests
    {
        private static readonly string s_projectId =
            Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        // Allow environment variables to override the default instance and database names.
        private static readonly string s_instanceId =
            Environment.GetEnvironmentVariable("TEST_SPANNER_INSTANCE") ?? "my-instance";
        private static readonly string s_databaseId =
            Environment.GetEnvironmentVariable("TEST_SPANNER_DATABASE") ?? "my-database";

        readonly CommandLineRunner _spannerCmd = new CommandLineRunner()
        {
            VoidMain = Program.Main,
            Command = "Spanner"
        };

        [Fact]
        void TestQuery()
        {
            // If the database has not been initialized, retry.
            try
            {
                QuerySampleData();
                return;
            }
            catch (AggregateException e) when (ContainsError(e, ErrorCode.NotFound))
            {
                // The database does not exist.  Create database and retry.
                _spannerCmd.Run("createSampleDatabase",
                    s_projectId, s_instanceId, s_databaseId);
                _spannerCmd.Run("insertSampleData",
                    s_projectId, s_instanceId, s_databaseId);
                QuerySampleData();
            }
            catch (XunitException)
            {
                // The database does not contain the expected datae.
                // Insert sample data and retry.
                _spannerCmd.Run("insertSampleData",
                    s_projectId, s_instanceId, s_databaseId);
                QuerySampleData();
            }
        }

        [Fact]
        void TestQueryTransaction()
        {
            ConsoleOutput output = _spannerCmd.Run("queryDataWithTransaction",
                s_projectId, s_instanceId, s_databaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
        }

        [Fact]
        void TestQueryTransactionCore()
        {
            ConsoleOutput output = _spannerCmd.Run("queryDataWithTransaction",
                s_projectId, s_instanceId, s_databaseId, "netcore");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
        }

        [Fact]
        void TestReadWriteTransaction()
        {
            ConsoleOutput output = _spannerCmd.Run("readWriteWithTransaction",
                s_projectId, s_instanceId, s_databaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Transaction complete.", output.Stdout);
        }

        [Fact]
        void TestReadWriteTransactionCore()
        {
            ConsoleOutput output = _spannerCmd.Run("readWriteWithTransaction",
                s_projectId, s_instanceId, s_databaseId, "netcore");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Transaction complete.", output.Stdout);
        }

        [Fact]
        void TestSpannerNoArgsSucceeds()
        {
            ConsoleOutput output = _spannerCmd.Run();
            Assert.Equal(0, output.ExitCode);
        }

        /// <summary>
        /// Run a couple queries and verify the database contains the
        /// data inserted by insertSampleData.
        /// </summary>
        /// <exception cref="XunitException">when an assertion fails.</exception>
        void QuerySampleData()
        {
            ConsoleOutput output = _spannerCmd.Run("querySampleData",
                s_projectId, s_instanceId, s_databaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
        }

        /// <summary>
        /// Returns true if an AggregateException contains a SpannerException
        /// with the given error code.
        /// </summary>
        /// <param name="e">The exception to examine.</param>
        /// <param name="errorCode">The error code to look for.</param>
        /// <returns></returns>
        static bool ContainsError(AggregateException e, ErrorCode errorCode)
        {
            foreach (var innerException in e.InnerExceptions)
            {
                SpannerException spannerException = innerException as SpannerException;
                if (spannerException != null && spannerException.ErrorCode == errorCode)
                    return true;
            }
            return false;
        }
    }
}
