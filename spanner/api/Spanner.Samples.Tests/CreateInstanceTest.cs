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

using System;
using System.Linq;
using Grpc.Core;
using System.Threading;
using Google.Rpc;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class CreateInstanceTest
{
    private const int MaxAttempts = 10;
    private static readonly string s_retryInfoMetadataKey = RetryInfo.Descriptor.FullName + "-bin";
    private readonly SpannerFixture _spannerFixture;

    public CreateInstanceTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public void TestCreateInstance()
    {
        CreateInstanceSample createInstanceSample = new CreateInstanceSample();
        RpcException exception;
        int attempt = 0;
        while (true)
        {
            // Instance already exists since it was created in the test setup so it should throw an exception.
            exception = Assert.Throws<RpcException>(() => createInstanceSample.CreateInstance(_spannerFixture.ProjectId, _spannerFixture.InstanceId));
            attempt++;
            // Retry the test if we get a temporary Unavailable error.
            if (StatusCode.Unavailable == exception.StatusCode && attempt < MaxAttempts)
            {
                var delay = ExtractRetryDelay(exception);
                Thread.Sleep(delay.Milliseconds);
                continue;
            }
            break;
        }
        Assert.Equal(StatusCode.AlreadyExists, exception.StatusCode);
    }

    private TimeSpan ExtractRetryDelay(RpcException exception)
    {
        var retryInfoEntry = exception.Trailers.FirstOrDefault(
            entry => s_retryInfoMetadataKey.Equals(entry.Key, StringComparison.InvariantCultureIgnoreCase));
        if (retryInfoEntry != null)
        {
            var retryInfo = RetryInfo.Parser.ParseFrom(retryInfoEntry.ValueBytes);
            var recommended = retryInfo.RetryDelay.ToTimeSpan();
            if (recommended != TimeSpan.Zero)
            {
                return recommended;
            }
        }
        return TimeSpan.FromSeconds(5);
    }
}
