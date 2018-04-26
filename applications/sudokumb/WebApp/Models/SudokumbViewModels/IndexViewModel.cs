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

using Sudokumb;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.SudokumbViewModels
{
    public class PuzzleAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            try
            {
                var board = GameBoard.ParseHandInput((string)value);
                return board != null;
            }
            catch (BadGameBoardException e)
            {
                this.ErrorMessage = e.Message;
                return false;
            }
            catch (ArgumentException)
            {
                this.ErrorMessage = "The puzzle must have 81 numbers or dots.";
                return false;
            }
        }
    }

    public class IndexViewForm
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

        [Required]
        [Puzzle]
        [DataType(DataType.MultilineText)]
        [Display(Prompt = SamplePuzzle)]
        public string Puzzle { get; set; }
    }

    public class IndexViewModel
    {
        public IndexViewForm Form { get; set; }
        public string Solution { get; set; }

        public string SolveRequestId { get; set; }
    }
}