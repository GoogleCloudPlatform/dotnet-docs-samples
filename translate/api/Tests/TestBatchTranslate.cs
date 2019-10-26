using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Google.Cloud.Translate.V3.Samples;
using GoogleCloudSamples;
using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;
using System.Linq;

namespace Tests
{
    public class BatchTranslateTest : IDisposable
    {
        protected string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GCLOUD_PROJECT");
        protected string _bucketName { get; private set; }
        protected Bucket Bucket { get; private set; }
        protected string InputUri { get; private set; } = "gs://cloud-samples-data/translation/glossary_ja.csv";
        readonly CommandLineRunner _sample = new CommandLineRunner()
        {
            VoidMain = TranslateV3BatchTranslateTextMain.Main,
            Command = "Batch Translate Text"
        };

        //Setup
        public BatchTranslateTest()
        {
            //Create temp bucket
            var storageClient = StorageClient.Create();
            _bucketName = "translate-v3-" + TestUtil.RandomName();
            Bucket = storageClient.CreateBucket(ProjectId, _bucketName);
        }

        public void Dispose()
        {
            var storageClient = StorageClient.Create();

            // Clean up output files.
            var blobList = storageClient.ListObjects(_bucketName, "");
            foreach (var outputFile in blobList.Where(x => x.Name.Contains("translation/")).Select(x => x.Name))
            {
                storageClient.DeleteObject(_bucketName, outputFile);
            }
            storageClient.DeleteBucket(_bucketName);
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
        public void TestBatchTranslateText()
        {
            string outputUri =
                string.Format("gs://{0}/translation/BATCH_TRANSLATION_OUTPUT/", _bucketName);

            var output = _sample.Run("--project_id=" + ProjectId,
                "--location=us-central1",
                "--source_lang=en",
                "--target_lang=es",
                "--output_uri=" + outputUri,
                "--input_uri=gs://cloud-samples-data/translation/text.txt");

            Assert.Contains("Total Characters: 13", output.Stdout);
        }
    }
}
