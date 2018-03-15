// Copyright 2018 Google Inc.
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
using Google.Cloud.Dlp.V2;

namespace GoogleCloudSamples
{
    abstract class InspectLocalOptions
    {
        [Value(0, HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }

        [Option('i', "info-types", HelpText = "Comma-separated infoTypes of information to match.",
            Default = "PHONE_NUMBER,EMAIL_ADDRESS,CREDIT_CARD_NUMBER,US_SOCIAL_SECURITY_NUMBER")]
        public string InfoTypes { get; set; }

        [Option('l', "minimum-likelihood",
            HelpText = "The minimum likelihood required before returning a match (0-5).", Default = 0)]
        public int MinLikelihood { get; set; }

        [Option('m', "max-findings",
            HelpText = "The maximum number of findings to report per request (0 = server maximum).", Default = 0)]
        public int MaxFindings { get; set; }

        [Option("no-quotes", HelpText = "Do not include matching quotes.")]
        public bool NoIncludeQuote { get; set; }
    }

    [Verb("inspectString", HelpText = "Inspects a content string.")]
    class InspectStringOptions : InspectLocalOptions
    {
        [Value(1, HelpText = "The item to inspect.", Required = true)]
        public string Value { get; set; }
    }

    [Verb("inspectFile", HelpText = "Inspects a content file.")]
    class InspectFileOptions : InspectLocalOptions
    {
        [Value(1, HelpText = "The path to the local file to inspect. Can be a text, JPG, or PNG.", Required = true)]
        public string File { get; set; }
    }

    [Verb("createInspectTemplate", HelpText = "Creates a template for inspecting operations.")]
    class CreateTemplateOptions : InspectLocalOptions
    {
        [Value(1, HelpText = "Display name of the template.", Required = true)]
        public string DisplayName { get; set; }

        [Value(2, HelpText = "Description of the template.", Required = true)]
        public string Description { get; set; }
    }

    [Verb("listTemplates", HelpText = "Lists all created inspection templates.")]
    class ListTemplatesOptions
    {
        [Value(0, HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }
    }

    [Verb("deleteTemplate", HelpText = "Deletes given template by name.")]
    class DeleteTemplatesOptions
    {
        [Value(0, HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }

        [Value(1, HelpText = "The name of the template to delete.", Required = true)]
        public string TemplateName { get; set; }
    }

    abstract class DeidOptions
    {
        [Value(0, HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }

        [Value(1, HelpText = "The string to deidentify.", Required = true)]
        public string Value { get; set; }
    }

    [Verb("deidMask", HelpText = "DeIdentify content via an input mask.")]
    class DeidMaskOptions : DeidOptions
    {
        [Option('i', "info-types", HelpText = "Comma-separated infoTypes of information to match.",
            Default = "PHONE_NUMBER,EMAIL_ADDRESS,CREDIT_CARD_NUMBER,US_SOCIAL_SECURITY_NUMBER")]
        public string InfoTypes { get; set; }

        [Option('m', "masking-character", HelpText = "Character to mask over deidentified content.", Default = "x")]
        public string Mask { get; set; }

        [Option('n', "num-to-mask", HelpText = "Number of sequential characters to mask in deidentified content.")]
        public int Num { get; set; }

        [Option('r', "reverse", HelpText = "Mask content from the end of string.")]
        public bool Reverse { get; set; }
    }

    class FpeOptions : DeidOptions
    {
        [Value(2, HelpText = "CryptoKey resource name to use in format projects/PROJECT_ID/locations/LOCATION/keyRings" +
            "/KEYRING/cryptoKeys/KEY_NAME", Required = true)]
        public string KeyName { get; set; }

        [Value(3, HelpText = "File path of key encrypted using the Cloud KMS key specified.", Required = true)]
        public string WrappedKeyFile { get; set; }

        [Option('a', "alphabet", HelpText = "Alphabet type to use for identification. Valid options are: " +
            "an (ALPHANUMERIC), hex (HEXADECIMAL), num (NUMERIC), and an-uc (ALPHANUMERIC_UPPERCASE).")]
        public string Alphabet { get; set; }
    }

    [Verb("deidFpe", HelpText = "DeIdentify content via a Cloud KMS encryption key.")]
    class DeidFpeOptions : FpeOptions { }

    [Verb("reidFpe", HelpText = "ReIdentify content removed by a previous call to deidFpe.")]
    class ReidFpeOptions : FpeOptions { }

    public class Dlp
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                InspectStringOptions,
                InspectFileOptions,
                CreateTemplateOptions,
                ListTemplatesOptions,
                DeleteTemplatesOptions,
                DeidMaskOptions,
                DeidFpeOptions,
                ReidFpeOptions>(args)
                .MapResult(
                (InspectStringOptions opts) => InspectLocal.InspectString(
                    opts.ProjectId,
                    opts.Value,
                    opts.MinLikelihood,
                    opts.MaxFindings,
                    !opts.NoIncludeQuote,
                    opts.InfoTypes),
                (InspectFileOptions opts) => InspectLocal.InspectFile(
                    opts.ProjectId,
                    opts.File,
                    opts.MinLikelihood,
                    opts.MaxFindings,
                    !opts.NoIncludeQuote,
                    opts.InfoTypes),
                (CreateTemplateOptions opts) => InspectTemplates.CreateInspectTemplate(
                    opts.ProjectId,
                    opts.DisplayName,
                    opts.Description,
                    opts.MinLikelihood,
                    opts.MaxFindings,
                    !opts.NoIncludeQuote,
                    opts.InfoTypes),
                (ListTemplatesOptions opts) => InspectTemplates.ListInspectTemplate(opts.ProjectId),
                (DeleteTemplatesOptions opts) => InspectTemplates.DeleteInspectTemplate(opts.ProjectId, opts.TemplateName),
                (DeidMaskOptions opts) => DeIdentify.DeidMask(
                    opts.ProjectId,
                    opts.Value,
                    opts.InfoTypes,
                    opts.Mask,
                    opts.Num,
                    opts.Reverse),
                (DeidFpeOptions opts) => DeIdentify.DeidFpe(
                    opts.ProjectId,
                    opts.Value,
                    opts.KeyName,
                    opts.WrappedKeyFile,
                    opts.Alphabet),
                (ReidFpeOptions opts) => DeIdentify.ReidFpe(
                    opts.ProjectId,
                    opts.Value,
                    opts.KeyName,
                    opts.WrappedKeyFile,
                    opts.Alphabet),
                errs => 1);
        }
    }
}
