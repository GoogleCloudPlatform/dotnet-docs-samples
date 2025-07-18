/*
 * Copyright 2025 Google LLC
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

// [START modelarmor_scan_pdf_file]
using System;
using System.IO;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.ModelArmor.V1;
using Google.Protobuf;
using Newtonsoft.Json;

namespace ModelArmor.Samples
{
    public class ScanPdfFileSample
    {
        public SanitizeUserPromptResponse ScanPdfFile(
            string projectId = "my-project",
            string locationId = "us-central1",
            string templateId = "my-template",
            string pdfFilePath = "path/to/file.pdf"
        )
        {
            // Endpoint to call the Model Armor server.
            ModelArmorClientBuilder clientBuilder = new ModelArmorClientBuilder
            {
                Endpoint = $"modelarmor.{locationId}.rep.googleapis.com",
            };

            // Create the client.
            ModelArmorClient client = clientBuilder.Build();

            // Build the resource name of the template.
            TemplateName templateName = TemplateName.FromProjectLocationTemplate(
                projectId,
                locationId,
                templateId
            );

            // Read the PDF file content
            byte[] fileContent = File.ReadAllBytes(pdfFilePath);

            // Prepare the request with PDF data
            SanitizeUserPromptRequest request = new SanitizeUserPromptRequest
            {
                TemplateName = templateName,
                UserPromptData = new DataItem
                {
                    ByteItem = new ByteDataItem
                    {
                        ByteDataType = ByteDataItem.Types.ByteItemType.Pdf,
                        ByteData = ByteString.CopyFrom(fileContent),
                    },
                },
            };

            // Send the request and get the response.
            SanitizeUserPromptResponse response = client.SanitizeUserPrompt(request);

            return response;
        }
    }
}
// [END modelarmor_scan_pdf_file]
