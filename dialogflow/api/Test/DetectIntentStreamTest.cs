using System;
using System.IO;
using Xunit;

namespace GoogleCloudSamples
{
    public class DetectIntentStreamsTest : DialogflowTest
    {

        protected string _audioWavPath = Path.Combine("resources", "book_a_room.wav");

        [Fact]
        void TestDetectIntentFromStream()
        {

            RunWithSessionId("detect-intent:stream", _audioWavPath);
            Assert.Equal(0, ExitCode);

            Assert.Contains("book", Stdout);
            Assert.Contains("Intent detected:", Stdout);
        }
    }
}
