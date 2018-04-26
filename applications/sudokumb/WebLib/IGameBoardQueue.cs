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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sudokumb
{
    public interface IGameBoardQueue
    {
        // Returns true if the puzzle was solved immediately.
        Task<bool> Publish(string solveRequestId,
            IEnumerable<GameBoard> gameBoards,
            CancellationToken cancellationToken);
    }

    public static class IGameBoardQueueExtensions
    {
        public static async Task<string> StartSolving(
            this IGameBoardQueue queue, GameBoard gameBoard,
            CancellationToken cancellationToken)
        {
            // Create a new request and publish it to pubsub.
            string solveRequestId = Guid.NewGuid().ToString();
            await queue.Publish(solveRequestId, new[] { gameBoard },
                cancellationToken);
            return solveRequestId;
        }
    }
}