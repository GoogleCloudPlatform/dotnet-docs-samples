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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Api.Gax.Grpc;
using Google.Cloud.Datastore.V1;
using Google.Cloud.Diagnostics.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sudokumb;

namespace Sudokumb
{
    public class DatastoreCounterOptions
    {
        public string Kind { get; set; } = "Counter";
        public TimeSpan CondenseFrequency { get; set; } = TimeSpan.FromDays(1);
    }

    // Rolls over the DatastoreCounter at half the CondenseFrequency.
    // Prevents race condition when a counter key gets cleaned up during
    // condensation, and later counter gets written again.
    internal class DatastoreCounterSingleton : IHostedService
    {
        readonly DatastoreDb _datastore;
        readonly IOptions<DatastoreCounterOptions> _options;
        readonly ILogger<DatastoreCounter> _logger;
        readonly IManagedTracer _tracer;
        readonly object _thisLock = new object();
        DatastoreCounter _counter;
        DatastoreCounter _oldCounter;

        DateTime _counterBirthday;

        public DatastoreCounterSingleton(DatastoreDb datastore,
            IOptions<DatastoreCounterOptions> options,
            ILogger<DatastoreCounter> logger, IManagedTracer tracer)
        {
            _datastore = datastore;
            _options = options;
            _logger = logger;
            _tracer = tracer;
            _counter = new DatastoreCounter(datastore, options, logger, tracer);
            _counterBirthday = DateTime.UtcNow;
        }

        public DatastoreCounter DaCounter
        {
            get
            {
                DateTime now = DateTime.UtcNow;
                lock (_thisLock)
                {
                    if (now - _counterBirthday >
                        (_options.Value.CondenseFrequency / 2))
                    {
                        // Time to roll over the counter.
                        if (_oldCounter != null)
                        {
                            _oldCounter.StopAsync(CancellationToken.None);
                        }
                        _counterBirthday = now;
                        _oldCounter = _counter;
                        _counter = new DatastoreCounter(
                            _datastore, _options, _logger, _tracer);
                        _counter.StartAsync(CancellationToken.None);
                    }
                    return _counter;
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken) =>
            DaCounter.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) =>
            DaCounter.StartAsync(cancellationToken);
    }

    public static class DatastoreCounterExtensions
    {
        public static IServiceCollection AddDatastoreCounter(
            this IServiceCollection services)
        {
            services.AddSingleton<DatastoreCounterSingleton, DatastoreCounterSingleton>();
            services.AddSingleton<IHostedService>(
                (p) => p.GetService<DatastoreCounterSingleton>());
            services.AddSingleton<DatastoreCounter>(
                (p) => p.GetService<DatastoreCounterSingleton>().DaCounter);
            return services;
        }
    }

    public class DatastoreCounter : IHostedService
    {
        const string COUNT = "count", TIMESTAMP = "timestamp";
        readonly DatastoreDb _datastore;
        readonly IOptions<DatastoreCounterOptions> _options;
        readonly KeyFactory _keyFactory;
        readonly string _shard = Guid.NewGuid().ToString();
        CancellationTokenSource _cancelHostedService;
        Task _hostedService;
        readonly ILogger _logger;
        readonly IManagedTracer _tracer;
        readonly ConcurrentDictionary<string, ICounter> _localCounters
                     = new ConcurrentDictionary<string, ICounter>();

        internal DatastoreCounter(DatastoreDb datastore,
            IOptions<DatastoreCounterOptions> options,
            ILogger<DatastoreCounter> logger,
            IManagedTracer tracer)
        {
            _datastore = datastore;
            _options = options;
            _logger = logger;
            _tracer = tracer;
            var opts = options.Value;
            _keyFactory = new KeyFactory(datastore.ProjectId,
                datastore.NamespaceId, opts.Kind);
        }

        public async Task<long> GetCountAsync(string key,
            CancellationToken cancellationToken)
        {
            using (_tracer.StartSpan(nameof(GetCountAsync)))
            {
                var callSettings = CallSettings.FromCancellationToken(
                    cancellationToken);
                var query = new Query(_options.Value.Kind)
                {
                    Filter = Filter.GreaterThan("__key__", _keyFactory.CreateKey(key)),
                    Order = { { "__key__", PropertyOrder.Types.Direction.Ascending } }
                };
                long count = 0;
                var lazyResults = _datastore.RunQueryLazilyAsync(query,
                    callSettings: callSettings).GetEnumerator();
                while (await lazyResults.MoveNext())
                {
                    Entity entity = lazyResults.Current;
                    if (!entity.Key.Path.First().Name.StartsWith(key))
                    {
                        break;
                    }
                    count += (long)entity[COUNT];
                }
                return count;
            }
        }

        string GetCounterId(string keyName)
        {
            int colon = keyName.LastIndexOf(':');
            string counterId = keyName.Substring(0, colon);
            return counterId;
        }

