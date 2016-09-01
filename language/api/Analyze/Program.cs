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

using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudNaturalLanguageAPI.v1beta1;
using Google.Apis.CloudNaturalLanguageAPI.v1beta1.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;

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

        public static CloudNaturalLanguageAPIService
            CreateNaturalLanguageAPIClient()
        {
            var credentials =
                GoogleCredential.GetApplicationDefaultAsync().Result;
            if (credentials.IsCreateScopedRequired)
            {
                credentials = credentials.CreateScoped(new[]
                {
                    CloudNaturalLanguageAPIService.Scope.CloudPlatform
                });
            }
            var serviceInitializer = new BaseClientService.Initializer()
            {
                ApplicationName = "NL Sample",
                HttpClientInitializer = credentials
            };
            return new CloudNaturalLanguageAPIService(serviceInitializer);
        }

        private static void AnalyzeEntities(string text, string encoding = "UTF16")
        {
            var service = CreateNaturalLanguageAPIClient();
            var response = service.Documents.AnalyzeEntities(
                new AnalyzeEntitiesRequest()
                {
                    Document = new Document()
                    {
                        Content = text,
                        Type = "PLAIN_TEXT"
                    },
                    EncodingType = encoding
                }).Execute();
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
            var service = CreateNaturalLanguageAPIClient();
            var response = service.Documents.AnalyzeSentiment(
                new AnalyzeSentimentRequest()
                {
                    Document = new Document()
                    {
                        Content = text,
                        Type = "PLAIN_TEXT"
                    },
                }).Execute();
            WriteSentiment(response.DocumentSentiment);
        }

        private static void WriteSentiment(Sentiment sentiment)
        {
            Console.WriteLine($"Polarity: {sentiment.Polarity}");
            Console.WriteLine($"Magnitude: {sentiment.Magnitude}");
        }

        private static void AnalyzeSyntax(string text, string encoding = "UTF16")
        {
            var service = CreateNaturalLanguageAPIClient();
            var response = service.Documents.AnnotateText(
                new AnnotateTextRequest()
                {
                    Document = new Document()
                    {
                        Content = text,
                        Type = "PLAIN_TEXT"
                    },
                    EncodingType = encoding,
                    Features = new Features()
                    {
                        ExtractSyntax = true
                    }
                }).Execute();
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
            var service = CreateNaturalLanguageAPIClient();
            var response = service.Documents.AnnotateText(
                new AnnotateTextRequest()
                {
                    Document = new Document()
                    {
                        Content = text,
                        Type = "PLAIN_TEXT"
                    },
                    EncodingType = encoding,
                    Features = new Features()
                    {
                        ExtractSyntax = true,
                        ExtractDocumentSentiment = true,
                        ExtractEntities = true,
                    }
                }).Execute();
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