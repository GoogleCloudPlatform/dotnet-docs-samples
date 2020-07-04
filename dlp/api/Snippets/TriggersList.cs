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

// [START dlp_list_triggers]

using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using System;

public class TriggersList
{
    public static PagedEnumerable<ListJobTriggersResponse, JobTrigger> List(string projectId)
    {
        var dlp = DlpServiceClient.Create();

        var response = dlp.ListJobTriggers(
            new ListJobTriggersRequest
            {
                Parent = new LocationName(projectId, "global").ToString(),
            });

        foreach (var trigger in response)
        {
            Console.WriteLine($"Name: {trigger.Name}");
            Console.WriteLine($"  Created: {trigger.CreateTime}");
            Console.WriteLine($"  Updated: {trigger.UpdateTime}");
            Console.WriteLine($"  Display Name: {trigger.DisplayName}");
            Console.WriteLine($"  Description: {trigger.Description}");
            Console.WriteLine($"  Status: {trigger.Status}");
            Console.WriteLine($"  Error count: {trigger.Errors.Count}");
        }

        return response;
    }
}

// [END dlp_list_triggers]
