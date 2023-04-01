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

// [START videostitcher_create_cdn_key]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Stitcher.V1;
using Google.Protobuf;


public class CreateCdnKeySample
{
    public CdnKey CreateCdnKey(
    string projectId, string location, string cdnKeyId, string hostname,
    string keyName, string privateKey, bool isMediaCdn)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        CdnKey cdnKey = new CdnKey
        {
            Hostname = hostname
        };

        if (isMediaCdn)
        {
            cdnKey.MediaCdnKey = new MediaCdnKey
            {
                KeyName = keyName,
                PrivateKey = ByteString.CopyFromUtf8(privateKey)
            };
        }
        else
        {
            cdnKey.GoogleCdnKey = new GoogleCdnKey
            {
                KeyName = keyName,
                PrivateKey = ByteString.CopyFromUtf8(privateKey)
            };
        }

        CreateCdnKeyRequest request = new CreateCdnKeyRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, location),
            CdnKeyId = cdnKeyId,
            CdnKey = cdnKey
        };

        // Call the API.
        CdnKey newCdnKey = client.CreateCdnKey(request);

        // Return the result.
        return newCdnKey;
    }
}
// [END videostitcher_create_cdn_key]
