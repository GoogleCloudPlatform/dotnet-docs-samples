/*
 * Copyright 2021 Google LLC
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

// [START compute_images_list_page]

using Google.Cloud.Compute.V1;
using System;
using System.Threading.Tasks;

public class ListImagesPerPageAsyncSample
{
    public async Task ListImagesPerPageAsync(
        // TODO(developer): Set your own default values for these parameters or pass different values when calling this method.
        string projectId = "your-project-id")
    {
        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests.
        ImagesClient client = await ImagesClient.CreateAsync();

        // Make the request to list all non-deprecated images in a project.
        ListImagesRequest request = new ListImagesRequest
        {
            Project = projectId,
            // Listing only non-deprecated images to reduce the size of the reply.
            Filter = "deprecated.state != DEPRECATED",
            // MaxResults indicates the maximum number of items that will be returned per page.
            MaxResults = 10
        };

        // Call the AsRawResponses() method of the returned image sequence to access the page sequece instead
        // This allows you to have more granular control of iteration over paginated results from the API.
        // Each time you access the next page, the library retrieves that page from the API.
        int pageIndex = 0;
        await foreach (var page in client.ListAsync(request).AsRawResponses())
        {
            Console.WriteLine($"Page index: {pageIndex}");
            pageIndex++;
            foreach (var image in page)
            {
                // The result is an Image collection.
                Console.WriteLine($"Image: {image.Name}");
            }
        }
    }
}

// [END compute_images_list_page]
