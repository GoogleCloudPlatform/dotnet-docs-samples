// Copyright 2023 Google Inc.
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

// [START dlp_inspect_image_listed_infotypes]

using System;
using System.IO;
using System.Linq;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;

public class InspectImageForSensitiveDataWithListedInfoTypes
{
    public static InspectContentResponse InspectImage(
        string projectId,
        string filePath)
    {
        // Instantiate dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the content item by providing the image file and its type.
        var contentItem = new ContentItem
        {
            ByteItem = new ByteContentItem
            {
                Type = ByteContentItem.Types.BytesType.ImagePng,
                Data = ByteString.FromStream(new FileStream(filePath, FileMode.Open))
            }
        };

        // Specify the type of info the inspection will look for.
        var infoTypes = new InfoType[]
        {
            new InfoType { Name = "PHONE_NUMBER" },
            new InfoType { Name = "EMAIL_ADDRESS" },
            new InfoType { Name = "US_SOCIAL_SECURITY_NUMBER" }
        };

        // Construct the Inspect config.
        var inspectConfig = new InspectConfig
        {
            InfoTypes = { infoTypes },
            IncludeQuote = true
        };

        // Construct the request.
        var request = new InspectContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            InspectConfig = inspectConfig,
            Item = contentItem
        };

        // Call the API.
        InspectContentResponse response = dlp.InspectContent(request);

        // Inspect the response.
        var resultFindings = response.Result.Findings;

        Console.WriteLine($"Findings: {resultFindings.Count}");

        foreach (var f in resultFindings)
        {
            var data = from location in f.Location.ContentLocations
                       from b in location.ImageLocation.BoundingBoxes
                       select new { b.Height, b.Width, b.Top, b.Left };

            Console.WriteLine("Info type: " + f.InfoType.Name);
            Console.WriteLine("\tQuote: " + f.Quote);
            Console.WriteLine("\tImageLocations: " + string.Join(",", data));
            Console.WriteLine("\tLikelihood: " + f.Likelihood);
        }

        return response;
    }
}

// [END dlp_inspect_image_listed_infotypes]
