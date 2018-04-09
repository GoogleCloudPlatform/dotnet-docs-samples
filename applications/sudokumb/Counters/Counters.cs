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
        long Count {get; }
    }

    public class UnsynchronizedCounter : ICounter
    {
        long count_ = 0;
        public long Count => count_;
        public void Increase(long amount)
        {
            count_ += amount;
        }
    }

    public class LockingCounter : ICounter
    {
        long count_ = 0;
        object thisLock = new object();

        public long Count
        {
            get
            {
                lock(thisLock)
                {
                    return count_;
                }
            }
        }

        public void Increase(long amount)
        {
            lock(thisLock)
            {
                count_ += amount;
            }
        }
    }

    public class InterlockedCounter : ICounter
    {
        long count_ = 0;

        public long Count => Interlocked.CompareExchange(ref count_, 0, 0);

        public void Increase(long amount)
        {
            Interlocked.Add(ref count_, amount);
        }
    }

    public class ShardedCounter : ICounter
    {
        object thisLock_ = new object();
        long deadShardSum_ = 0;
        List<Shard> shards_ = new List<Shard>();
        readonly LocalDataStoreSlot slot_ = Thread.AllocateDataSlot();

        public long Count
        {
            get
            {
                long sum = deadShardSum_;
                List<Shard> livingShards_ = new List<Shard>();
                lock (thisLock_)
                {
                    foreach (Shard shard in shards_)
                    {
                        sum += shard.Count;
                        if (shard.Owner.IsAlive)
                        {
                            livingShards_.Add(shard);
                        }
                        else
                        {
                            deadShardSum_ += shard.Count;
                        }
                    }
                    shards_ = livingShards_;
                }
                return sum;
            }
        }

        public void Increase(long amount)
        {
            Shard counter = Thread.GetData(slot_) as Shard;
            if (null == counter)
            {
                counter = new Shard()
                {
                    Owner = Thread.CurrentThread
                };
                Thread.SetData(slot_, counter);
                lock (thisLock_) shards_.Add(counter);
            }
            counter.Increase(amount);
        }

        class Shard : InterlockedCounter
        {
            public Thread Owner { get; set; }
        }
    }
}
