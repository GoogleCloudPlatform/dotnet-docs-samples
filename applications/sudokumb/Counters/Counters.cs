// Copyright (c) 2018 Google LLC.
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Sudokumb
{
    /// <summary>
    /// A simple counter interface.
    /// </summary>
    public interface ICounter
    {
        /// <summary>
        /// Increase the count be the amount.
        /// </summary>
        /// <param name="amount">The amount to increase the counter.</param>
        void Increase(long amount);
        /// <summary>
        /// Get the current count.
        /// </summary>
        /// <returns>The current count.</returns>
        long Count { get; }
    }

    /// <summary>
    /// Implement the ICounter interface without thread synchronization.
    /// Do not call its methods from multiple threads.
    /// </summary>
    public class UnsynchronizedCounter : ICounter
    {
        private long _count = 0;
        public long Count => _count;
        public void Increase(long amount)
        {
            _count += amount;
        }
    }

    /// <summary>
    /// Implement the ICounter interface using regular C# locks.
    /// </summary>
    public class LockingCounter : ICounter
    {
        private long _count = 0;
        private readonly object _thisLock = new object();

        public long Count
        {
            get
            {
                lock (_thisLock)
                {
                    return _count;
                }
            }
        }

        public void Increase(long amount)
        {
            lock (_thisLock)
            {
                _count += amount;
            }
        }
    }

    /// <summary>
    /// Implement the ICounterInterface using System.Threading.Interlocked
    /// functions.
    /// </summary>
    public class InterlockedCounter : ICounter
    {
        private long _count = 0;

        public long Count => Interlocked.CompareExchange(ref _count, 0, 0);

        public void Increase(long amount)
        {
            Interlocked.Add(ref _count, amount);
        }
    }

    /// <summary>
    /// Implement the ICounterInterface by creating thread-local counters
    /// fore each thread.
    /// </summary>
    public class ShardedCounter : ICounter
    {
        // Protects _deadShardSum and _shards.
        private readonly object _thisLock = new object();
        // The total sum from the shards from the threads which have terminated.
        private long _deadShardSum = 0;
        // The list of shards.
        private List<Shard> _shards = new List<Shard>();
        // The thread-local slot where shards are stored.
        private readonly LocalDataStoreSlot _slot = Thread.AllocateDataSlot();

        public long Count
        {
            get
            {
                // Sum over all the shards, and clean up dead shards at the
                // same time.
                long sum = _deadShardSum;
                List<Shard> livingShards_ = new List<Shard>();
                lock (_thisLock)
                {
                    foreach (Shard shard in _shards)
                    {
                        sum += shard.Count;
                        if (shard.Owner.IsAlive)
                        {
                            livingShards_.Add(shard);
                        }
                        else
                        {
                            _deadShardSum += shard.Count;
                        }
                    }
                    _shards = livingShards_;
                }
                return sum;
            }
        }

        public void Increase(long amount)
        {
            // Increase counter for this thread.
            Shard counter = Thread.GetData(_slot) as Shard;
            if (null == counter)
            {
                counter = new Shard()
                {
                    Owner = Thread.CurrentThread
                };
                Thread.SetData(_slot, counter);
                lock (_thisLock) _shards.Add(counter);
            }
            counter.Increase(amount);
        }

        private class Shard : InterlockedCounter
        {
            public Thread Owner { get; set; }
        }
    }
}