        public async Task CondenseOldCounters(CancellationToken cancellationToken)
        {
            var callSettings = CallSettings.FromCancellationToken(
                cancellationToken);
            List<string> oldKeys = new List<string>();
            DateTime tooOld = DateTime.UtcNow - _options.Value.CondenseFrequency;
            var query = new Query(_options.Value.Kind)
            {
                Filter = Filter.LessThan(TIMESTAMP, tooOld),
                Projection = { "__key__" }
            };
            var lazyResults = _datastore.RunQueryLazilyAsync(query,
                callSettings: callSettings).GetEnumerator();
            while (await lazyResults.MoveNext())
            {
                oldKeys.Add(lazyResults.Current.Key.Path.Last().Name);
            }
            oldKeys.Sort();
            // Find groups of old keys with matching counter ids.
            int firstInGroup = 0;
            for (int i = 1; i < oldKeys.Count; ++i)
            {
                int groupSize = i - firstInGroup;
                if (GetCounterId(oldKeys[i]) == GetCounterId(oldKeys[firstInGroup])
                    && groupSize < 10)
                {
                    // Same group.  Continue search for next group boundary.
                }
                else if (groupSize == 1)
                {
                    // Group of size one, nothing to do.
                    firstInGroup = i;
                }
                else
                {
                    await CondenseOldCounters(
                        oldKeys.GetRange(firstInGroup, groupSize),
                        callSettings);
                    firstInGroup = i;
                }
            }
        }

        async Task CondenseOldCounters(IEnumerable<string> keyNames,
            CallSettings callSettings)
        {
            var keys = keyNames.Select((keyName) => _keyFactory.CreateKey(keyName));
            long sum = 0;
            using (var transaction = await _datastore.BeginTransactionAsync())
            {
                var entities = (await transaction.LookupAsync(keys, callSettings))
                    .Where(e => e != null);
                if (entities.Count() < 2)
                {
                    return;  // Nothing to condense.
                }
                foreach (Entity entity in entities)
                {
                    sum += (long)entity[COUNT];
                }
                Entity first = entities.First();
                first[COUNT] = sum;
                first[COUNT].ExcludeFromIndexes = true;
                first[TIMESTAMP] = DateTime.UtcNow;
                transaction.Update(first);
                transaction.Delete(entities.Skip(1));
                await transaction.CommitAsync();
            }
        }

        // /////////////////////////////////////////////////////////////////////
        // IHostedService implementation periodically saves
        // counts to datastore.
        public Task StartAsync(CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.Assert(null == _cancelHostedService);
            _cancelHostedService = new CancellationTokenSource();
            _hostedService = Task.Run(async () => await HostedServiceMainAsync(
                _cancelHostedService.Token));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancelHostedService.Cancel();
            return _hostedService;
        }

        public ICounter GetLocalCounter(string id) =>
            _localCounters.GetOrAdd(id,
                (key) => (ICounter)new InterlockedCounter());

        async Task UpdateDatastoreFromLocalCountersAsync(
            Dictionary<string, long> localCountersSnapshot,
            CancellationToken cancellationToken)
        {
            List<Entity> entities = new List<Entity>();
            var now = DateTime.UtcNow;
            foreach (var keyValue in _localCounters)
            {
                long count = keyValue.Value.Count;
                if (count != localCountersSnapshot
                    .GetValueOrDefault(keyValue.Key))
                {
                    localCountersSnapshot[keyValue.Key] = count;
                    var entity = new Entity()
                    {
                        Key = _keyFactory.CreateKey($"{keyValue.Key}:{_shard}"),
                        [COUNT] = count,
                        [TIMESTAMP] = now
                    };
                    entity[COUNT].ExcludeFromIndexes = true;
                    entities.Add(entity);
                }
            }
            if (entities.Count > 0)
            {
                await _datastore.UpsertAsync(entities, CallSettings
                    .FromCancellationToken(cancellationToken));
            }
        }

        public async Task HostedServiceMainAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("DatastoreCounter.HostedServiceMainAsync()");
            var rand = new Random();
            Dictionary<string, long> localCountersSnapshot
                = new Dictionary<string, long>();
            DateTime nextCondense = DateTime.UtcNow.AddSeconds(rand.Next((int)
                _options.Value.CondenseFrequency.TotalSeconds * 2));
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                try
                {
                    await Task.Delay(1000, cancellationToken);
                    await UpdateDatastoreFromLocalCountersAsync(
                        localCountersSnapshot, cancellationToken);
                    DateTime now = DateTime.UtcNow;
                    if (now > nextCondense)
                    {
                        await CondenseOldCounters(cancellationToken);
                        nextCondense = DateTime.UtcNow.AddSeconds(rand.Next((int)
                            _options.Value.CondenseFrequency.TotalSeconds * 2));
                    }
                }
                catch (Exception e)
                when (!(e is OperationCanceledException))
                {
                    _logger.LogError(1, e, "Error while updating datastore.");
                }
            }
        }
    }
}


