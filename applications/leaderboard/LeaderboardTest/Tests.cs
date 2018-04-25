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
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Sdk;
using Grpc.Core;

namespace GoogleCloudSamples.Leaderboard
{
    public class LeaderboardFixture : IDisposable
    {
        public void Dispose()
        {
            try
            {
                // Delete database created from running the tests.
                CommandLineRunner runner = new CommandLineRunner()
                {
                    Main = Program.Main,
                    Command = "Leaderboard"
                };
                runner.Run("deleteDatabase",
                    ProjectId, InstanceId, DatabaseId);
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound) { }
        }

        public string ProjectId { get; private set; } =
            Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        // Allow environment variables to override the default instance and database names.
        public string InstanceId { get; private set; } =
            Environment.GetEnvironmentVariable("TEST_SPANNER_INSTANCE") ?? "my-instance";
        private static readonly string s_randomDatabaseName = "my-db-"
            + TestUtil.RandomName();
        public string DatabaseId =
            Environment.GetEnvironmentVariable("TEST_SPANNER_DATABASE") ?? s_randomDatabaseName;
        public bool s_initializedDatabase { get; set; } = false;
    }

    public class LeaderboardTests : IClassFixture<LeaderboardFixture>
    {
        readonly LeaderboardFixture _fixture;
        readonly CommandLineRunner _spannerCmd = new CommandLineRunner()
        {
            Main = Program.Main,
            Command = "Leaderboard"
        };

        public LeaderboardTests(LeaderboardFixture fixture)
        {
            _fixture = fixture;
            lock (this)
            {
                if (!_fixture.s_initializedDatabase)
                {
                    _fixture.s_initializedDatabase = true;
                    InitializeDatabase();
                }
            }
        }

        void InitializeDatabase()
        {
            // If the database has not been initialized, retry.
            _spannerCmd.Run("createSampleDatabase",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
        }

        [Fact]
        void TestLeaderboard()
        {
            // Insert Player records.
            ConsoleOutput insertOutput = _spannerCmd.Run("insertPlayers",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, insertOutput.ExitCode);
            // Insert Scores records.
            ConsoleOutput insertScoresOutput = _spannerCmd.Run("insertScores",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, insertScoresOutput.ExitCode);
            // Query Top Ten Players of all time.
            ConsoleOutput queryOutput = _spannerCmd.Run("queryTopTenAllTime",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, queryOutput.ExitCode);
            Assert.Contains("PlayerId :", queryOutput.Stdout);
            // Confirm output includes valid timestamps.
            string columnText = "Timestamp : ";
            string[] result = queryOutput.Stdout.Split(
                new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string valueToTest = result[0].Substring(result[0].IndexOf(columnText) + columnText.Length);
            DateTime value;
            Assert.True(DateTime.TryParse(valueToTest, out value));
            // Test that other Top Ten queries run successfully.
            ConsoleOutput queryTopTenOfYearOutput = _spannerCmd.Run("queryTopTenOfYear",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, queryTopTenOfYearOutput.ExitCode);
            ConsoleOutput queryTopTenOfMonthOutput = _spannerCmd.Run("queryTopTenOfMonth",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, queryTopTenOfMonthOutput.ExitCode);
            ConsoleOutput queryTopTenOfWeekOutput = _spannerCmd.Run("queryTopTenOfWeek",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, queryTopTenOfWeekOutput.ExitCode);
        }
    }
}
