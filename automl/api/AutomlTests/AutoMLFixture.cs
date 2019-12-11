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
        public readonly string ProjectId;
        public readonly Bucket Bucket;

        public CommandLineRunner SampleRunner { get; } = new CommandLineRunner()
        {
            Main = AutoMLProgram.Main
        };

        public AutoMLFixture()
        {
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            _client = StorageClient.Create();
            string BucketName = "translate-v3-bucket-" + TestUtil.RandomName();
            Bucket bucket = new Bucket { Location = "us-central1", Name = BucketName };
            Bucket = _client.CreateBucket(ProjectId, bucket);
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
