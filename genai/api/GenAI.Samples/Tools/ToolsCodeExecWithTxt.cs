/*
 * Copyright 2025 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START googlegenaisdk_tools_code_exec_with_txt]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public class ToolsCodeExecWithTxt
{
    public async Task<string> GenerateContent(
        string projectId = "your-project-id",
        string location = "global",
        string model = "gemini-2.5-flash")
    {
        await using var client = new Client(
            project: projectId,
            location: location,
            vertexAI: true,
            httpOptions: new HttpOptions { ApiVersion = "v1" });

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: "Calculate 20th fibonacci number. Then find the nearest palindrome to it.",
            config: new GenerateContentConfig
            {
                Tools = new List<Tool> { new Tool { CodeExecution = new ToolCodeExecution() } },
                Temperature = 0
            });

        List<Part> parts = response.Candidates?[0]?.Content?.Parts ?? new List<Part>();
        StringBuilder responseText = new StringBuilder();

        foreach (Part part in parts)
        {
            if (part.Text != null)
            {
                Console.WriteLine($"Text: \n{part.Text}");
                responseText.AppendLine(part.Text);
            }

            if (part.ExecutableCode != null)
            {
                Console.WriteLine($"Code: \n{part.ExecutableCode}");
            }

            if (part.CodeExecutionResult != null)
            {
                Console.WriteLine($"Outcome: \n{part.CodeExecutionResult}");
            }
        }

        // Example response:
        // Text:
        // o calculate the 20th Fibonacci number and find its nearest palindrome, I will perform the following steps:
        // 1.  * *Calculate the 20th Fibonacci number: **I will use a Python function to compute the Fibonacci sequence.
        // ...
        // Code: 
        // ExecutableCode {
        //    Code = def fibonacci(n):
        //      a, b = 0, 1
        //      for _ in range(n):
        //          a, b = b, a + b
        //      return a
        //  fib_20 = fibonacci(20)
        //  print(f'{fib_20=}')
        // , Language = PYTHON }
        //
        // Outcome:
        //  CodeExecutionResult { Outcome = OUTCOME_OK, Output = fib_20 = 6765 }
        //
        // Code: 
        // ExecutableCode {
        //    Code = def is_palindrome(n):
        // ...
        return responseText.ToString();
    }
}
// [END googlegenaisdk_tools_code_exec_with_txt]
