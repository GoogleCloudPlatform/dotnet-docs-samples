using Grpc.Core;
using System;
using System.IO;
using System.Threading;
using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class BatchPredictTest
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId;
        public BatchPredictTest(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _modelId = "TEN0000000000000000000";
        }

        [Fact]
        public void TestBatchPredict()
        {
            try
            {
                string bucketId = $"{ _fixture.ProjectId}-lcm";
                string inputUri = $"gs://{bucketId}/entity_extraction/input.jsonl";
                string outputUri = $"gs://{_fixture.Bucket.Name}/TEST_BATCH_PREDICT/";

                // Act
                ConsoleOutput output = _fixture.SampleRunner.Run("batch_predict",
    _fixture.ProjectId, _modelId, inputUri, outputUri);

                // Assert
                Assert.Contains("The model is either not found or not supported for prediction yet.", output.Stdout);
            }
            catch (Exception ex) when (ex is ThreadInterruptedException ||
     ex is IOException || ex is RpcException || ex is AggregateException)
            {
                Assert.Contains("The model is either not found or not supported for prediction yet.", ex.Message);
            }
        }
    }
}
