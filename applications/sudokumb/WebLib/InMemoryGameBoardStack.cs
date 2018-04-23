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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sudokumb
{
    public class InMemoryGameBoardStackImpl
    {
        private readonly Solver _solver;
        private readonly SolveStateStore _solveStateStore;

        public InMemoryGameBoardStackImpl(Solver solver,
            SolveStateStore solveStateStore)
        {
            _solver = solver;
            _solveStateStore = solveStateStore;
        }

        public async Task<bool> Publish(string solveRequestId,
            IEnumerable<GameBoard> gameBoards,
            CancellationToken cancellationToken)
        {
            Stack<GameBoard> stack = new Stack<GameBoard>(gameBoards);
            while (stack.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _solveStateStore.IncreaseExaminedBoardCount(solveRequestId, 1);
                GameBoard board = stack.Pop();
                IEnumerable<GameBoard> nextMoves;
                if (_solver.ExamineGameBoard(board, out nextMoves))
                {
                    await _solveStateStore.SetAsync(solveRequestId, board,
                        cancellationToken);
                    return true;
                }
                foreach (GameBoard gameBoard in nextMoves)
                {
                    stack.Push(gameBoard);
                }
            }
            return false;
        }
    }

    public class InMemoryGameBoardStack : InMemoryGameBoardStackImpl, IGameBoardQueue
    {
        public InMemoryGameBoardStack(Solver solver,
            SolveStateStore solveStateStore)
            : base(solver, solveStateStore)
        {
        }
    }
}