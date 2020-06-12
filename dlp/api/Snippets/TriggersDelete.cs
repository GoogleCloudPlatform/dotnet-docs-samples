// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START dlp_delete_trigger]

using Google.Cloud.Dlp.V2;
using System;

public class TriggersDelete
{

    public static void Delete(string triggerName)
    {
        var dlp = DlpServiceClient.Create();

        dlp.DeleteJobTrigger(
            new DeleteJobTriggerRequest
            {
                Name = triggerName
            });

        Console.WriteLine($"Successfully deleted trigger {triggerName}.");
    }
}

// [END dlp_delete_trigger]
