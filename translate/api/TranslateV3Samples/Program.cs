// Copyright 2019 Google Inc.
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

using CommandLine;
using System;

namespace GoogleCloudSamples
{
    abstract internal class TranslateTextBaseOptions
    {
        [Option("text", Default = "Hello, world!")]
        public string Text { get; set; }

        [Option("target_language", Default = "fr")]
        public string TargetLanguage { get; set; }

        [Option("project_id", Default = "[Google Cloud Project ID]")]
        public string ProjectId { get; set; }
    }

    [Verb("batchTranslateText", HelpText = "Batch translate text with given URI.")]
    internal class BatchTranslateTextOptions
    {
        [Option("input_uri", Default = "gs://cloud-samples-data/text.txt")]
        public string InputUri { get; set; }

        [Option("output_uri", Default = "gs://YOUR_BUCKET_ID/path_to_store_results/")]
        public string OutputUri { get; set; }

        [Option("project_id", Default = "[Google Cloud Project ID]")]
        public string ProjectId { get; set; }

        [Option("location", Default = "us-central1")]
        public string Location { get; set; }

        [Option("source_language", Default = "en")]
        public string SourceLanguage { get; set; }

        [Option("target_language", Default = "ja")]
        public string TargetLanguage { get; set; }
    }

    [Verb("batchTranslateTextWithGlossaryAndModel", HelpText = "")]
    internal class BatchTranslateTextWithGlossaryAndModelOptions
    {
        [Option("input_uri", Default = "gs://cloud-samples-data/text.txt")]
        public string InputUri { get; set; }

        [Option("output_uri", Default = "gs://YOUR_BUCKET_ID/path_to_store_results/")]
        public string OutputUri { get; set; }

        [Option("project_id", Default = "[Google Cloud Project ID]")]
        public string ProjectId { get; set; }

        [Option("location", Default = "us-central1")]
        public string Location { get; set; }

        [Option("target_language", Default = "en")]
        public string TargetLanguage { get; set; }

        [Option("source_language", Default = "de")]
        public string SourceLanguage { get; set; }

        [Option("model_id", Default = "[your-model-id]")]
        public string ModelId { get; set; }

        [Option("glossary_id", Default = "[YOUR_GLOSSARY_ID]")]
        public string GlossaryId { get; set; }
    }

    [Verb("batchTranslateTextWithGlossary", HelpText = "")]
    internal class BatchTranslateTextWithGlossaryOptions
    {
        [Option("input_uri", Default = "gs://cloud-samples-data/text.txt")]
        public string InputUri { get; set; }

        [Option("output_uri", Default = "gs://YOUR_BUCKET_ID/path_to_store_results/")]
        public string OutputUri { get; set; }

        [Option("project_id", Default = "[Google Cloud Project ID]")]
        public string ProjectId { get; set; }

        [Option("location", Default = "us-central1")]
        public string Location { get; set; }

        [Option("glossary_id", Default = "[YOUR_GLOSSARY_ID]")]
        public string GlossaryId { get; set; }

        [Option("target_language", Default = "en")]
        public string TargetLanguage { get; set; }

        [Option("source_language", Default = "de")]
        public string SourceLanguage { get; set; }
    }

    [Verb("batchTranslateTextWithModel", HelpText = "")]
    internal class BatchTranslateTextWithModelOptions
    {
        [Option("input_uri", Default = "gs://cloud-samples-data/text.txt")]
        public string InputUri { get; set; }

        [Option("output_uri", Default = "gs://YOUR_BUCKET_ID/path_to_store_results/")]
        public string OutputUri { get; set; }

        [Option("project_id", Default = "[Google Cloud Project ID]")]
        public string ProjectId { get; set; }

        [Option("location", Default = "us-central1")]
        public string Location { get; set; }

        [Option("target_language", Default = "en")]
        public string TargetLanguage { get; set; }

        [Option("source_language", Default = "de")]
        public string SourceLanguage { get; set; }

