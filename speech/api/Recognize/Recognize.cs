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
using Google.Cloud.Speech.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    class Options
    {
        [Value(0, HelpText = "A path to a sound file.  Encoding must be "
            + "Linear16 with a sample rate of 16000.", Required = true)]
        public string FilePath { get; set; }
    }

    class StorageOptions
    {
        [Value(0, HelpText = "A path to a sound file. "
            + "Can be a local file path or a Google Cloud Storage path like "
            + "gs://my-bucket/my-object. "
            + "Encoding must be "
            + "Linear16 with a sample rate of 16000.", Required = true)]
        public string FilePath { get; set; }
    }

    [Verb("sync", HelpText = "Detects speech in an audio file.")]
    class SyncOptions : StorageOptions
    {
        [Option('w', HelpText = "Report the time offsets of individual words.")]
        public bool EnableWordTimeOffsets { get; set; }

        [Option('p', HelpText = "Add punctuation to the transcription.")]
        public bool EnableAutomaticPunctuation { get; set; }

        [Option('m', HelpText = "Select a transcription model.")]
        public String SelectModel { get; set; }

        [Option('e', HelpText = "Use an enhanced transcription model.")]
        public bool UseEnhancedModel { get; set; }

        [Option('c', HelpText = "Set number of channels")]
        public int NumberOfChannels { get; set; }

        [Option('s', HelpText = "Set number of speakers")]
        public int NumberOfSpeakers { get; set; }

        [Option('r', HelpText = "Use recognition metadata")]
        public bool UseRecognitionMetadata { get; set; }

        [Option('l', HelpText = "Add word-level confidence values to transcription.")]
        public bool EnableWordLevelConfidence { get; set; }
    }

    [Verb("with-context", HelpText = "Detects speech in an audio file."
        + " Add additional context on stdin.")]
    class OptionsWithContext : StorageOptions { }

    [Verb("async", HelpText = "Creates a job to detect speech in an audio "
        + "file, and waits for the job to complete.")]
    class AsyncOptions : StorageOptions
    {
        [Option('w', HelpText = "Report the time offsets of individual words.")]
        public bool EnableWordTimeOffsets { get; set; }
    }

    [Verb("sync-creds", HelpText = "Detects speech in an audio file.")]
    class SyncOptionsWithCreds
    {
        [Value(0, HelpText = "A path to a sound file.  Encoding must be "
            + "Linear16 with a sample rate of 16000.", Required = true)]
        public string FilePath { get; set; }

        [Value(1, HelpText = "Path to Google credentials json file.", Required = true)]
        public string CredentialsFilePath { get; set; }
    }

    [Verb("stream", HelpText = "Detects speech in an audio file by streaming "
        + "it to the Speech API.")]
    class StreamingOptions : Options { }

    [Verb("listen", HelpText = "Detects speech in a microphone input stream.")]
    class ListenOptions
    {
        [Value(0, HelpText = "Number of seconds to listen for.", Required = false)]
        public int Seconds { get; set; } = 3;
    }

    [Verb("listen-infinite", HelpText = "Detects speech in a microphone input stream, with no length limit.")]
    class ListenInfiniteOptions
    {
    }

    [Verb("rec", HelpText = "Detects speech in an audio file. Supports other file formats.")]
    class RecOptions : Options
    {
        [Option('b', Default = 16000, HelpText = "Sample rate in bits per second.")]
        public int BitRate { get; set; }

        [Option('e', Default = RecognitionConfig.Types.AudioEncoding.Linear16,
            HelpText = "Audio file encoding format.")]
        public RecognitionConfig.Types.AudioEncoding Encoding { get; set; }
    }

    public class Recognize
    {
        static object Rec(string filePath, int bitRate,
            RecognitionConfig.Types.AudioEncoding encoding)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = encoding,
                SampleRateHertz = bitRate,
                LanguageCode = "en",
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

        // [START speech_transcribe_sync]
        static object SyncRecognize(string filePath)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 16000,
                LanguageCode = "en",
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
        // [END speech_transcribe_sync]

        // [START speech_sync_recognize_words]
        static object SyncRecognizeWords(string filePath)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 16000,
                LanguageCode = "en",
                EnableWordTimeOffsets = true,
            }, RecognitionAudio.FromFile(filePath));
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine($"Transcript: { alternative.Transcript}");
                    Console.WriteLine("Word details:");
                    Console.WriteLine($" Word count:{alternative.Words.Count}");
                    foreach (var item in alternative.Words)
                    {
                        Console.WriteLine($"  {item.Word}");
                        Console.WriteLine($"    WordStartTime: {item.StartTime}");
                        Console.WriteLine($"    WordEndTime: {item.EndTime}");
                    }
                }
            }
            return 0;
        }
        // [END speech_sync_recognize_words]

        // [START speech_transcribe_auto_punctuation]
        static object SyncRecognizePunctuation(string filePath)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 8000,
                LanguageCode = "en",
                EnableAutomaticPunctuation = true,
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
        // [END speech_transcribe_auto_punctuation]

        // [START speech_transcribe_model_selection]
        static object SyncRecognizeModelSelection(string filePath, string model)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 16000,
                LanguageCode = "en",
                // The `model` value must be one of the following:
                // "video", "phone_call", "command_and_search", "default"
                Model = model
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
        // [END speech_transcribe_model_selection]

        // [START speech_transcribe_enhanced_model]
        static object SyncRecognizeEnhancedModel(string filePath)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 8000,
                LanguageCode = "en-US",
                UseEnhanced = true,
                Model = "phone_call",
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
        // [END speech_transcribe_enhanced_model]

        // [START speech_transcribe_multichannel]
        static object SyncRecognizeMultipleChannels(string filePath, int channelCount)
        {
            var speech = SpeechClient.Create();

            // Create transcription request
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                LanguageCode = "en",
                // Configure request to enable multiple channels
                EnableSeparateRecognitionPerChannel = true,
                AudioChannelCount = channelCount
                // Note: Sample uses local file.
            }, RecognitionAudio.FromFile(filePath));

            // Print out the results.
            foreach (var result in response.Results)
            {
                // There can be several transcripts for a chunk of audio.
                // Print out the first (most likely) one here.
                var alternative = result.Alternatives[0];
                Console.WriteLine($"Transcript: {alternative.Transcript}");
                Console.WriteLine($"Channel Tag: {result.ChannelTag}");
            }
            return 0;
        }
        // [END speech_transcribe_multichannel]

        // [START speech_transcribe_diarization]
        static object SyncRecognizeMultipleSpeakers(string filePath, int numberOfSpeakers)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                LanguageCode = "en",
                DiarizationConfig = new SpeakerDiarizationConfig()
                {
                    EnableSpeakerDiarization = true,
                    MinSpeakerCount = 2
                }
            }, RecognitionAudio.FromFile(filePath));

            // Print out the results.
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine($"Transcript: { alternative.Transcript}");
                    Console.WriteLine("Word details:");
                    Console.WriteLine($" Word count:{alternative.Words.Count}");
                    foreach (var item in alternative.Words)
                    {
                        Console.WriteLine($"  {item.Word}");
                        Console.WriteLine($"  Speaker: {item.SpeakerTag}");
                    }
                }
            }

            return 0;
        }
        // [END speech_transcribe_diarization]

        //[START speech_transcribe_recognition_metadata]
        static object SyncRecognizeRecognitionMetadata(string filePath)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Flac,
                LanguageCode = "en",
                Metadata = new RecognitionMetadata()
                {
                    OriginalMediaType = RecognitionMetadata.Types.OriginalMediaType.Audio,
                    OriginalMimeType = "audio/mp3",

                    // The kind of device used to capture the audio
                    RecordingDeviceType = RecognitionMetadata.Types.RecordingDeviceType.OtherIndoorDevice,

                    // Use case of the audio, e.g. PHONE_CALL, DISCUSSION, etc
                    InteractionType = RecognitionMetadata.Types.InteractionType.VoiceSearch,

                    // The name of the defice used to make the recording.
                    // Arbitrary string, e.g. 'Pixel XL', 'VoIP', or other value
                    RecordingDeviceName = "Pixel XL"
                }
            }, RecognitionAudio.FromFile(filePath));

            foreach (var result in response.Results)
            {
                Console.WriteLine($"Transcript: { result.Alternatives[0].Transcript}");
            }
            return 0;
        }
        // [END speech_transcribe_recognition_metadata]


        /// <summary>
        /// Reads a list of phrases from stdin.
        /// </summary>
        static List<string> ReadPhrases()
        {
            Console.Write("Reading phrases from stdin.  Finish with blank line.\n> ");
            var phrases = new List<string>();
            string line = Console.ReadLine();
            while (!string.IsNullOrWhiteSpace(line))
            {
                phrases.Add(line.Trim());
                Console.Write("> ");
                line = Console.ReadLine();
            }
            return phrases;
        }

        static object RecognizeWithContext(string filePath, IEnumerable<string> phrases)
        {
            var speech = SpeechClient.Create();
            var config = new RecognitionConfig()
            {
                SpeechContexts = { new SpeechContext() { Phrases = { phrases } } },
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 16000,
                LanguageCode = "en",
            };
            var audio = IsStorageUri(filePath) ?
                RecognitionAudio.FromStorageUri(filePath) :
                RecognitionAudio.FromFile(filePath);
            var response = speech.Recognize(config, audio);
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine(alternative.Transcript);
                }
            }
            return 0;
        }

        static object SyncRecognizeWithCredentials(string filePath, string credentialsFilePath)
        {
            SpeechClientBuilder builder = new SpeechClientBuilder
            {
                CredentialsPath = credentialsFilePath
            };
            SpeechClient speech = builder.Build();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 16000,
                LanguageCode = "en",
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

        // [START speech_transcribe_sync_gcs]
        static object SyncRecognizeGcs(string storageUri)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 16000,
                LanguageCode = "en",
            }, RecognitionAudio.FromStorageUri(storageUri));
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine(alternative.Transcript);
                }
            }
            return 0;
        }
        // [END speech_transcribe_sync_gcs]

        // [START speech_transcribe_async]
        static object LongRunningRecognize(string filePath)
        {
            var speech = SpeechClient.Create();
            var longOperation = speech.LongRunningRecognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 16000,
                LanguageCode = "en",
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
        // [END speech_transcribe_async]

        // [START speech_transcribe_async_gcs]
        static object AsyncRecognizeGcs(string storageUri)
        {
            var speech = SpeechClient.Create();
            var longOperation = speech.LongRunningRecognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 16000,
                LanguageCode = "en",
            }, RecognitionAudio.FromStorageUri(storageUri));
            longOperation = longOperation.PollUntilCompleted();
            var response = longOperation.Result;
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine($"Transcript: { alternative.Transcript}");
                }
            }
            return 0;
        }
        // [END speech_transcribe_async_gcs]

        // [START speech_transcribe_async_word_time_offsets_gcs]
        static object AsyncRecognizeGcsWords(string storageUri)
        {
            var speech = SpeechClient.Create();
            var longOperation = speech.LongRunningRecognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 16000,
                LanguageCode = "en",
                EnableWordTimeOffsets = true,
            }, RecognitionAudio.FromStorageUri(storageUri));
            longOperation = longOperation.PollUntilCompleted();
            var response = longOperation.Result;
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine($"Transcript: { alternative.Transcript}");
                    Console.WriteLine("Word details:");
                    Console.WriteLine($" Word count:{alternative.Words.Count}");
                    foreach (var item in alternative.Words)
                    {
                        Console.WriteLine($"  {item.Word}");
                        Console.WriteLine($"    WordStartTime: {item.StartTime}");
                        Console.WriteLine($"    WordEndTime: {item.EndTime}");
                    }
                }
            }
            return 0;
        }
        // [END speech_transcribe_async_word_time_offsets_gcs]

        /// <summary>
        /// Stream the content of the file to the API in 32kb chunks.
        /// </summary>
        // [START speech_transcribe_streaming]
        static async Task<object> StreamingRecognizeAsync(string filePath)
        {
            var speech = SpeechClient.Create();
            var streamingCall = speech.StreamingRecognize();
            // Write the initial request with the config.
            await streamingCall.WriteAsync(
                new StreamingRecognizeRequest()
                {
                    StreamingConfig = new StreamingRecognitionConfig()
                    {
                        Config = new RecognitionConfig()
                        {
                            Encoding =
                            RecognitionConfig.Types.AudioEncoding.Linear16,
                            SampleRateHertz = 16000,
                            LanguageCode = "en",
                        },
                        InterimResults = true,
                    }
                });
            // Print responses as they arrive.
            Task printResponses = Task.Run(async () =>
            {
                var responseStream = streamingCall.GetResponseStream();
                while (await responseStream.MoveNextAsync())
                {
                    StreamingRecognizeResponse response = responseStream.Current;
                    foreach (StreamingRecognitionResult result in response.Results)
                    {
                        foreach (SpeechRecognitionAlternative alternative in result.Alternatives)
                        {
                            Console.WriteLine(alternative.Transcript);
                        }
                    }
                }
            });
            // Stream the file content to the API.  Write 2 32kb chunks per
            // second.
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                var buffer = new byte[32 * 1024];
                int bytesRead;
                while ((bytesRead = await fileStream.ReadAsync(
                    buffer, 0, buffer.Length)) > 0)
                {
                    await streamingCall.WriteAsync(
                        new StreamingRecognizeRequest()
                        {
                            AudioContent = Google.Protobuf.ByteString
                            .CopyFrom(buffer, 0, bytesRead),
                        });
                    await Task.Delay(500);
                };
            }
            await streamingCall.WriteCompleteAsync();
            await printResponses;
            return 0;
        }
        // [END speech_transcribe_streaming]

        // [START speech_transcribe_streaming_mic]
        static async Task<object> StreamingMicRecognizeAsync(int seconds)
        {
            var speech = SpeechClient.Create();
            var streamingCall = speech.StreamingRecognize();
            // Write the initial request with the config.
            await streamingCall.WriteAsync(
                new StreamingRecognizeRequest()
                {
                    StreamingConfig = new StreamingRecognitionConfig()
                    {
                        Config = new RecognitionConfig()
                        {
                            Encoding =
                            RecognitionConfig.Types.AudioEncoding.Linear16,
                            SampleRateHertz = 16000,
                            LanguageCode = "en",
                        },
                        InterimResults = true,
                    }
                });
            // Print responses as they arrive.
            Task printResponses = Task.Run(async () =>
            {
                var responseStream = streamingCall.GetResponseStream();
                while (await responseStream.MoveNextAsync())
                {
                    StreamingRecognizeResponse response = responseStream.Current;
                    foreach (StreamingRecognitionResult result in response.Results)
                    {
                        foreach (SpeechRecognitionAlternative alternative in result.Alternatives)
                        {
                            Console.WriteLine(alternative.Transcript);
                        }
                    }
                }
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

                        streamingCall.WriteAsync(
                            new StreamingRecognizeRequest()
                            {
                                AudioContent = Google.Protobuf.ByteString
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

            await streamingCall.WriteCompleteAsync();
            await printResponses;
            return 0;
        }
        // [END speech_transcribe_streaming_mic]

        static bool IsStorageUri(string s) => s.Substring(0, 4).ToLower() == "gs:/";

        public static int Main(string[] args)
        {
            return (int)Parser.Default.ParseArguments<
                SyncOptions, AsyncOptions,
                StreamingOptions, ListenOptions, ListenInfiniteOptions,
                RecOptions, SyncOptionsWithCreds,
                OptionsWithContext
                >(args).MapResult(
                (SyncOptions opts) => IsStorageUri(opts.FilePath) ?
                    SyncRecognizeGcs(opts.FilePath) : opts.EnableWordTimeOffsets ?
                    SyncRecognizeWords(opts.FilePath) : opts.EnableAutomaticPunctuation ?
                    SyncRecognizePunctuation(opts.FilePath) : (opts.SelectModel != null) ?
                    SyncRecognizeModelSelection(opts.FilePath, opts.SelectModel) : opts.UseEnhancedModel ?
                    SyncRecognizeEnhancedModel(opts.FilePath) : (opts.NumberOfChannels > 1) ?
                    SyncRecognizeMultipleChannels(opts.FilePath, opts.NumberOfChannels) : (opts.NumberOfSpeakers > 1) ?
                    SyncRecognizeMultipleSpeakers(opts.FilePath, opts.NumberOfSpeakers) : opts.UseRecognitionMetadata ?
                    SyncRecognizeRecognitionMetadata(opts.FilePath) : SyncRecognize(opts.FilePath),
                (AsyncOptions opts) => IsStorageUri(opts.FilePath) ?
                    (opts.EnableWordTimeOffsets ? AsyncRecognizeGcsWords(opts.FilePath)
                    : AsyncRecognizeGcs(opts.FilePath))
                    : LongRunningRecognize(opts.FilePath),
                (StreamingOptions opts) => StreamingRecognizeAsync(opts.FilePath).Result,
                (ListenOptions opts) => StreamingMicRecognizeAsync(opts.Seconds).Result,
                (ListenInfiniteOptions opts) => InfiniteStreaming.RecognizeAsync().Result,
                (RecOptions opts) => Rec(opts.FilePath, opts.BitRate, opts.Encoding),
                (SyncOptionsWithCreds opts) => SyncRecognizeWithCredentials(
                    opts.FilePath, opts.CredentialsFilePath),
                (OptionsWithContext opts) => RecognizeWithContext(opts.FilePath, ReadPhrases()),
                errs => 1);
        }
    }
}