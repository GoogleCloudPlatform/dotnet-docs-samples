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
    public class BatchTranslateWithGlossaryTest : IDisposable
    {
        protected string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        protected string _bucketName { get; private set; }

        protected string GlossaryId { get; private set; }
        protected string GlossaryInputUri { get; private set; } = "gs://cloud-samples-data/translation/glossary_ja.csv";
        protected string InputUri { get; private set; } = "gs://cloud-samples-data/translation/text_with_glossary.txt";
        readonly CommandLineRunner _sample = new CommandLineRunner()
        {
            VoidMain = BatchTranslateTextWithGlossaryMain.Main
        };

        //Setup
        public BatchTranslateWithGlossaryTest()
        {
            //Create temp bucket
            var storageClient = StorageClient.Create();
            _bucketName = "translate-v3-" + TestUtil.RandomName();
            storageClient.CreateBucket(ProjectId, _bucketName);

            //Create temp glossary
            GlossaryId = "must-start-with-letters" + TestUtil.RandomName();
            TranslateV3CreateGlossary.SampleCreateGlossary(ProjectId, GlossaryId, GlossaryInputUri);
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

            // Clean up glossary
            TranslateV3DeleteGlossary.SampleDeleteGlossary(ProjectId, GlossaryId);
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
        public void TestBatchTranslateTextWithGlossary()
        {
            string outputUri =
                string.Format("gs://{0}/translation/BATCH_TRANSLATION_OUTPUT/", _bucketName);

            var output = _sample.Run("--project_id=" + ProjectId,
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
