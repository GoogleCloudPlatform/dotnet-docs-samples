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

// [START generativeaionvertexai_imagen_generate_image]

using Google.Cloud.AIPlatform.V1;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Value = Google.Protobuf.WellKnownTypes.Value;

public class GenerateImage
{
    public async Task<FileInfo> Generate(
        string projectId = "your-project-id")
    {
        var predictionServiceClient = new PredictionServiceClientBuilder
        {
            Endpoint = "us-central1-aiplatform.googleapis.com"
        }.Build();


        string prompt = "a dog reading a newspaper";
        string outputFileName = "dog_newspaper.png";
        string model = "imagegeneration@006";

        var predictRequest = new PredictRequest
        {
            EndpointAsEndpointName = EndpointName.FromProjectLocationPublisherModel(projectId, "us-central1", "google", model),
            Instances =
            {
                Value.ForStruct(new()
                {
                    Fields =
                    {
                        ["prompt"] = Value.ForString(prompt)
                    }
                })
            },
            Parameters = Value.ForStruct(new()
            {
                Fields =
                {
                    ["sampleCount"] = Value.ForNumber(1)
                }
            })
        };

        PredictResponse response = await predictionServiceClient.PredictAsync(predictRequest);
        byte[] imageBytes = Convert.FromBase64String(response.Predictions.First().StructValue.Fields["bytesBase64Encoded"].StringValue);

        File.WriteAllBytes(outputFileName, imageBytes);
        FileInfo fileInfo = new FileInfo(Path.GetFullPath(outputFileName));

        Console.WriteLine($"Created output image {fileInfo.FullName} with {fileInfo.Length} bytes");
        return fileInfo;
    }
}

// [END generativeaionvertexai_imagen_generate_image]