        [Option("model_id", Default = "{your-model-id}")]
        public string ModelId { get; set; }
    }

    [Verb("detectLanguage", HelpText = "")]
    internal class DetectLanguageOptions
    {
        [Option("text", Default = "Hello, world!")]
        public string Text { get; set; }

        [Option("project_id", Default = "[Google Cloud Project ID]")]
        public string ProjectId { get; set; }
    }

    [Verb("getGlossary", HelpText = "")]
    internal class GetGlossaryOptions
    {
        [Option("project_id", Default = "[Google Cloud Project ID]")]
        public string ProjectId { get; set; }

        [Option("glossary_id", Default = "[Glossary ID]")]
        public string GlossaryId { get; set; }
    }

    [Verb("getSupportedLanguages", HelpText = "")]
    internal class GetSupportedLanguagesOptions
    {
        [Option("project_id", Default = "[Google Cloud Project ID]")]
        public string ProjectId { get; set; }
    }
    [Verb("getSupportedLanguagesWithTarget", HelpText = "")]
    internal class GetSupportedLanguagesWithTargetOptions
    {
        [Option("language_code", Default = "en")]
        public string LanguageCode { get; set; }

        [Option("project_id", Default = "[Google Cloud Project ID]")]
        public string ProjectId { get; set; }
    }

    [Verb("listGlossaries", HelpText = "")]
    internal class ListGlossaryOptions
    {
        [Option("project_id", Default = "[Google Cloud Project ID]")]
        public string ProjectId { get; set; }
    }

    [Verb("translateText", HelpText = "")]
    internal class TranslateTextOptions : TranslateTextBaseOptions { }

    [Verb("translateTextWithGlossary", HelpText = "")]
    internal class TranslateTextWithGlossaryOptions : TranslateTextBaseOptions
    {
        [Option("source_language", Default = "en")]
        public string SourceLanguage { get; set; }

        [Option("glossary_id", Default = "[YOUR_GLOSSARY_ID]")]
        public string GlossaryId { get; set; }
    }

    [Verb("translateTextWithModel", HelpText = "")]
    internal class TranslateTextWithModelOptions : TranslateTextBaseOptions
    {
        [Option("model_id", Default = "[MODEL ID]")]
        public string ModelId { get; set; }

        [Option("source_language", Default = "en")]
        public string SourceLanguage { get; set; }

        [Option("location", Default = "global")]
        public string Location { get; set; }
    }

    [Verb("translateTextWithGlossaryAndModel", HelpText = "")]
    internal class TranslateTextWithGlossaryAndModelOptions : TranslateTextBaseOptions
    {
        [Option("model_id", Default = "[MODEL ID]")]
        public string ModelId { get; set; }

        [Option("source_language", Default = "en")]
        public string SourceLanguage { get; set; }

        [Option("location", Default = "global")]
        public string Location { get; set; }

        [Option("glossary_id", Default = "[YOUR_GLOSSARY_ID]")]
        public string GlossaryId { get; set; }
    }

