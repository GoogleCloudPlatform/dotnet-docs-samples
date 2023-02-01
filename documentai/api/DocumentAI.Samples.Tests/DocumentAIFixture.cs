/*
 * Copyright 2023 Google LLC
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


using Google.Api.Gax.ResourceNames;
using Google.Cloud.DocumentAI.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading;
using Xunit;

[CollectionDefinition(nameof(DocumentAIFixture))]
public class DocumentAIFixture : ICollectionFixture<DocumentAIFixture>
{
    public string ProjectId { get; }
    public string LocationId { get; }

    public string ProcessorId { get; }
    public ProcessorName ProcessorName { get; }

    public string LocalPath { get; }

    public string MimeType { get; }

    public DocumentAIFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("Missing GOOGLE_PROJECT_ID environment variable.");
        }

        LocationId = "us";

        ProcessorId = Environment.GetEnvironmentVariable("DOCUMENT_AI_PROCESSOR_ID");
        if (string.IsNullOrEmpty(ProcessorId))
        {
            throw new Exception("Missing DOCUMENT_AI_PROCESSOR_ID environment variable.");
        }
        ProcessorName = ProcessorName.FromProjectLocationProcessor(ProjectId, LocationId, ProcessorId);

        LocalPath = "Resources/Invoice.pdf";
        MimeType = "application/pdf";
    }
}
