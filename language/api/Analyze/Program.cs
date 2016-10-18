// Copyright(c) 2016 Google Inc.
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

using Google.Cloud.Language.V1Beta1;
using System;
using System.Collections.Generic;
using static Google.Cloud.Language.V1Beta1.AnnotateTextRequest.Types;

namespace GoogleCloudSamples
{
    public class Analyze
    {
        public static string Usage = @"Usage:
C:\> Analyze command text

Where command is one of
    entities
    sentiment
    syntax
    everything
";

        private static void AnalyzeEntities(string text)
        {
            var client = LanguageServiceClient.Create();
            var response = client.AnalyzeEntities(new Document()
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            });
            WriteEntities(response.Entities);
        }

        private static void WriteEntities(IEnumerable<Entity> entities)
        {
            Console.WriteLine("Entities:");
            foreach (var entity in entities)
            {
                Console.WriteLine($"\tName: {entity.Name}");
                Console.WriteLine($"\tType: {entity.Type}");
                Console.WriteLine($"\tSalience: {entity.Salience}");
                Console.WriteLine("\tMentions:");
                foreach (var mention in entity.Mentions)
                    Console.WriteLine($"\t\t{mention.Text.BeginOffset}: {mention.Text.Content}");
                Console.WriteLine("\tMetadata:");
                foreach (var keyval in entity.Metadata)
                    Console.WriteLine($"\t\t{keyval.Key}: {keyval.Value}");
            }
        }

        private static void AnalyzeSentiment(string text)
        {
            var client = LanguageServiceClient.Create();
            var response = client.AnalyzeSentiment(new Document()
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            });
            WriteSentiment(response.DocumentSentiment);
        }

        private static void WriteSentiment(Sentiment sentiment)
        {
            Console.WriteLine($"Polarity: {sentiment.Polarity}");
            Console.WriteLine($"Magnitude: {sentiment.Magnitude}");
        }

        private static void AnalyzeSyntax(string text, string encoding = "UTF16")
        {
            var client = LanguageServiceClient.Create();
            var response = client.AnnotateText(new Document()
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            },
            new Features() { ExtractSyntax = true });
            WriteSentences(response.Sentences);
        }

        private static void WriteSentences(IEnumerable<Sentence> sentences)
        {
            Console.WriteLine("Sentences:");
            foreach (var sentence in sentences)
            {
                Console.WriteLine($"\t{sentence.Text.BeginOffset}: {sentence.Text.Content}");
            }
        }

        private static void AnalyzeEverything(string text, string encoding = "UTF16")
        {
            var client = LanguageServiceClient.Create();
            var response = client.AnnotateText(new Document()
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            },
            new Features()
            {
                ExtractSyntax = true,
                ExtractDocumentSentiment = true,
                ExtractEntities = true,
            });
            Console.WriteLine($"Language: {response.Language}");
            WriteSentiment(response.DocumentSentiment);
            WriteSentences(response.Sentences);
            WriteEntities(response.Entities);
        }

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Write(Usage);
                return;
            }
            string command = args[0].ToLower();
            string text = string.Join(" ",
                new ArraySegment<string>(args, 1, args.Length - 1));
            switch (command)
            {
                case "entities":
                    AnalyzeEntities(text);
                    break;

                case "syntax":
                    AnalyzeSyntax(text);
                    break;

                case "sentiment":
                    AnalyzeSentiment(text);
                    break;

                case "everything":
                    AnalyzeEverything(text);
                    break;

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}