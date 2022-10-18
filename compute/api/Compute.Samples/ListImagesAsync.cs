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

// [START compute_images_list]

using Google.Cloud.Compute.V1;
using System;
using System.Threading.Tasks;

public class ListImagesAsyncSample
{
    public async Task ListImagesAsync(
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
            MaxResults = 100
        };

        // Although the MaxResults parameter is specified in the request, the sequence returned
        // by the ListAsync() method hides the pagination mechanic. The library makes multiple
        // requests to the API for you, so you can simply iterate over all the images.
        await foreach (var image in client.ListAsync(request))
        {
            // The result is an Image collection.
            Console.WriteLine($"Image: {image.Name}");
        }
    }
}

// [END compute_images_list]
