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
    class Program
    {
        public const string SamplePuzzle = @". . .  . 3 2  . . 7
9 2 7  . . .  . . .
. . 5  . . .  2 6 .

. . .  . 2 6  . . .
. 8 9  . 5 .  3 4 .
. . .  8 9 .  . . .

. 9 3  . . .  8 . .
. . .  . . .  1 5 6
5 . .  7 8 .  . . .
";

        static void Main(string[] args)
        {
            GameBoard board = GameBoard.ParseHandInput(SamplePuzzle);
            CancellationTokenSource cancel = new CancellationTokenSource();
            FakeSolveStateStore fakeStore = new FakeSolveStateStore();
            InMemoryGameBoardStack stack = new InMemoryGameBoardStack(
                new Solver(), fakeStore);
            const int seconds = 5;
            Console.WriteLine($"Solving puzzles for {seconds} seconds...");
            Task loop = Task.Run(() => Loop(new[] { board }, stack, cancel.Token));
            Thread.Sleep(seconds * 1000);
            cancel.Cancel();
            try
            {
                loop.Wait();
            }
            catch (AggregateException e)
            when (e.InnerException is TaskCanceledException)
            {
                // We canceled the task, so this is the expected result.
            }
            Console.WriteLine($"Examined {fakeStore.Count} boards.");
            Console.WriteLine($"At a rate of {fakeStore.Count / seconds} boards per second.");
        }

        static async Task Loop(IEnumerable<GameBoard> seedBoards,
            InMemoryGameBoardStack stack,
            CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await stack.Publish("1", seedBoards, token);
            }
        }

        class FakeSolveStateStore : ISolveStateStore
        {
            private long _count = 0;
            public long Count
            {
                get { return _count; }
            }

            public void IncreaseExaminedBoardCount(string solveRequestId, long amount)
            {
                _count += amount;
            }

            public Task SetAsync(string solveRequestId, GameBoard gameBoard,
                CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}
