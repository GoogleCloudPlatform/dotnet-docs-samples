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
		/// <param name="gcsURI"></param>
		/// <param name="field1"></param>
		/// <param name="field2"></param>
        public static void LoadTableFromJSON(BigQueryClient client,
		    string datasetId, string tableId, string gcsURI, string field1,
		    string field2)
        {
            // [START bigquery_load_table_gcs_json]
            var dataset = client.CreateDataset(datasetId);

            var schema = new TableSchemaBuilder
            {
                { field1, BigQueryDbType.String },
                { field2, BigQueryDbType.String }
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
        /// <param name="gcsURI">URI of the source.</param>
        /// <param name="field1">Field in the source file.</param>
        /// <param name="field2">Another field in the source file.</param>
        public static void LoadTableFromCSV(string datasetId,
		    string tableId, BigQueryClient client, string gcsURI, string field1, string field2)
        {
			// [START bigquery_load_table_gcs_csv]
			var dataset = client.CreateDataset(datasetId);
            
			var schema = new TableSchemaBuilder {
				{ field1, BigQueryDbType.String },
				{ field2, BigQueryDbType.String }
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
        
    }
}
