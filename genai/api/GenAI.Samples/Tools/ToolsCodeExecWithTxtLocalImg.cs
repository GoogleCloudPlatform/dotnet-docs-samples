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

// [START googlegenaisdk_tools_code_exec_with_txt_local_img]

using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class ToolsCodeExecWithTxtLocalImg
{
    public async Task<string> GenerateContent(
        string projectId = "your-project-id",
        string location = "global",
        string model = "gemini-2.5-flash",
        string localImageFilePath = "path/to/img.png")
    {
        await using var client = new Client(
            project: projectId,
            location: location,
            vertexAI: true,
            httpOptions: new HttpOptions { ApiVersion = "v1" });

        // Image source used in this sample:
        // https://upload.wikimedia.org/wikipedia/commons/thumb/3/3f/Monty_open_door.svg/640px-Monty_open_door.svg.png
        byte[] localImgBytes = File.ReadAllBytes(localImageFilePath);

        string prompt = "Run a simulation of the Monty Hall Problem with 1,000 trials.\n"
              + "Here's how this works as a reminder. In the Monty Hall Problem, you're on a game"
              + " show with three doors. Behind one is a car, and behind the others are goats. You"
              + " pick a door. The host, who knows what's behind the doors, opens a different door"
              + " to reveal a goat. Should you switch to the remaining unopened door?\n"
              + " The answer has always been a little difficult for me to understand when people"
              + " solve it with math - so please run a simulation with Python to show me what the"
              + " best strategy is.\n"
              + " Thank you!";

        var contents = new List<Content>
        {
            new Content
            {
                Role = "user",
                Parts = new List<Part>
                {
                    new Part { InlineData = new Blob { Data = localImgBytes, MimeType = "image/png" } },
                    new Part { Text = prompt }
                }
            }
        };

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: model,
            contents: contents,
            config: new GenerateContentConfig
            {
                Tools = new List<Tool> { new Tool { CodeExecution = new ToolCodeExecution() } },
                // The temperature controls the randomness of the output, ranging from 0.0 to 2.0.
                // A temperature of 0 is more deterministic, meaning that the model can generate more focused and predictable responses.
                // Higher temperatures can lead to less predictable and more creative responses.
                Temperature = 0
            });

        List<Part> parts = response.Candidates?[0]?.Content?.Parts ?? new List<Part>();

        StringBuilder executableCode = new StringBuilder();

        foreach (Part part in parts)
        {
            if (part.ExecutableCode != null)
            {
                Console.WriteLine($"Code: \n{part.ExecutableCode}");
                executableCode.Append(part.ExecutableCode);
            }

            if (part.CodeExecutionResult != null)
            {
                Console.WriteLine($"Outcome: \n{part.CodeExecutionResult}");
            }
        }
        // Example response:
        // Code: 
        // ExecutableCode {
        //    Code = import random
        //    def run_monty_hall_trial(switch_doors):
        //      if len(available_doors_for_host) == 1:
        // ...
        //      print(f"Simulation of {num_trials} trials:")
        //      print(f"Wins if staying: {wins_if_stay} ({wins_if_stay / num_trials:.2%})")
        //      print(f"Wins if switching: {wins_if_switch} ({wins_if_switch / num_trials:.2%})")
        // , Language = PYTHON }
        // Outcome:
        //    CodeExecutionResult {
        //        Outcome = OUTCOME_OK, Output = Simulation of 1000 trials:
        //        Wins if staying: 317(31.70 %)
        //        Wins if switching: 687(68.70 %)
        //    }
        return executableCode.ToString();
    }
}
// [END googlegenaisdk_tools_code_exec_with_txt_local_img]
