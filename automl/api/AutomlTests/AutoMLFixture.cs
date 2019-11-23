using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using Xunit;

namespace GoogleCloudSamples
{
    [CollectionDefinition(nameof(AutoMLFixture))]
    public class AutoMLFixture : ICollectionFixture<AutoMLFixture>
    {
        private readonly StorageClient _client;

        public string ProjectId { get; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        //public string ModelId { get; } = "TRL8772189639420149760";
        //public string GlossaryId { get; }
        public Bucket Bucket { get; set; }

        public CommandLineRunner SampleRunner { get; } = new CommandLineRunner()
        {
            Main = AutoMLProgram.Main
        };

        public AutoMLFixture()
        {
            _client = StorageClient.Create();
            string BucketName = "translate-v3-bucket-" + TestUtil.RandomName();
            //GlossaryId = "must-start-with-letters" + TestUtil.RandomName();

            Bucket = _client.CreateBucket(ProjectId, BucketName);
        }

        public void Dispose()
        {
            _client.DeleteBucket(Bucket.Name,
            new DeleteBucketOptions
            {
                DeleteObjects = true
            });
            _client.Dispose();
        }
    }
}
