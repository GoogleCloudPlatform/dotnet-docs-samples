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
    public class BatchTranslateWithGlossaryTest : IDisposable
    {
        private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        private readonly string _bucketName;

        protected string GlossaryId { get; private set; }
        protected string GlossaryInputUri { get; private set; } = "gs://cloud-samples-data/translation/glossary_ja.csv";
        protected string InputUri { get; private set; } = "gs://cloud-samples-data/translation/text_with_glossary.txt";
        private readonly CommandLineRunner _sample = new CommandLineRunner()
        {
            VoidMain = BatchTranslateTextWithGlossaryMain.Main
        };

        // Setup
        public BatchTranslateWithGlossaryTest()
        {
            // Create temp bucket
            var storageClient = StorageClient.Create();
            _bucketName = "translate-v3-" + TestUtil.RandomName();
            storageClient.CreateBucket(_projectId, _bucketName);

            // Create temp glossary
            GlossaryId = "must-start-with-letters" + TestUtil.RandomName();
            TranslateV3CreateGlossary.CreateGlossarySample(_projectId, GlossaryId, GlossaryInputUri);
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

            // Clean up glossary
            TranslateV3DeleteGlossary.DeleteGlossarySample(_projectId, GlossaryId);
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
        public void BatchTranslateTextWithGlossary()
        {
            string outputUri =
                string.Format("gs://{0}/translation/BATCH_TRANSLATION_OUTPUT/", _bucketName);

            var output = _sample.Run("--project_id=" + _projectId,
                "--location=us-central1",
                "--source_language=en",
                "--target_language=ja",
                "--glossary_id=" + GlossaryId,
                "--output_uri=" + outputUri,
                "--input_uri=" + InputUri);
            Console.WriteLine(output.Stdout);
            Assert.Contains("Total Characters: 9", output.Stdout);
        }
    }
}