    public class TranslateV3Samples
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Invalid number of arguments supplied");
                Environment.Exit(-1);
            }

            // If we ever have more than 15 verbs we should split this.
            switch (args[0])
            {
                case "batchTranslateText":
                case "batchTranslateTextWithGlossaryAndModel":
                case "batchTranslateTextWithGlossary":
                case "batchTranslateTextWithModel":
                case "detectLanguage":
                case "getGlossary":
                case "getSupportedLanguages":
                case "getSupportedLanguagesWithTarget":
                case "listGlossaries":
                case "translateText":
                case "translateTextWithModel":
                case "translateTextWithGlossary":
                case "translateTextWithGlossaryAndModel":
                    Parser.Default.ParseArguments<
                        BatchTranslateTextOptions,
                        BatchTranslateTextWithGlossaryAndModelOptions,
                        BatchTranslateTextWithGlossaryOptions,
                        BatchTranslateTextWithModelOptions,
                        DetectLanguageOptions,
                        GetGlossaryOptions,
                        GetSupportedLanguagesOptions,
                        GetSupportedLanguagesWithTargetOptions,
                        ListGlossaryOptions,
                        TranslateTextOptions,
                        TranslateTextWithGlossaryAndModelOptions,
                        TranslateTextWithGlossaryOptions,
                        TranslateTextWithModelOptions
                        >(args).MapResult(
                        (BatchTranslateTextOptions opts) =>
                           {
                               BatchTranslateText.BatchTranslateTextSample(
                                  opts.InputUri, opts.OutputUri, opts.ProjectId,
                                  opts.Location, opts.SourceLanguage, opts.TargetLanguage);
                               return 0;
                           },
                        (BatchTranslateTextWithGlossaryAndModelOptions opts) =>
                        {
                            BatchTranslateTextWithGlossaryAndModel.BatchTranslateTextWithGlossaryAndModelSample(
                                opts.InputUri, opts.OutputUri, opts.ProjectId, opts.Location,
                                opts.TargetLanguage, opts.SourceLanguage, opts.ModelId, opts.GlossaryId);
                            return 0;
                        },
                        (BatchTranslateTextWithGlossaryOptions opts) =>
                        {
                            BatchTranslateTextWithGlossary.BatchTranslateTextWithGlossarySample(
                                    opts.InputUri, opts.OutputUri, opts.ProjectId, opts.Location, opts.GlossaryId, opts.TargetLanguage, opts.SourceLanguage);
                            return 0;
                        },
                        (BatchTranslateTextWithModelOptions opts) =>
                        {
                            BatchTranslateTextWithModel.BatchTranslateTextWithModelSample(
                                opts.InputUri, opts.OutputUri, opts.ProjectId, opts.Location,
                                opts.TargetLanguage, opts.SourceLanguage, opts.ModelId);
                            return 0;
                        },
                        (DetectLanguageOptions opts) =>
                        {
                            DetectLanguage.DetectLanguageSample(opts.Text, opts.ProjectId);
                            return 0;
                        },
                        (GetGlossaryOptions opts) =>
                        {
                            GetGlossary.GetGlossarySample(opts.ProjectId, opts.GlossaryId);
                            return 0;
                        },
                        (GetSupportedLanguagesOptions opts) =>
                        {
                            GetSupportedLanguages.GetSupportedLanguagesSample(opts.ProjectId);
                            return 0;
                        },
                        (GetSupportedLanguagesWithTargetOptions opts) =>
                        {
                            GetSupportedLanguagesForTarget.GetSupportedLanguagesForTargetSample(opts.LanguageCode, opts.ProjectId);
                            return 0;
                        },
                        (ListGlossaryOptions opts) =>
                        {
                            ListGlossary.ListGlossariesSample(opts.ProjectId);
                            return 0;
                        },
                        (TranslateTextOptions opts) =>
                        {
                            TranslateText.TranslateTextSample(opts.Text, opts.TargetLanguage, opts.ProjectId);
                            return 0;
                        },
                        (TranslateTextWithModelOptions opts) =>
                        {
                            TranslateTextWithModel.TranslateTextWithModelSample(opts.ModelId, opts.Text, opts.TargetLanguage, opts.SourceLanguage, opts.ProjectId, opts.Location);
                            return 0;
                        },
                        (TranslateTextWithGlossaryOptions opts) =>
                        {
                            TranslateTextWithGlossary.TranslateTextWithGlossarySample(opts.Text, opts.SourceLanguage, opts.TargetLanguage, opts.ProjectId, opts.GlossaryId);
                            return 0;
                        },
                        (TranslateTextWithGlossaryAndModelOptions opts) =>
                        {
                            TranslateTextWithGlossaryAndModel.TranslateTextWithGlossaryAndModelSample(opts.ModelId, opts.GlossaryId,
                           opts.Text, opts.TargetLanguage, opts.SourceLanguage, opts.ProjectId, opts.Location);
                            return 0;
                        },
                        errs => 1);
                    break;
            }
        }
    }
}