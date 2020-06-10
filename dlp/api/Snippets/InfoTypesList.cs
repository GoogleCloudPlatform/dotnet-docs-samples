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

// [START dlp_list_info_types]

using System;
using Google.Cloud.Dlp.V2;

public class InfoTypesList
{
    public static ListInfoTypesResponse ListInfoTypes(string languageCode, string filter)
    {
        var dlp = DlpServiceClient.Create();
        var response = dlp.ListInfoTypes(
            new ListInfoTypesRequest
            {
                LanguageCode = languageCode,
                Filter = filter
            });

        // Uncomment to print infotypes
        // Console.WriteLine("Info Types:");
        // foreach (var InfoType in response.InfoTypes)
        // {
        //     Console.WriteLine($"\t{InfoType.Name} ({InfoType.DisplayName})");
        // }

        return response;
    }
}

// [END dlp_list_info_types]
