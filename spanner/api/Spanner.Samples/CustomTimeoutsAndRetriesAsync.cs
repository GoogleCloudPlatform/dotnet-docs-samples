// Copyright 2020 Google Inc.
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

// [START spanner_set_custom_timeout_and_retry]

using Google.Api.Gax;
using Google.Api.Gax.Grpc;
using Google.Cloud.Spanner.Common.V1;
using Google.Cloud.Spanner.V1;
using Grpc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Google.Cloud.Spanner.V1.TransactionOptions.Types;

public class CustomTimeoutsAndRetriesAsyncSample
{
    public async Task<long> CustomTimeoutsAndRetriesAsync(string projectId, string instanceId, string databaseId)
    {
        // Create a SessionPool.
        SpannerClient client = SpannerClient.Create();
        SessionPool sessionPool = new SessionPool(client, new SessionPoolOptions());

        // Acquire a session with a read-write transaction to run a query.
        DatabaseName databaseName =
            DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId);
        TransactionOptions transactionOptions = new TransactionOptions
        {
            ReadWrite = new ReadWrite()
        };
        using PooledSession session = await sessionPool.AcquireSessionAsync(
            databaseName, transactionOptions, CancellationToken.None);

        ExecuteSqlRequest request = new ExecuteSqlRequest
        {
            Sql = "INSERT Singers (SingerId, FirstName, LastName) VALUES (20, 'George', 'Washington')"
        };

        // Prepare the call settings with custom timeout and retry settings.
        CallSettings settings = CallSettings
            .FromExpiration(Expiration.FromTimeout(TimeSpan.FromSeconds(60)))
            .WithRetry(RetrySettings.FromExponentialBackoff(
                maxAttempts: 12,
                initialBackoff: TimeSpan.FromMilliseconds(500),
                maxBackoff: TimeSpan.FromSeconds(16),
                backoffMultiplier: 1.5,
                retryFilter: RetrySettings.FilterForStatusCodes(
                    new StatusCode[] { StatusCode.Unavailable })));

        ResultSet result = await session.ExecuteSqlAsync(request, settings);
        await session.CommitAsync(new CommitRequest(), null);

        return result.Stats.RowCountExact;
    }
}
// [END spanner_set_custom_timeout_and_retry]
