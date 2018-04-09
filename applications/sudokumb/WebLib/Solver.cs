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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Datastore.V1;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sudokumb;
using Newtonsoft.Json;
using Google.Protobuf;
using Microsoft.Extensions.Hosting;

namespace Sudokumb
{
    /// <summary>
    /// Reads sudoku puzzles from Pub/Sub and solves them.
    /// </summary>
    public class Solver
    {
        public bool ExamineGameBoard(GameBoard board,
            out IEnumerable<GameBoard> nextMoves)
        {
            if (!board.HasEmptyCell())
            {
                nextMoves = null;
                return true;
            }
            nextMoves = board.FillNextEmptyCell();
            return false;
        }
    }
}
