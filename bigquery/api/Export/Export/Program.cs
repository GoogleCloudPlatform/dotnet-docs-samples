// Copyright (c) 2016 Google Inc.
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
// See https://cloud.google.com/bigquery/exporting-data-from-bigquery

using CommandLine;
using CommandLine.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Services;
using System;

namespace GoogleCloudSamples
{
    public class Options
    {
        [Option('p', "ProjectId", Required = true,
          HelpText = "The id of your Google Cloud Project.")]
        public string ProjectId { get; set; }

        [Option('s', "DatasetId", DefaultValue = "test_dataset",
          HelpText = "The id of your Google Cloud Project.")]
        public string DatasetId { get; set; }

        [Option('t', "TableId", DefaultValue = "test_table",
            HelpText = "The id of the BigQuery table to export.")]
        public string TableId { get; set; }

        [Option('o', "CloudStoragePath", Required = true,
          HelpText = "Fully qualified path to a Google Cloud Storage Location. "
            + "Example: gs://mybucket/myfolder")]
        public string CloudStoragePath { get; set; }

        [Option('f', "ExportFormat", DefaultValue = "CSV",
            HelpText = "The output format.  One of CSV, NEWLINE_DELIMITED_JSON, or AVRO.")]
        public string ExportFormat { get; set; }

        [Option('c', "Compression", DefaultValue = "NONE",
            HelpText = "Format to compress result with.  One of NONE or GZIP")]
        public string Compression { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                var response = ExportTable(options);
                Console.WriteLine($"Job {response.JobReference.JobId} created with status { response.Status.State }.");
            }
            else
            {
                Console.WriteLine(options.GetUsage());
            }
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        // [START ExportTable]
        public static Job ExportTable(Options options)
        {
            var bigquery = CreateAuthorizedClient();
            var jobId = Guid.NewGuid().ToString();
            var request = new JobsResource.InsertRequest(bigquery, new Job
            {
                Configuration = new JobConfiguration
                {
                    Extract = new JobConfigurationExtract
                    {
                        SourceTable = new TableReference
                        {
                            ProjectId = options.ProjectId,
                            DatasetId = options.DatasetId,
                            TableId = options.TableId,
                        },
                        DestinationUris = new[] { options.CloudStoragePath },
                        DestinationFormat = options.ExportFormat,
                        Compression = options.Compression
                    }
                },
                JobReference = new JobReference
                {
                    ProjectId = options.ProjectId,
                    JobId = jobId,
                }
            }, options.ProjectId);
            return request.Execute();
        }
        // [END ExportTable]


        /// <summary>
        /// Creates an authorized Bigquery client service using Application
        /// Default Credentials.
        /// </summary>
        /// <returns>an authorized Bigquery client</returns>
        static public BigqueryService CreateAuthorizedClient()
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefaultAsync().Result;
            // Inject the Bigquery scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    BigqueryService.Scope.Bigquery
                });
            }
            return new BigqueryService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DotNet Bigquery Samples",
            });
        }
    }
}
