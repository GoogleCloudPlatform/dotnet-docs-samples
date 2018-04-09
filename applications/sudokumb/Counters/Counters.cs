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
    public interface ICounter
    {
        void Increase(long amount);
        long Count { get; }
    }

    public class UnsynchronizedCounter : ICounter
    {
        long _count = 0;
        public long Count => _count;
        public void Increase(long amount)
        {
            _count += amount;
        }
    }

    public class LockingCounter : ICounter
    {
        long _count = 0;
        readonly object _thisLock = new object();

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

    public class InterlockedCounter : ICounter
    {
        long _count = 0;

        public long Count => Interlocked.CompareExchange(ref _count, 0, 0);

        public void Increase(long amount)
        {
            Interlocked.Add(ref _count, amount);
        }
    }

    public class ShardedCounter : ICounter
    {
        readonly object _thisLock = new object();
        long _deadShardSum = 0;
        List<Shard> _shards = new List<Shard>();
        readonly LocalDataStoreSlot _slot = Thread.AllocateDataSlot();

        public long Count
        {
            get
            {
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

        class Shard : InterlockedCounter
        {
            public Thread Owner { get; set; }
        }
    }
}
