/*
 * Copyright (c) 2017 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Sudokumb
{
    public class GameBoardTests
    {
        private static readonly string s_boardA =
            "123|   |789" +
            "   |   |   " +
            "   |   |   " +
            "---+---+---" +
            "   |4  |   " +
            " 7 | 5 |   " +
            "   |  6| 2 " +
            "---+---+---" +
            "  1|   |   " +
            " 5 |  3|   " +
            "3  |   |1  ";

        private readonly GameBoard _boardA = GameBoard.Create(s_boardA);

        [Fact]
        public void TestAccessors()
        {
            Assert.Equal("123   789", _boardA.Row(0));
            Assert.Equal(" 7  5    ", _boardA.Row(4));
            Assert.Equal("3     1  ", _boardA.Row(8));

            Assert.Equal("1       3", _boardA.Column(0));
            Assert.Equal("    5    ", _boardA.Column(4));
            Assert.Equal("9        ", _boardA.Column(8));

            Assert.Equal("123      ", _boardA.Group(0, 0));
            Assert.Equal("    7    ", _boardA.Group(4, 1));
            Assert.Equal("      1  ", _boardA.Group(8, 8));
        }

        [Fact]
        public void TestFillNextEmpty()
        {
            var expectedNextBoards = new[]
            {
                "123|5  |789" +
                "   |   |   " +
                "   |   |   " +
                "---+---+---" +
                "   |4  |   " +
                " 7 | 5 |   " +
                "   |  6| 2 " +
                "---+---+---" +
                "  1|   |   " +
                " 5 |  3|   " +
                "3  |   |1  ",

                "123|6  |789" +
                "   |   |   " +
                "   |   |   " +
                "---+---+---" +
                "   |4  |   " +
                " 7 | 5 |   " +
                "   |  6| 2 " +
                "---+---+---" +
                "  1|   |   " +
                " 5 |  3|   " +
                "3  |   |1  "
            }.Select((board) => GameBoard.Create(board));

            var nextBoards = _boardA.FillNextEmptyCell();
            Assert.Equal(expectedNextBoards.Count(), nextBoards.Count());
            for (int i = 0; i < expectedNextBoards.Count(); ++i)
            {
                Assert.Equal(expectedNextBoards.ElementAt(i).Board, nextBoards.ElementAt(i).Board);
            }
        }

        [Fact]
        public void TestParseHandInput()
        {
            using (Stream m = File.OpenRead("SampleBoard.txt"))
            {
                GameBoard a = GameBoard.ParseHandInput(m);
                Assert.Equal(_boardA.Board, a.Board);
            }
        }
    }
}
