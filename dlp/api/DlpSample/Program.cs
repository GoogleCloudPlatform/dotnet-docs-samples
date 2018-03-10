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
            Default = "PHONE_NUMBER,EMAIL_ADDRESS,CREDIT_CARD_NUMBER")]
        public string InfoTypes { get; set; }

        [Option('l', "minimum-likelihood",
            HelpText = "The minimum likelihood required before returning a match (0-5).", Default = 0)]
        public int MinLikelihood { get; set; }

        [Option('m', "max-findings",
            HelpText = "The maximum number of findings to report per request (0 = server maximum).", Default = 0)]
        public int MaxFindings { get; set; }

        [Option('n', "no-includeQuotes", HelpText = "Do not include matching quotes.")]
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

    [Verb("createInspectTemplate", HelpText = "Creates a template for inspecting operations")]
    class InspectTemplateOptions : InspectLocalOptions
    {
        [Value(1, HelpText = "Display name of the template", Required = true)]
        public string DisplayName { get; set; }

        [Value(2, HelpText = "Description of the template", Required = true)]
        public string Description { get; set; }
    }

    [Verb("listTemplates", HelpText = "Lists all created inspection templates")]
    class ListTemplatesOptions
    {
        [Value(0, HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }
    }

    [Verb("deleteTemplate", HelpText = "Deletes given template by name")]
    class DeleteTemplatesOptions
    {
        [Value(0, HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }

        [Value(1, HelpText = "The name of the template to delete", Required = true)]
        public string TemplateName { get; set; }
    }


    public class Dlp
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<InspectStringOptions, InspectFileOptions, InspectTemplateOptions, ListTemplatesOptions, DeleteTemplatesOptions>(args)
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
                (InspectTemplateOptions opts) => InspectTemplates.CreateInspectTemplate(
                    opts.ProjectId,
                    opts.DisplayName,
                    opts.Description,
                    opts.MinLikelihood,
                    opts.MaxFindings,
                    !opts.NoIncludeQuote,
                    opts.InfoTypes),
                (ListTemplatesOptions opts) => InspectTemplates.ListInspectTemplate(opts.ProjectId),
                (DeleteTemplatesOptions opts) => InspectTemplates.DeleteInspectTemplate(opts.ProjectId, opts.TemplateName),
                errs => 1);
        }
    }
}
