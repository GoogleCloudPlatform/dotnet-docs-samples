﻿/*
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

// [START video_stitcher_update_cdn_key]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Stitcher.V1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

public class UpdateCdnKeySample
{
    public CdnKey UpdateCdnKey(
        string projectId, string location, string cdnKeyId, string hostname,
        string gcdnKeyName, string gcdnPrivateKey, string akamaiTokenKey)
    {
        // Create the client.
        VideoStitcherServiceClient client = VideoStitcherServiceClient.Create();

        CdnKey cdnKey = new CdnKey
        {
            CdnKeyName = CdnKeyName.FromProjectLocationCdnKey(projectId, location, cdnKeyId),
            Hostname = hostname
        };

        string path;
        if (akamaiTokenKey is null)
        {
            path = "google_cdn_key";
            cdnKey.GoogleCdnKey = new GoogleCdnKey
            {
                KeyName = gcdnKeyName,
                PrivateKey = ByteString.CopyFromUtf8(gcdnPrivateKey)
            };
        }
        else
        {
            path = "akamai_cdn_key";
            cdnKey.AkamaiCdnKey = new AkamaiCdnKey
            {
                TokenKey = ByteString.CopyFromUtf8(akamaiTokenKey)
            };
        }

        UpdateCdnKeyRequest request = new UpdateCdnKeyRequest
        {
            CdnKey = cdnKey,
            UpdateMask = new FieldMask { Paths = { "hostname", path } }
        };

        // Call the API.
        CdnKey newCdnKey = client.UpdateCdnKey(request);

        // Return the result.
        return newCdnKey;
    }
}
// [END video_stitcher_update_cdn_key]
