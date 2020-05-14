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

using Google.Api.Gax.Grpc;
using Google.Cloud.PubSub.V1;
using GoogleCloudSamples;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Xunit;

[CollectionDefinition(nameof(PubsubFixture))]
public class PubsubFixture : IDisposable, ICollectionFixture<PubsubFixture>
{
    public readonly string ProjectId;
    public readonly PublisherServiceApiClient Publisher;
    public readonly SubscriberServiceApiClient Subscriber;
    public readonly List<string> TempTopicIds = new List<string>();
    public readonly List<string> TempSubscriptionIds = new List<string>();

    public PubsubFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        Publisher = PublisherServiceApiClient.Create();
        Subscriber = SubscriberServiceApiClient.Create();
    }

    public void Dispose()
    {
        var deleteTopicSampleObject = new DeleteTopicSample();
        var deleteSubscriptionSampleObject = new DeleteSubscriptionSample();
        foreach (string subscriptionId in TempSubscriptionIds)
        {
            Eventually(HandleDeleteRace(() =>
                deleteSubscriptionSampleObject.DeleteSubscription(ProjectId, subscriptionId)));
        }
        foreach (string topicId in TempTopicIds)
        {
            Eventually(HandleDeleteRace(() =>
                deleteTopicSampleObject.DeleteTopic(ProjectId, topicId)));
        }
    }

    /// <summary>
    /// Returns true if an action should be retried when an exception is
    /// thrown.
    /// </summary>
    static bool ShouldRetry(Exception e)
    {
        AggregateException aggregateException = e as AggregateException;
        if (aggregateException != null)
        {
            return ShouldRetry(aggregateException.InnerExceptions
                .LastOrDefault());
        }
        if (e is Xunit.Sdk.XunitException)
        {
            return true;
        }
        var rpcException = e as Grpc.Core.RpcException;
        return rpcException?.Status.StatusCode == StatusCode.NotFound;
    }

    private readonly RetryRobot _retryRobot = new RetryRobot()
    {
        ShouldRetry = (Exception e) => ShouldRetry(e)
    };

    public void Eventually(Action action) => _retryRobot.Eventually(action);

    /// <summary>
    /// Handle a special race condition that can occur when deleting
    /// something:
    /// 1. Delete request times out.
    /// 2. Delete operation continues on server and succeeds.
    /// 3. Later requests to delete the same entity see NotFound error.
    /// </summary>
    /// <param name="delete">The delete operation to run.</param>
    /// <returns>An action to run inside Eventually().</returns>
    Action HandleDeleteRace(Action delete)
    {
        bool sawTimeout = false;
        return () =>
        {
            if (!sawTimeout)
            {
                try
                {
                    delete();
                }
                catch (Grpc.Core.RpcException e)
                when (e.Status.StatusCode == StatusCode.DeadlineExceeded)
                {
                    sawTimeout = true;
                    throw;
                }
            }
            else
            {
                try
                {
                    delete();
                }
                catch (Grpc.Core.RpcException e)
                when (e.Status.StatusCode == StatusCode.NotFound)
                {
                    // Earlier timeout request that deleted the thing
                    // actually succeeded on the server.
                }
            }
        };
    }

    /// <summary>
    /// Creates new CallSettings that will retry an RPC that fails.
    /// </summary>
    /// <param name="tryCount">
    /// How many times to try the RPC before giving up?
    /// </param>
    /// <param name="finalStatusCodes">
    /// Which status codes should we *not* retry?
    /// </param>
    /// <returns>
    /// A CallSettings instance.
    /// </returns>
    public CallSettings NewRetryCallSettings(int tryCount, params StatusCode[] finalStatusCodes) =>
        // Initialize values for backoff settings to be used
        // by the CallSettings for RPC retries
        CallSettings.FromRetry(
            RetrySettings.FromExponentialBackoff(
                maxAttempts: tryCount,
                initialBackoff: TimeSpan.FromSeconds(3),
                maxBackoff: TimeSpan.FromSeconds(10),
                backoffMultiplier: 2,
                retryFilter: ex =>
                    ex is RpcException rpcEx &&
                    rpcEx.StatusCode != StatusCode.OK &&
                    !finalStatusCodes.Contains(rpcEx.StatusCode),
                backoffJitter: RetrySettings.NoJitter))
            .WithHeader("ClientVersion", "2.0.0-beta02");

    public Topic CreateTopic(string topicId)
    {
        var createTopicSampleObject = new CreateTopicSample();
        var topic = createTopicSampleObject.CreateTopic(ProjectId, topicId);
        TempTopicIds.Add(topicId);
        return topic;
    }

    public Subscription CreateSubscription(string topicId, string subscriptionId)
    {
        var createSubscriptionSampleObject = new CreateSubscriptionSample();
        var subscription = createSubscriptionSampleObject.CreateSubscription(ProjectId, topicId, subscriptionId);
        TempSubscriptionIds.Add(subscriptionId);
        return subscription;
    }

    public string RandomName()
    {
        using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
        {
            string legalChars = "abcdefhijklmnpqrstuvwxyz";
            byte[] randomByte = new byte[1];
            var randomChars = new char[20];
            int nextChar = 0;
            while (nextChar < randomChars.Length)
            {
                rng.GetBytes(randomByte);
                if (legalChars.Contains((char)randomByte[0]))
                    randomChars[nextChar++] = (char)randomByte[0];
            }
            return new string(randomChars);
        }
    }
}
