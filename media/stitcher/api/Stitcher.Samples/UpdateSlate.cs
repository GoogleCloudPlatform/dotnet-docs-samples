/*
 * Copyright 2022 Google LLC
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

// [START videostitcher_update_slate]

using Google.Cloud.Video.Stitcher.V1;
using Google.Protobuf.WellKnownTypes;

public class UpdateSlateSample
{
    public Slate UpdateSlate(
        string projectId, string location, string slateId, string slateUri)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        UpdateSlateRequest request = new UpdateSlateRequest
        {
            Slate = new Slate
            {
                SlateName = SlateName.FromProjectLocationSlate(projectId, location, slateId),
                Uri = slateUri
            },
            UpdateMask = new FieldMask { Paths = { "uri" } }
        };

        // Call the API.
        Slate slate = client.UpdateSlate(request);

        // Return the result.
        return slate;
    }
}
// [END videostitcher_update_slate]
