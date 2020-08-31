// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START tts_synthesize_text_file]

using System.IO;
using Google.Cloud.TextToSpeech.V1;

public class SynthesizeTextFileSample
{
    public void SynthesizeTextFile(string textFilePath, string outputFile)
    {
        string text = File.ReadAllText(textFilePath);

        var client = TextToSpeechClient.Create();
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

        using (Stream output = File.Create(outputFile))
        {
            response.AudioContent.WriteTo(output);
        }
    }
}
// [END tts_synthesize_text_file]
