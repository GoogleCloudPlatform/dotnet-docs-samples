// Copyright (c) 2020 Google LLC.
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

// [START speech_context_classes]

using Google.Cloud.Speech.V1;
using System;

namespace GoogleCloudSamples
{
    public class SpeechContextClasses
    {
        /// <summary>
        /// Provides "hints" to the speech recognizer to favor specific classes of words in the results.
        ///</summary>
        /// <param name="uriPath">Path to the audio file stored on GCS.</param>
        public static object TranscribeContextClasses(
            string uriPath = "gs://cloud-samples-data/speech/brooklyn_bridge.mp3")
        {
            var speechClient = SpeechClient.Create();
            SpeechContext speechContext = new SpeechContext();
            // SpeechContext: to configure your speech_context see:
            // https://cloud.google.com/speech-to-text/docs/reference/rpc/google.cloud.speech.v1#speechcontext
            // Full list of supported phrases (class tokens) here:
            // https://cloud.google.com/speech-to-text/docs/class-tokens
            speechContext.Phrases.Add("$TIME");

            // RecognitionConfig: to configure your encoding and sample_rate_hertz, see:
            // https://cloud.google.com/speech-to-text/docs/reference/rpc/google.cloud.speech.v1#recognitionconfig
            RecognitionConfig recognitionConfig = new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 8000,
                LanguageCode = "en-US"
            };
            recognitionConfig.SpeechContexts.Add(speechContext);

            // Set the path to your audio file
            RecognitionAudio audio = new RecognitionAudio
            {
                Uri = uriPath
            };

            // Build the request
            RecognizeRequest request = new RecognizeRequest
            {
                Config = recognitionConfig,
                Audio = audio
            };

            // Perform the request
            var response = speechClient.Recognize(request);
            foreach (SpeechRecognitionResult result in response.Results)
            {
                // First alternative is the most probable result
                var alternative = result.Alternatives[0];
                Console.WriteLine($"Transcript: {alternative.Transcript}");
            }
            return 0;
        }
    }
}
// [END speech_context_classes]