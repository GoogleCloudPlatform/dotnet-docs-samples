// Copyright(c) 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using CommandLine;
using Google.Cloud.Dialogflow.V2;

namespace GoogleCloudSamples
{
    // Samples demonstrating how to detect Intents using a stream
    public class DetectIntentStreaming
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((DetectIntentFromStreamOptions opts) =>
                     DetectIntentFromStream(opts.ProjectId, opts.SessionId, opts.AudioFile, opts.LanguageCode).Result);
        }

        [Verb("detect-intent:stream", HelpText = "Detect Intent using a stream")]
        public class DetectIntentFromStreamOptions : OptionsWithProjectIdAndSessionId
        {
            [Value(0, MetaName = "audioFile", HelpText = "Path to audio file", Required = true)]
            public string AudioFile { get; set; }

            [Value(1, MetaName = "languageCode", HelpText = "Language code, eg. en-US", Default = "en-US")]
            public string LanguageCode { get; set; }
        }

        // [START dialogflow_detect_intent_stream]
        public async static Task<object> DetectIntentFromStream(string projectId,
                                                string sessionId,
                                                string audioFile,
                                                string languageCode = "en-US")
        {
            var client = SessionsClient.Create();

            var streamingCall = client.StreamingDetectIntent();

            // Write the initial request with the config.
            await streamingCall.WriteAsync(
                new StreamingDetectIntentRequest()
                {
                    Session = new SessionName(projectId, sessionId).ToString(),
                    QueryInput = new QueryInput()
                    {
                        AudioConfig = new InputAudioConfig()
                        {
                            AudioEncoding = AudioEncoding.Linear16,
                            SampleRateHertz = 16000,
                            LanguageCode = "en-US"
                        }
                    },
                    SingleUtterance = true
                }
            );

            // Print responses as they arrive.
            Task printResponses = Task.Run(async () =>
            {
                while (await streamingCall.ResponseStream.MoveNext(default(CancellationToken)))
                {
                    var streamingResponse = streamingCall.ResponseStream.Current;

                    if (streamingResponse.RecognitionResult != null)
                    {
                        Console.WriteLine("Intermediate transcript:");
                        Console.WriteLine(streamingResponse.RecognitionResult.Transcript);
                    }
                    else
                    {
                        Console.WriteLine("Detected intent:");
                        Console.WriteLine(streamingResponse.QueryResult.Intent.DisplayName);
                    }
                }
            });

            // Stream the file content to the API. Write 2 32kb chunks per second.
            using (FileStream fileStream = new FileStream(audioFile, FileMode.Open))
            {
                var buffer = new byte[32 * 1024];
                int bytesRead;
                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await streamingCall.WriteAsync(
                        new StreamingDetectIntentRequest()
                        {
                            InputAudio = Google.Protobuf.ByteString.CopyFrom(buffer, 0, bytesRead),
                        });
                    await Task.Delay(500);
                };
            }

            await streamingCall.WriteCompleteAsync();
            await printResponses;
            return 0;

        }
        // [END dialogflow_detect_intent_stream]
    }
}
