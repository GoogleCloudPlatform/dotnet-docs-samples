/*
 * Copyright (c) 2017 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using CommandLine;
using Google.Cloud.Speech.V1Beta1;
using System;

namespace GoogleCloudSamples
{
    class Options
    {
        [Value(0, HelpText = "A path to a sound file.  Encoding must be "
            + "Linear16 with a sample rate of 16000.", Required = true)]
        public string FilePath { get; set; }
    }

    [Verb("sync", HelpText = "Detects speech in an audio file.")]
    class SyncOptions : Options { }

    [Verb("async", HelpText = "Creates a job to detect speech in an audio "
        + "file, and waits for the job to complete.")]
    class AsyncOptions : Options { }


    public class Recognize
    {
        // [START speech_sync_recognize]
        static object SyncRecognize(string filePath)
        {
            var speech = SpeechClient.Create();
            var response = speech.SyncRecognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRate = 16000,
            }, RecognitionAudio.FromFile(filePath));
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine(alternative.Transcript);
                }
            }
            return 0;
        }
        // [END speech_sync_recognize]

        // [START speech_async_recognize]
        static object AsyncRecognize(string filePath)
        {
            var speech = SpeechClient.Create();
            var longOperation = speech.AsyncRecognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRate = 16000,
            }, RecognitionAudio.FromFile(filePath));
            longOperation = longOperation.PollUntilCompleted();
            var response = longOperation.Result;
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine(alternative.Transcript);
                }
            }
            return 0;
        }
        // [END speech_async_recognize]

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                SyncOptions, AsyncOptions>(args).MapResult(
                (SyncOptions opts) => SyncRecognize(opts.FilePath),
                (AsyncOptions opts) => AsyncRecognize(opts.FilePath),
                errs => 1);
        }
    }
}
