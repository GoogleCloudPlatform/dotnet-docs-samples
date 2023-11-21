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

// [START aiplatform_sdk_sentiment_analysis]

using Google.Cloud.AIPlatform.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using Value = Google.Protobuf.WellKnownTypes.Value;

// Text sentiment analysis with a Large Language Model
public class PredictTextSentimentSample
{
    public string PredictTextSentiment(
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
        var content = @"I had to compare two versions of Hamlet for my Shakespeare
class and unfortunately I picked this version. Everything from the acting
(the actors deliver most of their lines directly to the camera) to the camera
shots (all medium or close up shots...no scenery shots and very little back
ground in the shots) were absolutely terrible. I watched this over my spring
break and it is very safe to say that I feel that I was gypped out of 114
minutes of my vacation. Not recommended by any stretch of the imagination.
Classify the sentiment of the message: negative

Something surprised me about this movie - it was actually original. It was
not the same old recycled crap that comes out of Hollywood every month. I saw
this movie on video because I did not even know about it before I saw it at my
local video store. If you see this movie available - rent it - you will not
regret it.
Classify the sentiment of the message: positive

My family has watched Arthur Bach stumble and stammer since the movie first
came out. We have most lines memorized. I watched it two weeks ago and still
get tickled at the simple humor and view-at-life that Dudley Moore portrays.
Liza Minelli did a wonderful job as the side kick - though I'm not her
biggest fan. This movie makes me just enjoy watching movies. My favorite scene
is when Arthur is visiting his fiancée's house. His conversation with the
butler and Susan's father is side-spitting. The line from the butler,
""Would you care to wait in the Library"" followed by Arthur's reply,
""Yes I would, the bathroom is out of the question"" is my NEWMAIL
notification on my computer.
Classify the sentiment of the message: positive

This Charles outing is decent but this is a pretty low-key performance. Marlon
Brando stands out. There's a subplot with Mira Sorvino and Donald Sutherland
that forgets to develop and it hurts the film a little. I'm still trying to
figure out why Charlie want to change his name.
Classify the sentiment of the message: negative

Tweet: The Pixel 7 Pro, is too big to fit in my jeans pocket, so I bought new
jeans.
Classify the sentiment of the message: ";

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
                { "maxDecodeSteps", new Value { NumberValue = 5 } },
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

// [END aiplatform_sdk_sentiment_analysis]
