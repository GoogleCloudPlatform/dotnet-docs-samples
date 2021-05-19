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

// [START spanner_retry_strategy]

using System;
using System.Threading;

public class RetryRobot
{
    public TimeSpan FirstRetryDelay { get; set; } = TimeSpan.FromSeconds(1000);
    public float DelayMultiplier { get; set; } = 2;
    public int MaxTryCount { get; set; } = 7;
    public Func<Exception, bool> ShouldRetry { get; set; }

    /// <summary>
    /// Retry action when assertion fails.
    /// </summary>
    /// <param name="func"></param>
    public T Eventually<T>(Func<T> func)
    {
        TimeSpan delay = FirstRetryDelay;
        for (int i = 0; ; ++i)
        {
            try
            {
                return func();
            }
            catch (Exception e) when (ShouldCatch(e) && i < MaxTryCount)
            {
                Thread.Sleep(delay);
                delay *= (int)DelayMultiplier;
            }
        }
    }

    private bool ShouldCatch(Exception e) => ShouldRetry?.Invoke(e) ?? false;
}
// [END spanner_retry_strategy]
