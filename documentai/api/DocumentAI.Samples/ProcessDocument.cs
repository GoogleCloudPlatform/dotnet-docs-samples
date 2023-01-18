// Copyright 2023 Google Inc.
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

// [START documentai_process_document]
// [START documentai_quickstart]

using Google.Cloud.DocumentAI.V1;
using System;
using System.IO;

public class ProcessDocumentSample
{
    public void ProcessDocument(
        string projectId = "your-project-id",
        string location = "your-processor-location",
        string processorId = "your-processor-id",
        string localPath = "my-local-path/my-file-name",
        string mimeType = "application/pdf"
    )
    {
        // Create client
        var documentai = DocumentProcessorServiceClient.Create();

        // Read in local file
        using var fileStream = File.OpenRead(localPath);
        var rawDocument = new RawDocument {
            Content = fileStream,
            MimeType = mimeType
        };

        // Initialize request argument(s)
        var request = new ProcessRequest
        {
            Name = ProcessorName.FromProjectLocationProcessor(projectId, location, processorId),
            RawDocument = rawDocument
        };

        // Make the request
        var response = documentProcessorServiceClient.ProcessDocument(request);

        var document = response.Document;

        Console.WriteLine($"{document.Text}");
    }
}

// [END documentai_quickstart]
// [END documentai_process_document]
