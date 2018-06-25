// Copyright(c) 2018 Google Inc.
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

using System;
using Xunit;
using Google.Cloud.Bigtable.V2;
using Grpc.Core;

namespace GoogleCloudSamples.Bigtable
{
    public class QuickStartTests
    {
        readonly CommandLineRunner _quickStart = new CommandLineRunner()
        {
            Main = QuickStart.Main,
            Command = "dotnet run"
        };

        [Fact]
        public void TestRunQuickStart()
        {
            var output = _quickStart.Run();
            Assert.Equal(0, output.ExitCode);
        }
    }

    public class HelloWorldTests
    {
        readonly CommandLineRunner _helloWorld = new CommandLineRunner()
        {
            Main = HelloWorld.Main,
            Command = "dotnet run"
        };

        [Fact]
        public void TestRunHelloWorld()
        {
            var output = _helloWorld.Run();
            Assert.Equal(0, output.ExitCode);
        }
    }

    public class BigtableFixture : IDisposable
    {
        public void Dispose()
        {
            try
            {
                // Delete table created from running the tests.
                CommandLineRunner runner = new CommandLineRunner()
                {
                    Main = TableAdmin.Main,
                    Command = "TableAdmin"
                };
                runner.Run("DeleteTable", TableId);
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound) { }
        }

        public string ProjectId { get; private set; } =
            Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        // Allow environment variables to override the default instance and table names.
        public string InstanceId { get; private set; } =
            Environment.GetEnvironmentVariable("TEST_BIGTABLE_INSTANCE") ?? "my-instance";
        private static readonly string s_randomTableName = "my-table-"
            + TestUtil.RandomName();
        public string TableId =
            Environment.GetEnvironmentVariable("TEST_BIGTABLE_TABLE") ?? s_randomTableName;
        public bool s_initializedTestSetup { get; set; } = false;
    }

    public class BigtableTests : IClassFixture<BigtableFixture>
    {
        readonly BigtableFixture _fixture;

        readonly CommandLineRunner _tableAdminCmd = new CommandLineRunner()
        {
            Main = TableAdmin.Main,
            Command = "TableAdmin"
        };
        readonly CommandLineRunner _instanceAdminCmd = new CommandLineRunner()
        {
            Main = InstanceAdmin.Main,
            Command = "InstanceAdmin"
        };

        public BigtableTests(BigtableFixture fixture)
        {
            _fixture = fixture;
            lock (this)
            {
                if (!_fixture.s_initializedTestSetup)
                {
                    _fixture.s_initializedTestSetup = true;
                    InitializeTestSetup();
                }
            }
        }

        void InitializeTestSetup()
        {
            // Create a table
            _tableAdminCmd.Run("createTable", _fixture.TableId);
        }

        [Fact]
        public void TestListTables()
        {
            string expectedOutput = $"Table ID:                     {_fixture.TableId}";
            ConsoleOutput output = _tableAdminCmd.Run("listTables");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(expectedOutput, output.Stdout);
        }

        [Fact]
        public void TestListInstances()
        {
            ConsoleOutput output = _instanceAdminCmd.Run("listInstances");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(_fixture.InstanceId, output.Stdout);
        }
    }
}
