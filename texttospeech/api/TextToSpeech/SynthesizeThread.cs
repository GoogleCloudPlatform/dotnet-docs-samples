﻿using Google.Cloud.TextToSpeech.V1;
using Google.Type;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GoogleCloudSamples
{
    class SynthesizeThread
    {
        private static int iD = 1;
        private int id = iD++;
        private string testId;

        public SynthesizeThread(string testid)
        {
            testId = testid;
        }

        public void Synthesize()
        {
            Console.WriteLine($"[{id}] start");
            var client = TextToSpeechClient.Create();
            VoiceSelectionParams voice = new VoiceSelectionParams
            {
                LanguageCode = "de-DE",
                Name = "de-DE-Wavenet-A",
                SsmlGender = SsmlVoiceGender.Female
            };
            AudioConfig audio = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Linear16,
                SampleRateHertz = 24000
            };

            //warm up
            client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput
                {
                    Text = "Hallo",
                },
                // Note: voices can also be specified by name
                Voice = voice,
                AudioConfig = audio
            });

            string line = TextToSpeech.GetOne();
            int k = 0;
            double totalFirstTime = 0;
            double totalAudioTime = 0;
            while (line != null)
            {
                k++;
                Console.WriteLine($"[{id}] send: {line}.");
                DateTime stime = DateTime.Now;
                var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
                {
                    Input = new SynthesisInput
                    {
                        Text = line,
                    },
                    Voice = voice,
                    AudioConfig = audio
                });

                //Console.WriteLine($"[{id}] get:" +
                TimeSpan span = DateTime.Now - stime;
                byte[] audioData = response.AudioContent.ToByteArray();
                double audioLength = audioData.Length * 1.0 / 24000 / 2;
                Console.WriteLine($"[{id}] FBL: {span.TotalSeconds}, audio length: {audioLength}.");
                File.WriteAllBytes(testId + "_" + id + "_" + k + ".wav", audioData);
                totalAudioTime += audioLength;
                totalFirstTime += span.TotalSeconds;
                line = TextToSpeech.GetOne();
            }

            TextToSpeech.SumUp(k, totalFirstTime, totalAudioTime);
            Console.WriteLine($"[{id}]Avg FBL: " + (totalFirstTime / k));
        }
    }
}
