// Copyright 2021 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START spanner_get_commit_stats]

using Google.Cloud.Spanner.Data;
using Google.Cloud.Spanner.V1;
using Google.Cloud.Spanner.V1.Internal.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public class LogCommitStatsAsyncSample
{
    public async Task<long> LogCommitStatsAsync(string projectId, string instanceId, string databaseId)
    {
        // Commit statistics are logged at level Info by the default logger.
        // This sample uses a custom logger to access the commit statistics.
        // See https://googleapis.github.io/google-cloud-dotnet/docs/Google.Cloud.Spanner.Data/logging.html
        // for more information on how to use loggers.
        var logger = new CommitStatsSampleLogger();
        var options = new SessionPoolOptions();
        var poolManager = SessionPoolManager.Create(options, logger);
        var connectionStringBuilder = new SpannerConnectionStringBuilder
        {
            ConnectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}",
            // Set LogCommitStats to true to enable logging commit statistics for all transactions on the connection.
            // LogCommitStats can also be enabled/disabled for individual Spanner transactions.
            LogCommitStats = true,
            SessionPoolManager = poolManager,
        };

        using var connection = new SpannerConnection(connectionStringBuilder);
        await connection.OpenAsync();

        using var cmd = connection.CreateDmlCommand("INSERT Singers (SingerId, FirstName, LastName) VALUES (110, 'Virginia', 'Watson')");
        var rowCount = await cmd.ExecuteNonQueryAsync();
        var mutationCount = logger._lastCommitResponse.CommitStats.MutationCount;

        Console.WriteLine($"{rowCount} row(s) inserted...");
        Console.WriteLine($"{mutationCount} mutation(s) in transaction...");

        return mutationCount;
    }

    /// <summary>
    /// Sample logger that keeps a reference to the last seen commit response.
    /// Use the default logger if you only want to log the commit stats.
    /// </summary>
    public class CommitStatsSampleLogger : Logger
    {
        internal CommitResponse _lastCommitResponse;

        /// <summary>
        /// This method is called when a transaction that requested commit stats is committed.
        /// </summary>
        public override void LogCommitStats(CommitRequest request, CommitResponse response)
        {
            _lastCommitResponse = response;
            base.LogCommitStats(request, response);
        }

        protected override void LogImpl(LogLevel level, string message, Exception exception) =>
            WriteLine(exception == null ? $"{level}: {message}" : $"{level}: {message}, Exception: {exception}");

        protected override void LogPerformanceEntries(IEnumerable<string> entries)
        {
            string separator = Environment.NewLine + "  ";
            WriteLine($"Performance:{separator}{string.Join(separator, entries)}");
        }

        private void WriteLine(string line) => Trace.TraceInformation(line);
    }
}
// [END spanner_get_commit_stats]
