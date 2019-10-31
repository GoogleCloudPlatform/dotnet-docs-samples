using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using TranslateV3Samples;
using GoogleCloudSamples;
using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;
using System.Linq;

namespace Tests
{
    public class BatchTranslateTest : IDisposable
    {
        private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        private readonly string _bucketName;

        private readonly CommandLineRunner _sample = new CommandLineRunner()
        {
            VoidMain = TranslateV3BatchTranslateTextMain.Main
        };

        // Setup
        public BatchTranslateTest()
        {
            // Create temp bucket
            using (var storageClient = StorageClient.Create()){
                _bucketName = "translate-v3-" + TestUtil.RandomName();
                storageClient.CreateBucket(_projectId, _bucketName);
            }
        }

        public void Dispose()
        {
            using (var storageClient = StorageClient.Create()) {
                // Clean up output files.
                var blobList = storageClient.ListObjects(_bucketName, "");
                foreach (var outputFile in blobList.Where(x => x.Name.Contains("translation/")).Select(x => x.Name))
                {
                    storageClient.DeleteObject(_bucketName, outputFile);
                }
                storageClient.DeleteBucket(_bucketName);
            }
        }

        /// <summary>
        ///  Run the command and track all cloud assets that were created.
        /// </summary>
        /// <param name="arguments">The command arguments.</param>
        public ConsoleOutput Run(params string[] arguments)
        {
            return _sample.Run(arguments);
        }

        [Fact]
        public void BatchTranslateText()
        {
            string outputUri =
                string.Format("gs://{0}/translation/BATCH_TRANSLATION_OUTPUT/", _bucketName);

            var output = _sample.Run("--project_id=" + _projectId,
                "--location=us-central1",
                "--source_language=en",
                "--target_language=es",
                "--output_uri=" + outputUri,
                "--input_uri=gs://cloud-samples-data/translation/text.txt");

            Assert.Contains("Total Characters: 13", output.Stdout);
        }
    }
}
