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

// [START aiplatform_sdk_extraction]

using Google.Cloud.AIPlatform.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using Value = Google.Protobuf.WellKnownTypes.Value;

// Extractive Question Answering with a Large Language Model
public class PredictTextExtractionSample
{
    public string PredictTextExtraction(
        string projectId = "your-project-id",
        string locationId = "us-central1",
        string publisher = "google",
        string model = "text-bison@001")
    {
        // Initialize client that will be used to send requests.
        // This client only needs to be created once,
        // and can be reused for multiple requests.
        var client = new PredictionServiceClientBuilder
        {
            Endpoint = $"{locationId}-aiplatform.googleapis.com"
        }.Build();

        // Configure the parent resource.
        var endpoint = EndpointName.FromProjectLocationPublisherModel(projectId, locationId, publisher, model);

        // Initialize request argument(s).
        var content = @"
Background: There is evidence that there have been significant changes
in Amazon rainforest vegetation over the last 21,000 years through the Last
Glacial Maximum (LGM) and subsequent deglaciation. Analyses of sediment
deposits from Amazon basin paleo lakes and from the Amazon Fan indicate that
rainfall in the basin during the LGM was lower than for the present, and this
was almost certainly associated with reduced moist tropical vegetation cover
in the basin. There is debate, however, over how extensive this reduction
was. Some scientists argue that the rainforest was reduced to small, isolated
refugia separated by open forest and grassland; other scientists argue that
the rainforest remained largely intact but extended less far to the north,
south, and east than is seen today. This debate has proved difficult to
resolve because the practical limitations of working in the rainforest mean
that data sampling is biased away from the center of the Amazon basin, and
both explanations are reasonably well supported by the available data.

Q: What does LGM stands for?
A: Last Glacial Maximum.

Q: What did the analysis from the sediment deposits indicate?
A: Rainfall in the basin during the LGM was lower than for the present.

Q: What are some of scientists arguments?
A: The rainforest was reduced to small, isolated refugia separated by open forest
and grassland.

Q: There have been major changes in Amazon rainforest vegetation over the last how many years?
A: 21,000.

Q: What caused changes in the Amazon rainforest vegetation?
A: The Last Glacial Maximum (LGM) and subsequent deglaciation

Q: What has been analyzed to compare Amazon rainfall in the past and present?
A: Sediment deposits.

Q: What has the lower rainfall in the Amazon during the LGM been attributed to?
A:";

        var instances = new List<Value>
        {
            Value.ForStruct(new()
            {
                Fields =
                {
                    ["content"] = Value.ForString(content),
                }
            })
        };

        var parameters = Value.ForStruct(new()
        {
            Fields =
            {
                { "temperature", new Value { NumberValue = 0 } },
                { "maxDecodeSteps", new Value { NumberValue = 32 } },
                { "topP", new Value { NumberValue = 0 } },
                { "topK", new Value { NumberValue = 1 } }
            }
        });

        // Make the request.
        var response = client.Predict(endpoint, instances, parameters);

        // Parse and return the content.
        var responseContent = response.Predictions.First().StructValue.Fields["content"].StringValue;
        Console.WriteLine($"Content: {responseContent}");
        return responseContent;
    }
}

// [END aiplatform_sdk_extraction]
