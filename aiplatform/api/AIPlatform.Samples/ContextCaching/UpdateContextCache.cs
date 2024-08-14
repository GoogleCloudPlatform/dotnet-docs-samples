/*
 * Copyright 2024 Google LLC
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

// [START generativeaionvertexai_gemini_update_context_cache]

using Google.Cloud.AIPlatform.V1Beta1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading.Tasks;

public class UpdateContextCache
{
    public async Task<Timestamp> UpdateExpireTime(CachedContentName name)
    {
        var client = await new GenAiCacheServiceClientBuilder
        {
            Endpoint = "us-central1-aiplatform.googleapis.com"
        }.BuildAsync();

        var cachedContent = await client.GetCachedContentAsync(new GetCachedContentRequest
        {
            CachedContentName = name
        });
        Console.WriteLine($"Original expire time: {cachedContent.ExpireTime}");

        // Update the expiration time by 2 hours
        cachedContent.Ttl = Duration.FromTimeSpan(TimeSpan.FromHours(2));

        var updatedCachedContent = await client.UpdateCachedContentAsync(new UpdateCachedContentRequest
        {
            CachedContent = cachedContent
        });

        Console.WriteLine($"Updated expire time: {updatedCachedContent.ExpireTime}");

        return updatedCachedContent.ExpireTime;
    }
}

// [END generativeaionvertexai_gemini_update_context_cache]
