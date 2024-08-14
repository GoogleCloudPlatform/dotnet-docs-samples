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

// [START generativeaionvertexai_gemini_create_context_cache]

using Google.Cloud.AIPlatform.V1Beta1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading.Tasks;

public class CreateContextCache
{
    public async Task<CachedContentName> Create(string projectId)
    {
        var client = await new GenAiCacheServiceClientBuilder
        {
            Endpoint = "us-central1-aiplatform.googleapis.com"
        }.BuildAsync();

        var request = new CreateCachedContentRequest
        {
            Parent = $"projects/{projectId}/locations/us-central1",
            CachedContent = new CachedContent
            {
                Model = $"projects/{projectId}/locations/us-central1/publishers/google/models/gemini-1.5-pro-001",
                SystemInstruction = new Content
                {
                    Parts =
                    {
                        new Part { Text = "You are an expert researcher. You always stick to the facts in the sources provided and"
                            + " never make up new facts. Now look at these research papers, and answer the following questions." }
                    }
                },
                Contents =
                {
                    new Content
                    {
                        Role = "USER",
                        Parts =
                        {
                            new Part { FileData = new() { MimeType = "application/pdf", FileUri = "gs://cloud-samples-data/generative-ai/pdf/2312.11805v3.pdf" } },
                            new Part { FileData = new() { MimeType = "application/pdf", FileUri = "gs://cloud-samples-data/generative-ai/pdf/2403.05530.pdf" } }
                        }
                    }
                },
                Ttl = Duration.FromTimeSpan(TimeSpan.FromMinutes(60))
            }
        };

        var cachedContent = await client.CreateCachedContentAsync(request);
        Console.WriteLine($"Created cache: {cachedContent.CachedContentName}");
        return cachedContent.CachedContentName;
    }
}

// [END generativeaionvertexai_gemini_create_context_cache]
