using System;
using System.Diagnostics;
using Google.Cloud.BigQuery.V2;

namespace GoogleCloudSamples
{
    public static class BiqQuerySnippets
    {
        /// <summary>
        /// Loads the table from a JSON source.
        /// </summary>
        /// <param name="client">BigQuery client</param>
        /// <param name="datasetId"></param>
        /// <param name="tableId"></param>
        public static void LoadTableFromJSON(BigQueryClient client,
            string datasetId, string tableId)
        {
            // [START bigquery_load_table_gcs_json]
            var gcsURI = "gs://cloud-samples-data/bigquery/us-states/us-states.json";

            var dataset = client.CreateDataset(datasetId);

            var schema = new TableSchemaBuilder {
                { "name", BigQueryDbType.String },
                { "post_abbr", BigQueryDbType.String }
            }.Build();

            var jobOptions = new CreateLoadJobOptions()
            {
                SourceFormat = FileFormat.NewlineDelimitedJson
            };

            var loadJob = client.CreateLoadJob(gcsURI, dataset.GetTableReference(tableId),
                schema, jobOptions);

            loadJob.PollUntilCompleted();

            // [END bigquery_load_table_gcs_json]
        }

        /// <summary>
        /// Imports data from a CSV source
        /// </summary>
        /// <param name="datasetId">Dataset identifier.</param>
        /// <param name="tableId">Table identifier.</param>
        /// <param name="client">A BigQuery client.</param>
        public static void LoadTableFromCSV(string datasetId,
            string tableId, BigQueryClient client)
        {
            // [START bigquery_load_table_gcs_csv]
            var gcsURI = "gs://cloud-samples-data/bigquery/us-states/us-states.csv";

            var dataset = client.CreateDataset(datasetId);

            var schema = new TableSchemaBuilder {
                { "name", BigQueryDbType.String },
                { "post_abbr", BigQueryDbType.String }
            }.Build();

            var jobOptions = new CreateLoadJobOptions()
            {
                // The source format defaults to CSV; line below is optional.
                SourceFormat = FileFormat.Csv,
                SkipLeadingRows = 1
            };

            var loadJob = client.CreateLoadJob(gcsURI, dataset.GetTableReference(tableId),
                schema, jobOptions);

            loadJob.PollUntilCompleted();

            // [END bigquery_load_table_gcs_csv]
        }

        /// <summary>
        /// Imports data from an Orc source
        /// </summary>
        /// <param name="datasetId">Dataset identifier.</param>
        /// <param name="tableId">Table identifier.</param>
        /// <param name="client">A BigQuery client.</param>
        public static void LoadTableFromOrc(string datasetId,
            string tableId, BigQueryClient client)
        {
            // [START bigquery_load_table_gcs_orc]
            var gcsURI = "gs://cloud-samples-data/bigquery/us-states/us-states.orc";

            var dataset = client.CreateDataset(datasetId);

            var jobOptions = new CreateLoadJobOptions()
            {
                SourceFormat = FileFormat.Orc
            };

            // Pass null as the schema because the schema is inferred when
            // loading Orc data
            var loadJob = client.CreateLoadJob(gcsURI, dataset.GetTableReference(tableId),
                null, jobOptions);

            loadJob.PollUntilCompleted();

            // [END bigquery_load_table_gcs_orc]
        }
    }
}
