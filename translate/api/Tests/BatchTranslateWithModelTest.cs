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
    public class BatchTranslateWithModelTest : IDisposable
    {
        private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        private readonly string _bucketName;
        protected Bucket Bucket { get; private set; }
        protected string InputUri { get; private set; } = "gs://cloud-samples-data/translation/custom_model_text.txt";

        protected string ModelId { get; private set; } = "TRL8772189639420149760";
        private readonly CommandLineRunner _sample = new CommandLineRunner()
        {
            VoidMain = TranslateV3BatchTranslateTextWithModelMain.Main
        };

        // Setup
        public BatchTranslateWithModelTest()
        {
            // Create temp bucket
            using (var storageClient = StorageClient.Create())
            {
                _bucketName = "translate-v3-" + TestUtil.RandomName();
                storageClient.CreateBucket(_projectId, _bucketName);
            }
        }

        public void Dispose()
        {
            using (var storageClient = StorageClient.Create())
            {
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
        public void BatchTranslateTextWithModel()
        {
            string outputUri =
                string.Format("gs://{0}/translation/BATCH_TRANSLATION_OUTPUT/", _bucketName);

            var output = _sample.Run("--project_id=" + _projectId,
                "--location=us-central1",
                "--source_language=en",
                "--target_language=ja",
                "--output_uri=" + outputUri,
                "--input_uri=" + InputUri,
                "--model_id=" + ModelId);

            Assert.Contains("Total Characters: 15", output.Stdout);
        }
    }
}
