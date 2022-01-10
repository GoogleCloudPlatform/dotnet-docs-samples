// Copyright 2021 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;

namespace grs_product
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("Please run the application again and pass one of the file names to run.");
            }

            string exampleName = args[0];
            var examples = Attributes.ExampleAttributeHelper.GetExamples(exampleName);
            if (!examples.Any())
            {
                throw new ArgumentException($"Cannot find Example with name '{exampleName}'");
            }

            foreach (var example in examples)
            {
                Attributes.ExampleAttributeHelper.ExecuteExampleMethod(example);
            }
        }
    }
}