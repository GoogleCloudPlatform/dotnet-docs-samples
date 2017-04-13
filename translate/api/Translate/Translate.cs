// Copyright(c) 2017 Google Inc.
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
//

using CommandLine;
using Google.Cloud.Translation.V2;
using System;

namespace GoogleCloudSamples
{
    [Verb("translate", HelpText = "Translate text.")]
    class TranslateArgs
    {
        [Value(0, HelpText = "The text to translate.",
            Required = true)]
        public string Text { get; set; }

        [Option('s', HelpText = "Source language code.")]
        public string SourceLanguage { get; set; }

        [Option('t', HelpText = "Target language code.", Default = "ru")]
        public string TargetLanguage { get; set; }

        [Option('p', HelpText = "Use the premium translation model.")]
        public bool PremiumModel { get; set; }
    }

    [Verb("list", HelpText = "List available languages.")]
    class ListArgs
    {
        [Option('t', HelpText = "Also list the language name in the target language.")]
        public string TargetLanguage { get; set; }
    }

    [Verb("detect", HelpText = "Detects which language some text is written in.")]
    class DetectArgs
    {
        [Value(0, HelpText = "The text to examine.",
            Required = true)]
        public string Text { get; set; }
    }

    public class Translator
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Parser.Default.ParseArguments<TranslateArgs, ListArgs,
                DetectArgs>(args).MapResult(
                (TranslateArgs targs) => targs.PremiumModel ?
                    TranslateWithModel(targs) :
                    Translate(targs),
                (ListArgs largs) => largs.TargetLanguage == null ?
                    ListLanguageCodes() :
                    ListLanguages(largs.TargetLanguage),
                (DetectArgs dargs) => DetectLanguage(dargs.Text),
                errs => 1);
        }

        // [START translate_translate_text]
        static void Translate(string text, string targetLanguageCode,
            string sourceLanguageCode)
        {
            TranslationClient client = TranslationClient.Create();
            var response = client.TranslateText(text, targetLanguageCode,
                sourceLanguageCode);
            Console.WriteLine(response.TranslatedText);
        }
        // [END translate_translate_text]

        static object Translate(TranslateArgs args)
        {
            Translate(args.Text, args.TargetLanguage, args.SourceLanguage);
            return 0;
        }

        // [START translate_list_codes]
        static object ListLanguageCodes()
        {
            TranslationClient client = TranslationClient.Create();
            foreach (var language in client.ListLanguages())
            {
                Console.WriteLine("{0}", language.Code);
            }
            return 0;
        }
        // [END translate_list_codes]

        // [START translate_list_language_names]
        static object ListLanguages(string targetLanguageCode)
        {
            TranslationClient client = TranslationClient.Create();
            foreach (var language in client.ListLanguages(targetLanguageCode))
            {
                Console.WriteLine("{0}\t{1}", language.Code, language.Name);
            }
            return 0;
        }
        // [END translate_list_language_names]

        // [START translate_detect_language]
        static object DetectLanguage(string text)
        {
            TranslationClient client = TranslationClient.Create();
            foreach (var detection in client.DetectLanguage(text))
            {
                Console.WriteLine("{0}\tConfidence: {1}",
                    detection.Language, detection.Confidence);
            }
            return 0;
        }
        // [END translate_detect_language]

        static object TranslateWithModel(TranslateArgs args)
        {
            TranslateWithModel(args.Text, args.TargetLanguage,
                args.SourceLanguage,
                TranslationModel.NeuralMachineTranslation);
            return 0;
        }

        // [START translate_text_with_model]
        static void TranslateWithModel(string text,
            string targetLanguageCode, string sourceLanguageCode,
            TranslationModel model)
        {
            TranslationClient client = TranslationClient.Create();
            var response = client.TranslateText(text,
                targetLanguageCode, sourceLanguageCode, model);
            Console.WriteLine("Model: {0}", response.Model);
            Console.WriteLine(response.TranslatedText);
        }
        // [END translate_text_with_model]
    }
}
