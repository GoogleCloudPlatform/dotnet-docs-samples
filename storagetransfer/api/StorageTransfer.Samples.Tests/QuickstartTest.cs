using Google.Cloud.StorageTransfer.V1;
using Xunit;

namespace StorageTransfer.Samples.Tests
{
    [Collection(nameof(StorageFixture))]
    public class QuickstartTest
    {
        private readonly StorageFixture _fixture;

        public QuickstartTest(StorageFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestQuickstart()
        {
            QuickstartSample quickstartSample = new QuickstartSample();
            var transferJob = quickstartSample.Quickstart(_fixture.ProjectId, _fixture.BucketNameSource, _fixture.BucketNameSink);
            Assert.Contains("transferJobs/", transferJob.Name);

            // Delete the job to clean up
            _fixture.Sts.UpdateTransferJob(new UpdateTransferJobRequest()
            {
                ProjectId = _fixture.ProjectId,
                JobName = transferJob.Name,
                TransferJob = new TransferJob()
                {
                    Name = transferJob.Name,
                    Status = TransferJob.Types.Status.Deleted
                }
            });
        }
    }
}

