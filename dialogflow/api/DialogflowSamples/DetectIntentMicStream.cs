// Copyright(c) 2020 Google Inc.
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

using CommandLine;
using Google.Cloud.Dialogflow.V2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NAudio;

namespace GoogleCloudSamples
{
    public class DetectIntentMicStream
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((DetectIntentFromStreamOptions opts) =>
                     DetectIntentFromStreamAsync(opts.ProjectId, opts.SessionId, opts.Seconds).Result);
        }

        [Verb("detect-intent:streams", HelpText = "Detect intent from stream")]
        public class DetectIntentFromStreamOptions : OptionsWithProjectIdAndSessionId
        {
            [Value(0, MetaName = "seconds", HelpText = "Time to wait", Required = true)]
            public int Seconds { get; set; }
        }

        // [START dialogflow_detect_intent_streaming]
        public static async Task<object> DetectIntentFromStreamAsync(
            string projectId,
            string sessionId,
            string filePath)
        {
            var sessionsClient = SessionsClient.Create();
            var sessionName = SessionName.FromProjectSession(projectId, sessionId).ToString();

            // Initialize streaming call, retrieving the stream object
            var streamingDetectIntent = sessionsClient.StreamingDetectIntent();

            // Define a task to process results from the API
            var responseHandlerTask = Task.Run(async () =>
            {
                var responseStream = streamingDetectIntent.GetResponseStream();
                while (await responseStream.MoveNextAsync())
                {
                    var response = responseStream.Current;
                    var queryResult = response.QueryResult;

                    if (queryResult != null)
                    {
                        Console.WriteLine($"Query text: {queryResult.QueryText}");
                        if (queryResult.Intent != null)
                        {
                            Console.Write("Intent detected:");
                            Console.WriteLine(queryResult.Intent.DisplayName);
                        }
                    }
                }
            });

            // Instructs the speech recognizer how to process the audio content.
            // Note: hard coding audioEncoding, sampleRateHertz for simplicity.
            var queryInput = new QueryInput
            {
                AudioConfig = new InputAudioConfig
                {
                    AudioEncoding = AudioEncoding.Linear16,
                    LanguageCode = "en-US",
                    SampleRateHertz = 16000
                }
            };

            // The first request must **only** contain the audio configuration:
            await streamingDetectIntent.WriteAsync(new StreamingDetectIntentRequest
            {
                QueryInput = queryInput,
                Session = sessionName
            });

            // Read from the microphone and stream to API.
            object writeLock = new object();
            bool writeMore = true;
            var waveIn = new NAudio.Wave.WaveInEvent();
            waveIn.DeviceNumber = 0;
            waveIn.WaveFormat = new NAudio.Wave.WaveFormat(16000, 1);
            waveIn.DataAvailable +=
                (object sender, NAudio.Wave.WaveInEventArgs args) =>
                {
                    lock (writeLock)
                    {
                        if (!writeMore)
                        {
                            return;
                        }

                        streamingDetectIntent.WriteAsync(
                            new StreamingDetectIntentRequest()
                            {
                                InputAudio = Google.Protobuf.ByteString
                                    .CopyFrom(args.Buffer, 0, args.BytesRecorded)
                            }).Wait();
                    }
                };
            waveIn.StartRecording();
            Console.WriteLine("Speak now.");
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            // Stop recording and shut down.
            waveIn.StopRecording();
            lock (writeLock)
            {
                writeMore = false;
            }

            // Tell the service you are done sending data
            await streamingDetectIntent.WriteCompleteAsync();

            // This will complete once all server responses have been processed.
            await responseHandlerTask;

            return 0;
        }
        // [END dialogflow_detect_intent_streaming]
    }
}
