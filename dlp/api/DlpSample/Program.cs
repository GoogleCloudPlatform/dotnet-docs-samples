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

using System;
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

        [Option('c', "custom-dictionary", HelpText = "Comma-separated dictionary phrases to match.", Default = "")]
        public string CustomDictionary { get; set; }

        [Option('r', "custom-regexes", HelpText = "Comma-separated regexes to match.", Default = "")]
        public string CustomRegexes { get; set; }

        [Option('l', "minimum-likelihood",
            HelpText = "The minimum likelihood required before returning a match (0-5).", Default = "Unlikely")]
        public string MinLikelihood { get; set; }

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

    [Verb("inspectBigQuery", HelpText = "Inspects a content in BigQuery")]
    class InspectBigQueryOptions : InspectLocalOptions
    {
        [Option("identifyingFields", HelpText = "Identifying fields.")]
        public string IdentifyingFields { get; set; }

        [Option("databaseId", HelpText = "BigQuery Database Id")]
        public string DatasetId { get; set; }

        [Option("tableId", HelpText = "BigQuery Table Id")]
        public string TableId { get; set; }
    }

    [Verb("inspectDataStore", HelpText = "Inspects a content in Datastore")]
    class InspectDatastoreOptions : InspectLocalOptions
    {
        [Option("kindName", HelpText = "Datastore Kind name")]
        public string KindName { get; set; }

        [Option("namespaceId", HelpText = "Datastore Namespace Id")]
        public string NamespaceId { get; set; }

        [Option("datasetId", HelpText = "Datastore Dataset Id")]
        public string DatasetId { get; set; }

        [Option("tableId", HelpText = "Datastore Table Id")]
        public string TableId { get; set; }
    }

    [Verb("inspectGcs", HelpText = "Inspects a content in GCS")]
    class InspectGcsOptions : InspectLocalOptions
    {
        [Option("bucketName", HelpText = "Storage bucket name")]
        public string BucketName { get; set; }

        [Option("topicId", HelpText = "Pub/Sub topic Id")]
        public string TopicId { get; set; }

        [Option("subscriptionId", HelpText = "Pub/Sub Subscription Id")]
        public string SubscriptionId { get; set; }
    }

    [Verb("createInspectTemplate", HelpText = "Creates a template for inspecting operations.")]
    class CreateTemplateOptions : InspectLocalOptions
    {
        [Value(1, HelpText = "The name of the template to be created.", Required = true)]
        public string TemplateId { get; set; }

        [Value(2, HelpText = "Display name of the template.", Required = true)]
        public string DisplayName { get; set; }

        [Value(3, HelpText = "Description of the template.", Required = true)]
        public string Description { get; set; }
    }

    [Verb("redactImage", HelpText = "Redacts an image and saves the outcome into provided file path.")]
    class RedactFromImageOptions
    {
        [Option("projectId", HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }

        [Option("imageFromPath", HelpText = "The path of the image to redact.", Required = true)]
        public string ImageFromPath { get; set; }

        [Option("imageToPath", HelpText = "The path of the redacted image.", Required = true)]
        public string ImageToPath { get; set; }
    }

    [Verb("listTemplates", HelpText = "Lists all created inspection templates.")]
    class ListTemplatesOptions
    {
        [Value(1, HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }
    }

    [Verb("deleteTemplate", HelpText = "Deletes given template by name.")]
    class DeleteTemplatesOptions
    {
        [Value(1, HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }

        [Value(2, HelpText = "The name of the template to delete.", Required = true)]
        public string TemplateName { get; set; }
    }

    abstract class DeidOptions
    {
        [Value(0, HelpText = "The project ID to run dthe API call under.", Required = true)]
        public string ProjectId { get; set; }

        [Value(1, HelpText = "The string to deidentify.", Required = true)]
        public string Value { get; set; }
    }

    [Verb("deidDateShift", HelpText = "Deidentify dates in a CSV file by pseudorandomly shifting them.")]
    class DeidDateShiftOptions
    {
        [Value(0, HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }

        [Value(1, HelpText = "The path to the CSV file to deidentify.", Required = true)]
        public string InputCsvFile { get; set; }

        [Value(2, HelpText = "The path to save the date-shifted CSV file to.", Required = true)]
        public string OutputCsvFile { get; set; }

        [Value(3, HelpText = "The maximum number of days to shift a date backward.", Required = true)]
        public int LowerBoundDays { get; set; }

        [Value(4, HelpText = "The maximum number of days to shift a date forward.", Required = true)]
        public int UpperBoundDays { get; set; }

        [Value(5, HelpText = "The list of (date) fields in the CSV file to date shift.", Required = true)]
        public string DateFields { get; set; }

        [Value(6, HelpText = "The column to determine date shift amount based on.", Default = "")]
        public string ContextFieldId { get; set; }

        [Value(7, HelpText = "The name of the Cloud KMS key used to encrypt('wrap') the AES-256 key.", Default = "")]
        public string WrappedKey { get; set; }

        [Value(8, HelpText = "The encrypted('wrapped') AES-256 key to use when shifting dates.", Default = "")]
        public string KeyName { get; set; }
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

    abstract class FpeOptions : DeidOptions
    {
        [Value(2, HelpText = "CryptoKey resource name to use in format projects/PROJECT_ID/locations/LOCATION/keyRings" +
            "/KEYRING/cryptoKeys/KEY_NAME", Required = true)]
        public string KeyName { get; set; }

        [Value(3, HelpText = "File path of key encrypted using the Cloud KMS key specified.", Required = true)]
        public string WrappedKeyFile { get; set; }

        [Value(4, HelpText = "Alphabet type to use for identification. Valid options are: " +
            "an (ALPHANUMERIC), hex (HEXADECIMAL), num (NUMERIC), and an-uc (ALPHANUMERIC_UPPERCASE).", Required = true)]
        public string Alphabet { get; set; }
    }

    [Verb("deidFpe", HelpText = "DeIdentify content via a Cloud KMS encryption key.")]
    class DeidFpeOptions : FpeOptions
    {
        [Option('i', "info-types", HelpText = "Comma-separated infoTypes of information to match.",
            Default = "PHONE_NUMBER,EMAIL_ADDRESS,CREDIT_CARD_NUMBER,US_SOCIAL_SECURITY_NUMBER")]
        public string InfoTypes { get; set; }
    }

    [Verb("reidFpe", HelpText = "ReIdentify content removed by a previous call to deidFpe.")]
    class ReidFpeOptions : FpeOptions { }

    [Verb("listJobs", HelpText = "List Data Loss Prevention API jobs corresponding to a given filter.")]
    class ListJobsOptions
    {
        [Value(0, HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }

        [Value(1, HelpText = "The filter expression to use. For more information and filter syntax, see https://cloud.google.com/dlp/docs/reference/rest/v2/projects.dlpJobs/list", Required = true)]
        public string Filter { get; set; }

        [Value(2, HelpText = "The type of job to list. (either 'InspectJob' or 'RiskAnalysisJob')", Default = "InspectJob")]
        public string JobType { get; set; }
    }

    [Verb("deleteJob", HelpText = "Delete results of a Data Loss Prevention API job.")]
    class DeleteJobOptions
    {
        [Value(0, HelpText = "The full name of the job whose results should be deleted.", Required = true)]
        public string JobName { get; set; }
    }

    [Verb("createJobTrigger", HelpText = "Create a Data Loss Prevention API job trigger.")]
    class CreateJobTriggerOptions : InspectLocalOptions
    {
        [Value(1, HelpText = "The name of the bucket to scan.", Required = true)]
        public string BucketName { get; set; }

        [Value(2, HelpText = "How often to wait between scans, in days. (minimum = 1 day)", Required = true)]
        public int ScanPeriod { get; set; }

        [Option("autoPopulateTimespan", HelpText = "Limit scan to new content only.")]
        public bool AutoPopulateTimespan { get; set; }

        [Option('t', "triggerId", HelpText = "The name of the trigger to be created.", Default = "")]
        public string TriggerId { get; set; }

        [Option('n', "displayName", HelpText = "A display name for the trigger to be created.", Default = "")]
        public string DisplayName { get; set; }

        [Option('d', "description", HelpText = "A description for the trigger to be created.", Default = "")]
        public string Description { get; set; }
    }

    abstract class RiskAnalysisOptions
    {
        [Value(0, HelpText = "The project ID to run the API call under.", Required = true)]
        public string CallingProjectId { get; set; }

        [Value(1, HelpText = "The project ID the table is stored under.", Required = true)]
        public string TableProjectId { get; set; }

        [Value(2, HelpText = "The ID of the dataset to inspect. (e.g. 'my_dataset')", Required = true)]
        public string DatasetId { get; set; }

        [Value(3, HelpText = "The ID of the table to inspect. (e.g. 'my_table')", Required = true)]
        public string TableId { get; set; }

        [Value(4, HelpText = "The name of the Pub/Sub topic to notify once the job completes.", Default = 0)]
        public string TopicId { get; set; }

        [Value(5, HelpText = "The name of the Pub/Sub subscription to use when listening for job completion notifications.", Default = 0)]
        public string SubscriptionId { get; set; }
    }

    abstract class StatsOptions : RiskAnalysisOptions
    {
        [Value(6, HelpText = "The name of the column to compute risk metrics for. (e.g. 'age')", Default = 0)]
        public string ColumnName { get; set; }
    }

    abstract class QuasiIdOptions : RiskAnalysisOptions
    {
        [Value(6, HelpText = "A set of columns that form a composite key, delimited by commas. (e.g. 'name,city')", Required = true)]
        public string QuasiIdColumns { get; set; }
    }

    [Verb("numericalStats", HelpText = "Computes risk metrics of a column of numbers in a Google BigQuery table.")]
    class NumericalStatsOptions : StatsOptions { }

    [Verb("categoricalStats", HelpText = "Computes risk metrics of a column of data in a Google BigQuery table.")]
    class CategoricalStatsOptions : StatsOptions { }

    [Verb("kAnonymity", HelpText = "Computes the k-anonymity of a column set in a Google BigQuery table.")]
    class KAnonymityOptions : QuasiIdOptions { }

    [Verb("lDiversity", HelpText = "Computes the k-anonymity of a column set in a Google BigQuery table.")]
    class LDiversityOptions : QuasiIdOptions
    {
        [Value(7, HelpText = "The column to measure l-diversity relative to. (e.g. 'age')", Required = true)]
        public string SensitiveAttribute { get; set; }
    }

    [Verb("kMap", HelpText = "Computes the k-map risk estimation of a column set in a Google BigQuery table.")]
    class KMapOptions : QuasiIdOptions
    {
        [Value(7, HelpText = "A list of the infoTypes for each quasi-id, delimited by commas.", Required = true)]
        public string InfoTypes { get; set; }

        [Value(8, HelpText = "The ISO 3166-1 region code that the data is representative of.", Default = "")]
        public string RegionCode { get; set; }
    }

    [Verb("listInfoTypes", HelpText = "List the types of sensitive information the DLP API supports.")]
    class ListInfoTypesOptions
    {
        [Value(0, HelpText = "The BCP-47 language code to use. (e.g. 'en-US')", Default = "en-US")]
        public string LanguageCode { get; set; }

        [Option('f', "filter", HelpText = "The filter to use.", Default = "")]
        public string Filter { get; set; }
    }

    public class Dlp
    {
        public static void Main(string[] args)
        {
            // Command line argument parser we use doesn't support more than 15 options, therefore
            // we split it
            if (args.Length == 0)
            {
                Console.WriteLine("Invalid number of arguments supplied");
                Environment.Exit(-1);
            }

            switch (args[0])
            {
                case "redactImage":
                    Parser.Default.ParseArguments<RedactFromImageOptions>(args).MapResult(
                        (RedactFromImageOptions options) => RedactSamples.RedactFromImage(
                            options.ProjectId,
                            options.ImageFromPath,
                            options.ImageToPath),
                        errs => 1);
                    break;
                case "kAnonymity":
                case "lDiversity":
                case "deidDateShift":
                case "kMap":
                case "listInfoTypes":
                case "inspectBigQuery":
                case "inspectDataStore":
                case "inspectGcs":
                    Parser.Default.ParseArguments<
                    KAnonymityOptions,
                    LDiversityOptions,
                    DeidDateShiftOptions,
                    KMapOptions,
                    ListInfoTypesOptions,
                    InspectBigQueryOptions,
                    InspectDatastoreOptions,
                    InspectGcsOptions>(args).MapResult(
                    (KAnonymityOptions opts) => RiskAnalysis.KAnonymity(
                        opts.CallingProjectId,
                        opts.TableProjectId,
                        opts.DatasetId,
                        opts.TableId,
                        opts.TopicId,
                        opts.SubscriptionId,
                        DlpSamplesUtils.ParseQuasiIds(opts.QuasiIdColumns)),
                    (LDiversityOptions opts) => RiskAnalysis.LDiversity(
                        opts.CallingProjectId,
                        opts.TableProjectId,
                        opts.DatasetId,
                        opts.TableId,
                        opts.TopicId,
                        opts.SubscriptionId,
                        DlpSamplesUtils.ParseQuasiIds(opts.QuasiIdColumns),
                        opts.SensitiveAttribute),
                    (KMapOptions opts) => RiskAnalysis.KMap(
                        opts.CallingProjectId,
                        opts.TableProjectId,
                        opts.DatasetId,
                        opts.TableId,
                        opts.TopicId,
                        opts.SubscriptionId,
                        DlpSamplesUtils.ParseQuasiIds(opts.QuasiIdColumns),
                        DlpSamplesUtils.ParseInfoTypes(opts.InfoTypes),
                        opts.RegionCode),
                    (DeidDateShiftOptions opts) => DeIdentify.DeidDateShift(
                        opts.ProjectId,
                        opts.InputCsvFile,
                        opts.OutputCsvFile,
                        opts.LowerBoundDays,
                        opts.UpperBoundDays,
                        opts.DateFields,
                        opts.ContextFieldId,
                        opts.KeyName,
                        opts.WrappedKey),
                    (ListInfoTypesOptions opts) => Metadata.ListInfoTypes(
                        opts.LanguageCode,
                        opts.Filter),
                    (InspectBigQueryOptions opts) => InspectSamples.InspectBigQuery(
                        opts.ProjectId,
                        opts.MinLikelihood,
                        opts.MaxFindings,
                        !opts.NoIncludeQuote,
                        DlpSamplesUtils.ParseIdentifyingFields(opts.IdentifyingFields),
                        DlpSamplesUtils.ParseInfoTypes(opts.InfoTypes),
                        DlpSamplesUtils.ParseCustomInfoTypes(opts.CustomDictionary, opts.CustomRegexes),
                        opts.DatasetId,
                        opts.TableId),
                    (InspectDatastoreOptions opts) => InspectSamples.InspectCloudDataStore(
                        opts.ProjectId,
                        opts.MinLikelihood,
                        opts.MaxFindings,
                        !opts.NoIncludeQuote,
                        opts.KindName,
                        opts.NamespaceId,
                        DlpSamplesUtils.ParseInfoTypes(opts.InfoTypes),
                        DlpSamplesUtils.ParseCustomInfoTypes(opts.CustomDictionary, opts.CustomRegexes),
                        opts.DatasetId,
                        opts.TableId),
                    (InspectGcsOptions opts) => InspectSamples.InspectGCS(
                        opts.ProjectId,
                        opts.MinLikelihood,
                        opts.MaxFindings,
                        !opts.NoIncludeQuote,
                        DlpSamplesUtils.ParseInfoTypes(opts.InfoTypes),
                        DlpSamplesUtils.ParseCustomInfoTypes(opts.CustomDictionary, opts.CustomRegexes),
                        opts.BucketName,
                        opts.TopicId,
                        opts.SubscriptionId),
                    errs => 1);
                    break;
                default:
                    Parser.Default.ParseArguments<
                        InspectStringOptions,
                        InspectFileOptions,
                        CreateTemplateOptions,
                        ListTemplatesOptions,
                        DeleteTemplatesOptions,
                        DeidMaskOptions,
                        DeidFpeOptions,
                        ReidFpeOptions,
                        ListJobsOptions,
                        DeleteJobOptions,
                        CreateJobTriggerOptions,
                        ListJobTriggersOptions,
                        DeleteJobTriggerOptions,
                        NumericalStatsOptions,
                        CategoricalStatsOptions>(args)
                        .MapResult(
                        (InspectStringOptions opts) => InspectSamples.InspectString(
                            opts.ProjectId,
                            opts.Value,
                            opts.MinLikelihood,
                            opts.MaxFindings,
                            !opts.NoIncludeQuote,
                            DlpSamplesUtils.ParseInfoTypes(opts.InfoTypes),
                            DlpSamplesUtils.ParseCustomInfoTypes(opts.CustomDictionary, opts.CustomRegexes)),
                        (InspectFileOptions opts) => InspectSamples.InspectFile(
                            opts.ProjectId,
                            opts.File,
                            opts.MinLikelihood,
                            opts.MaxFindings,
                            !opts.NoIncludeQuote,
                            DlpSamplesUtils.ParseInfoTypes(opts.InfoTypes),
                            DlpSamplesUtils.ParseCustomInfoTypes(opts.CustomDictionary, opts.CustomRegexes)),
                        (CreateTemplateOptions opts) => InspectTemplates.CreateInspectTemplate(
                            opts.ProjectId,
                            opts.TemplateId,
                            opts.DisplayName,
                            opts.Description,
                            opts.MinLikelihood,
                            opts.MaxFindings,
                            !opts.NoIncludeQuote),
                        (ListTemplatesOptions opts) => InspectTemplates.ListInspectTemplate(opts.ProjectId),
                        (DeleteTemplatesOptions opts) => InspectTemplates.DeleteInspectTemplate(opts.ProjectId, opts.TemplateName),
                        (DeidMaskOptions opts) => DeIdentify.DeidMask(
                            opts.ProjectId,
                            opts.Value,
                            DlpSamplesUtils.ParseInfoTypes(opts.InfoTypes),
                            opts.Mask,
                            opts.Num,
                            opts.Reverse),
                        (DeidFpeOptions opts) => DeIdentify.DeidFpe(
                            opts.ProjectId,
                            opts.Value,
                            DlpSamplesUtils.ParseInfoTypes(opts.InfoTypes),
                            opts.KeyName,
                            opts.WrappedKeyFile,
                            opts.Alphabet),
                        (ReidFpeOptions opts) => DeIdentify.ReidFpe(
                            opts.ProjectId,
                            opts.Value,
                            opts.KeyName,
                            opts.WrappedKeyFile,
                            opts.Alphabet),
                        (ListJobsOptions opts) => Jobs.ListJobs(
                            opts.ProjectId,
                            opts.Filter,
                            opts.JobType),
                        (DeleteJobOptions opts) => Jobs.DeleteJob(opts.JobName),
                        (CreateJobTriggerOptions opts) => JobTriggers.CreateJobTrigger(
                            opts.ProjectId,
                            opts.BucketName,
                            opts.MinLikelihood,
                            opts.MaxFindings,
                            opts.AutoPopulateTimespan,
                            opts.ScanPeriod,
                            DlpSamplesUtils.ParseInfoTypes(opts.InfoTypes),
                            opts.TriggerId,
                            opts.DisplayName,
                            opts.Description
                        ),
                        (ListJobTriggersOptions opts) => JobTriggers.ListJobTriggers(
                            opts.ProjectId
                        ),
                        (DeleteJobTriggerOptions opts) => JobTriggers.DeleteJobTrigger(
                            opts.TriggerName
                        ),
                        (NumericalStatsOptions opts) => RiskAnalysis.NumericalStats(
                            opts.CallingProjectId,
                            opts.TableProjectId,
                            opts.DatasetId,
                            opts.TableId,
                            opts.TopicId,
                            opts.SubscriptionId,
                            opts.ColumnName
                        ),
                        (CategoricalStatsOptions opts) => RiskAnalysis.CategoricalStats(
                            opts.CallingProjectId,
                            opts.TableProjectId,
                            opts.DatasetId,
                            opts.TableId,
                            opts.TopicId,
                            opts.SubscriptionId,
                            opts.ColumnName
                        ),
                        errs => 1);
                    break;
            }
        }
    }
}
