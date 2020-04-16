// Copyright (c) 2020 Google LLC.
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

using Google.Cloud.Bigtable.V2;
using System;

namespace Filters
{
    public class PrintRow
    {
        /// <summary>
        /// Prints the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        public static string Print(Row row)
        {
            String result = $"Reading data for {row.Key.ToStringUtf8()}\n";
            foreach (Family family in row.Families)
            {
                result += $"Column Family {family.Name}\n";

                foreach (Column column in family.Columns)
                {
                    foreach (Cell cell in column.Cells)
                    {
                        result += $"\t{column.Qualifier.ToStringUtf8()}: {cell.Value.ToStringUtf8()} @{cell.TimestampMicros} {cell.Labels}\n";
                    }
                }
            }
            result += "\n";
            Console.WriteLine(result);
            return result;
        }
    }
}
