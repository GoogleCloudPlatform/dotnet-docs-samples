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
using System.Collections.Generic;
using CommandLine;
using Google.Cloud.Dialogflow.V2;

namespace GoogleCloudSamples
{
    // Samples demonstrating how to detect Intents using audio file
    public class DetectIntentAudio
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((DetectIntentFromAudioOptions opts) =>
                     DetectIntentFromAudio(opts.ProjectId, opts.SessionId, opts.AudioFile, opts.LanguageCode));
        }

        [Verb("detect-intent:audio", HelpText = "Detect Intent using provided audio file")]
        public class DetectIntentFromAudioOptions : OptionsWithProjectIdAndSessionId
        {
            [Value(0, MetaName = "audioFile", HelpText = "Path to audio file", Required = true)]
            public string AudioFile { get; set; }

            [Value(1, MetaName = "languageCode", HelpText = "Language code, eg. en-US", Default = "en-US")]
            public string LanguageCode { get; set; }
        }

        // [START dialogflow_detect_intent_audio]
        public static int DetectIntentFromAudio(string projectId,
                                                string sessionId,
                                                string audioFile,
                                                string languageCode = "en-US")
        {
            var client = SessionsClient.Create();

            var response = client.DetectIntent(
                new DetectIntentRequest()
                {
                    InputAudio = Google.Protobuf.ByteString.CopyFrom(
                        File.ReadAllBytes(audioFile)
                    ),
                    SessionAsSessionName = new SessionName(projectId, sessionId),
                    QueryInput = new QueryInput()
                    {
                        AudioConfig = new InputAudioConfig()
                        {
                            AudioEncoding = AudioEncoding.Linear16,
                            SampleRateHertz = 16000,
                            LanguageCode = "en-US"
                        }
                    }
                }
            );

            var queryResult = response.QueryResult;

            Console.WriteLine($"Query text: {queryResult.QueryText}");
            Console.WriteLine($"Intent detected: {queryResult.Intent.DisplayName}");
            Console.WriteLine($"Intent confidence: {queryResult.IntentDetectionConfidence}");
            Console.WriteLine($"Fulfillment text: {queryResult.FulfillmentText}");
            Console.WriteLine();

            return 0;
        }
        // [END dialogflow_detect_intent_audio]
    }
}
