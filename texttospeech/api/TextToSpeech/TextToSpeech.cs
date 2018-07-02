/*
 * Copyright (c) 2018 Google Inc.
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
using System;
using System.IO;
using Google.Cloud.TextToSpeech.V1;

namespace GoogleCloudSamples
{

    [Verb("list", HelpText = "List available voices.")]
    class ListArgs
    {}

    [Verb("synthesize", HelpText = "Synthesize input to audio")]
    class SynthesizeArgs
    {
        [Value(0, HelpText = "The text to translate.",
            Required = true)]
        public string Text { get; set; }

        [Option('f', HelpText = "Source formatting")]
        public string SourceFormat { get; set; }

    }

    [Verb("synthesize-file", HelpText = "Synthesize a file to audio")]
    class SynthesizeFileArgs
    {
        [Value(0, HelpText = "The file to synthesize",
            Required = true)]
        public string Text { get; set; }

        [Option('f', HelpText = "Source formatting")]
        public string SourceFormat { get; set; }
    }

    public class TextToSpeech
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Parser.Default.ParseArguments<ListArgs, SynthesizeArgs,
                SynthesizeFileArgs>(args).MapResult(
                (ListArgs largs) => ListVoices(largs),
                (SynthesizeArgs sargs) => Synthesize(sargs),
                (SynthesizeFileArgs sfargs) => SynthesizeFile(sfargs),
                errs => 1);
        }

        private static int ListVoices(ListArgs args)
        {
            ListVoices();
            return 0;
        }

        // [START tts_list_voices]
        /// <summary>
        /// Lists all the voices available for speech synthesis.
        /// </summary>
        public static void ListVoices()
        {
            TextToSpeechClient client = TextToSpeechClient.Create();

            // Performs the list voices request
            var response = client.ListVoices(new ListVoicesRequest
            {
                // Uncomment the following block and specify a language
                // code to get results for that language only.
                //LanguageCode = "[LANGUAGE_CODE]"
            });

            foreach (Voice voice in response.Voices)
            {
                // Display the voices's name.
                Console.WriteLine($"Name: {voice.Name}");

                // Display the supported language codes for this voice.
                foreach (var languageCode in voice.LanguageCodes)
                {
                    Console.WriteLine($"Supported language(s): {languageCode}");
                }

                // Display the SSML Voice Gender
                Console.WriteLine("SSML Voice Gender: " +
                    (SsmlVoiceGender)voice.SsmlGender);

                // Display the natural sample rate hertz for this voice.
                Console.WriteLine("Natural Sample Rate Hertz: " +
                    voice.NaturalSampleRateHertz);
            }
        }
        // [END tts_list_voices]


        private static int Synthesize(SynthesizeArgs args)
        {
            if (args.SourceFormat != null && args.SourceFormat.Equals("ssml"))
            {
                SynthesizeSSML(args.Text);
            }
            else
            {
                SynthesizeText(args.Text);
            }
            return 0;
        }

        // [START tts_synthesize_text]
        /// <summary>
        /// Creates an audio file from the text input.
        /// </summary>
        /// <param name="text">Text to synthesize into audio</param>
        /// <remarks>
        /// Generates a file named 'output.mp3' in project folder.
        /// </remarks>
        public static void SynthesizeText(string text)
        {
            TextToSpeechClient client = TextToSpeechClient.Create();
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput
                {
                    Text = text
                },
                // Note: voices can also be specified by name
                Voice = new VoiceSelectionParams
                {
                    LanguageCode = "en-US",
                    SsmlGender = SsmlVoiceGender.Female
                },
                AudioConfig = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3
                }
            });

            using (Stream output = File.Create("output.mp3"))
            {
                response.AudioContent.WriteTo(output);
            }
        }
        // [END tts_synthesize_text]

        // [START tts_synthesize_ssml]
        /// <summary>
        /// Creates an audio file from the SSML-formatted string.
        /// </summary>
        /// <param name="ssml">SSML string to synthesize</param>
        /// <remarks>
        /// Generates a file named 'output.mp3' in project folder.
        /// Note: SSML must be well-formed according to:
        ///    https://www.w3.org/TR/speech-synthesis/
        /// </remarks>
        public static void SynthesizeSSML(string ssml)
        {
            TextToSpeechClient client = TextToSpeechClient.Create();
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput
                {
                    Ssml = ssml
                },
                // Note: voices can also be specified by name
                Voice = new VoiceSelectionParams
                {
                    LanguageCode = "en-US",
                    SsmlGender = SsmlVoiceGender.Female
                },
                AudioConfig = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3
                }
            });

            using (Stream output = File.Create("output.mp3"))
            {
                response.AudioContent.WriteTo(output);
            }
        }
        // [END tts_synthesize_ssml]

        private static int SynthesizeFile(SynthesizeFileArgs args)
        {
            if (args.SourceFormat != null && args.SourceFormat.Equals("ssml"))
            {
                SynthesizeSSMLFile(args.Text);
            }
            else
            {
                SynthesizeTextFile(args.Text);
            }
            return 0;
        }

        // [START tts_synthesize_text_file]
        /// <summary>
        /// Creates an audio file from the input file of text.
        /// </summary>
        /// <param name="textFilePath">Path to text file</param>
        /// <remarks>
        /// Generates a file named 'output.mp3' in project folder.
        /// </remarks>
        public static void SynthesizeTextFile(string textFilePath)
        {
            string text = File.ReadAllText(textFilePath);

            TextToSpeechClient client = TextToSpeechClient.Create();
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput
                {
                    Text = text
                },
                // Note: voices can also be specified by name
                Voice = new VoiceSelectionParams
                {
                    LanguageCode = "en-US",
                    SsmlGender = SsmlVoiceGender.Female
                },
                AudioConfig = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3
                }
            });

            using (Stream output = File.Create("output.mp3"))
            {
                response.AudioContent.WriteTo(output);
            }
        }
        // [START tts_synthesize_text_file]

        // [START tts_synthesize_ssml_file]
        /// <summary>
        /// Creates an audio file from the input SSML file.
        /// </summary>
        /// <param name="ssmlFilePath">Path to SSML file</param>
        /// <remarks>
        /// Generates a file named 'output.mp3' in project folder.
        /// Note: SSML must be well-formed according to:
        ///    https://www.w3.org/TR/speech-synthesis/
        /// </remarks>
        public static void SynthesizeSSMLFile(string ssmlFilePath)
        {
            string text = File.ReadAllText(ssmlFilePath);

            TextToSpeechClient client = TextToSpeechClient.Create();
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput
                {
                    Ssml = text
                },
                // Note: voices can also be specified by name
                Voice = new VoiceSelectionParams
                {
                    LanguageCode = "en-US",
                    SsmlGender = SsmlVoiceGender.Female
                },
                AudioConfig = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3
                }
            });

            using (Stream output = File.Create("output.mp3"))
            {
                response.AudioContent.WriteTo(output);
            }
        }
        // [START tts_synthesize_ssml_file]
    }
}
