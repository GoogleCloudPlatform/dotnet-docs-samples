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

using Google.Api.Gax.Grpc;
using Google.Cloud.Datastore.V1;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sudokumb
{
    // Represents the status of one puzzle as its being solved.
    public class SolveState
    {
        /// <summary>
        ///  Null means the puzzle hasn't been completely solved.
        /// </summary>
        public GameBoard Solution { get; set; }

        /// <summary>
        /// How many game boards have been examined while searching for the
        /// solution?
        /// </summary>
        public long BoardsExaminedCount { get; set; }
    }

    public class SolveStateStore
    {
        private const string SOLUTION_KIND = "Solution";
        private readonly DatastoreDb _datastore;
        private readonly KeyFactory _solutionKeyFactory;
        private readonly DatastoreCounter _datastoreCounter;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private readonly ICounter _locallyExaminedBoardCount = new InterlockedCounter();

        public long LocallyExaminedBoardCount
        {
            get => _locallyExaminedBoardCount.Count;
            private set { }
        }

        public SolveStateStore(DatastoreDb datastore,
            DatastoreCounter datastoreCounter, IMemoryCache cache,
            ILogger<SolveStateStore> logger)
        {
            _datastore = datastore;
            _datastoreCounter = datastoreCounter;
            _cache = cache;
            _logger = logger;
            _solutionKeyFactory = new KeyFactory(datastore.ProjectId,
                datastore.NamespaceId, SOLUTION_KIND);
        }

        public async Task<SolveState> GetAsync(string solveRequestId,
            CancellationToken cancellationToken)
        {
            var lookupTask = _datastore.LookupAsync(
                _solutionKeyFactory.CreateKey(solveRequestId));
            var solveState = new SolveState()
            {
                BoardsExaminedCount = await _datastoreCounter
                    .GetCountAsync(solveRequestId, cancellationToken)
            };
            Entity entity = await lookupTask;
            if (null != entity && entity.Properties.ContainsKey(SOLUTION_KIND))
            {
                solveState.Solution = GameBoard.Create(
                    (string)entity[SOLUTION_KIND]);
            }
            return solveState;
        }

        public Task<SolveState> GetCachedAsync(string solveRequestId,
            CancellationToken cancellationToken)
        {
            return _cache.GetOrCreate<Task<SolveState>>(solveRequestId,
            entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(5);
                return GetAsync(solveRequestId, cancellationToken);
            });
        }

        public Task SetAsync(string solveRequestId, GameBoard gameBoard,
            CancellationToken cancellationToken)
        {
            Entity entity = new Entity()
            {
                Key = _solutionKeyFactory.CreateKey(solveRequestId),
                [SOLUTION_KIND] = gameBoard.Board
            };
            entity[SOLUTION_KIND].ExcludeFromIndexes = true;
            return _datastore.UpsertAsync(entity,
                CallSettings.FromCancellationToken(cancellationToken));
        }

        public void IncreaseExaminedBoardCount(string solveRequestId,
            long amount)
        {
            _locallyExaminedBoardCount.Increase(amount);
            _datastoreCounter.GetLocalCounter(solveRequestId).Increase(amount);
        }
    }
}