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

using System;
using Xunit;

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
        readonly string _instanceId = "my-instance";
        readonly string _databseId = "my-database";

        readonly CommandLineRunner _spanner = new CommandLineRunner()
        {
            VoidMain = Program.Main,
            Command = "Spanner"
        };

        [Fact]
        void TestQuery()
        {
            ConsoleOutput output = _spanner.Run("querySampleData",
                s_projectId, _instanceId, _databseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
        }

        [Fact]
        void TestQueryTransaction()
        {
            ConsoleOutput output = _spanner.Run("queryDataWithTransaction",
                s_projectId, _instanceId, _databseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
        }

        [Fact]
        void TestQueryTransactionCore()
        {
            ConsoleOutput output = _spanner.Run("queryDataWithTransaction",
                s_projectId, _instanceId, _databseId, "netcore");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
        }

        [Fact]
        void TestReadWriteTransaction()
        {
            ConsoleOutput output = _spanner.Run("readWriteWithTransaction",
                s_projectId, _instanceId, _databseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Transaction complete.", output.Stdout);
        }

        [Fact]
        void TestReadWriteTransactionCore()
        {
            ConsoleOutput output = _spanner.Run("readWriteWithTransaction",
                s_projectId, _instanceId, _databseId, "netcore");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Transaction complete.", output.Stdout);
        }

        [Fact]
        void TestSpannerNoArgsSucceeds()
        {
            ConsoleOutput output = _spanner.Run();
            Assert.Equal(0, output.ExitCode);
        }
    }
}
