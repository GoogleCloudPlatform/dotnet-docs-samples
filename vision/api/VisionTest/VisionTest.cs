// Copyright(c) 2017 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using Google.Cloud.Storage.V1;
using Google.Cloud.Vision.V1;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace GoogleCloudSamples
{
    /// <summary>
    /// Tests for the old tutorial samples.
    /// </summary>
    public class TutorialTests
    {
        readonly CommandLineRunner _detectFaces = new CommandLineRunner()
        {
            VoidMain = DetectFaces.Main,
            Command = "DetectFaces"
        };

        [Fact]
        public void TestFacesWithNoArgs()
        {
            var output = _detectFaces.Run();
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Google Cloud Vision API", output.Stdout);
        }

        [Fact]
        public void TestFacesWithTower()
        {
            // Tower.jpg contains no faces, so the output response should 
            // confirm this.
            File.Delete(Path.Combine("data", "tower.faces.jpg"));
            var output = _detectFaces.Run(Path.Combine("data", "tower.jpg"));
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Found 0 faces.", output.Stdout);
        }

        [Fact]
        public void TestFacesWithFace()
        {
            // Face.png contains a face, so the output file should be different
            // from the original.
            File.Delete(Path.Combine("data", "face.faces.png"));
            var output = _detectFaces.Run(Path.Combine("data", "face.png"));
            Assert.Equal(0, output.ExitCode);
            Assert.NotEqual(ToPngBytes(Path.Combine("data", "face.png")),
                ToPngBytes(Path.Combine("data", "face.faces.png")));
        }

        public static byte[] ToPngBytes(string imagePath)
        {
            var stream = new MemoryStream();
            using (var image = System.Drawing.Image.FromFile(imagePath))
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }

    /// <summary>
    /// For every DetectYadaYada function, we need to test with a local file
    /// and with a file on Google Cloud Storage.  This class contains all
    /// the real tests and assertions.  Derived classes implement Run().
    /// </summary>
    public abstract class CommonTests
    {
        /// <summary>
        /// Derived classes implement this function to examine the file
        /// locally, or first upload it to Google Cloud Storage and then
        /// examine it.
        /// </summary>
        /// <param name="args">Command line arguments to Main().</param>
        protected abstract ConsoleOutput Run(params string[] args);

        [Fact]
        public void DetectFace()
        {
            var output = Run("faces", Path.Combine("data", "face.png"));
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Joy", output.Stdout);
        }

        [Fact]
        public void DetectNoFace()
        {
            var output = Run("faces", Path.Combine("data", "tower.jpg"));
            Assert.Equal(0, output.ExitCode);
            Assert.DoesNotContain("Joy", output.Stdout);
        }

        [Fact]
        public void DetectLabel()
        {
            var output = Run("labels", Path.Combine("data", "cat.jpg"));
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("cat", output.Stdout);
        }

        [Fact]
        public void DetectLandmark()
        {
            var output = Run("landmarks", Path.Combine("data", "tower.jpg"));
            Assert.Equal(0, output.ExitCode);
            Assert.Matches(@".*(Eiffel|Mars).*", output.Stdout);
        }

        [Fact]
        public void DetectText()
        {
            var output = Run("text", Path.Combine("data", "bonito.gif"));
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("fermented", output.Stdout);
        }

        [Fact]
        public void DetectTextWithMultiRegion()
        {
            var output = Run("text", Path.Combine("data", "bonito.gif"), "-m");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("fermented", output.Stdout);
        }

        [Fact]
        public void DetectNoText()
        {
            var output = Run("text", Path.Combine("data", "no-text.jpg"));
            Assert.Equal(0, output.ExitCode);
            Assert.Equal("", output.Stdout);
        }

        [Fact]
        public void DetectLogos()
        {
            var output = Run("text", Path.Combine("data", "logo.jpg"));
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("google", output.Stdout.ToLower());
        }

        [Fact]
        public void DetectNoLogos()
        {
            var output = Run("text", Path.Combine("data", "cat.jpg"));
            Assert.Equal(0, output.ExitCode);
            Assert.Equal("", output.Stdout);
        }

        [Fact]
        public void DetectProperties()
        {
            var output = Run("properties", Path.Combine("data", "logo.jpg"));
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Alpha", output.Stdout);
        }

        [Fact]
        public void DetectSafeSearch()
        {
            var output = Run("safe-search", Path.Combine("data", "logo.jpg"));
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Spoof", output.Stdout);
            Assert.Contains("Unlikely", output.Stdout);
        }

        [Fact]
        public void DetectWeb()
        {
            var output = Run("web", Path.Combine("data", "tower.jpg"));
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("MatchingImage", output.Stdout);
        }


        [Fact]
        public void DetectCropHint()
        {
            var output = Run("crop-hint", Path.Combine("data", "face.png"));
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Confidence:", output.Stdout);
            Assert.Contains("Bounding Polygon:", output.Stdout);
            Assert.Contains("\tX:", output.Stdout);
            Assert.Contains("\tY:", output.Stdout);
        }

        [Fact]
        public void DetectObjectLocalization()
        {
            // TODO(erschmid): Replace with 'puppies.jpg' after merge.
            var output = Run("object-localization",
                             Path.Combine("data", "tower.jpg"));
            Assert.Contains("Building", output.Stdout);
            Assert.Equal(0, output.ExitCode);
        }
    }

    /// <summary>
    /// Runs tests on local file.
    /// </summary>
    public class LocalTests : CommonTests
    {
        readonly CommandLineRunner _detect = new CommandLineRunner()
        {
            VoidMain = DetectProgram.Main,
            Command = "Detect"
        };

        protected override ConsoleOutput Run(params string[] args)
        {
            return _detect.Run(args);
        }
    }

    /// <summary>
    /// Uploads the local file to Google Cloud Storage, then runs test
    /// using the Cloud Storage location prefixed with "gs://".
    /// </summary>
    public class CloudStorageTests : CommonTests, IClassFixture<RandomBucketFixture>
    {
        readonly CommandLineRunner _detect = new CommandLineRunner()
        {
            VoidMain = DetectProgram.Main,
            Command = "Detect"
        };
        readonly string _bucketName;

        public CloudStorageTests(RandomBucketFixture bucketFixture)
        {
            _bucketName = bucketFixture.BucketName;
        }

        protected override ConsoleOutput Run(params string[] args)
        {
            string objectName = "VisionTest/" + Path.GetFileName(args[1]);
            string[] cmdArgs = { args[0], $"gs://{_bucketName}/{objectName}" };
            using (var collector = new BucketCollector(_bucketName))
            {
                collector.CopyToBucket(args[1], objectName);
                return _detect.Run(cmdArgs);
            }
        }

        [Fact]
        public void DetectPdfDocument()
        {
            var _pdfFileName = "HodgeConj.pdf";
            var outputPrefix = "";
            var localPath = Path.Combine("data", _pdfFileName);
            var gcsSourceURI = $"gs://{_bucketName}/{_pdfFileName}";
            ConsoleOutput output;

            string[] cmdArgs = { "ocr", gcsSourceURI, _bucketName, outputPrefix };

            using (var collector = new BucketCollector(_bucketName))
            {
                collector.CopyToBucket(localPath, _pdfFileName);
                output = _detect.Run(cmdArgs);

                Assert.Equal(0, output.ExitCode);
                Assert.Contains("Full text:", output.Stdout);
                Assert.Contains("Hodge conjecture", output.Stdout);
            }

            // Clean up output files.
            var storageClient = StorageClient.Create();
            var blobList = storageClient.ListObjects(_bucketName, "");
            foreach (var outputFile in blobList.Where(x => x.Name.Contains(".json")).Select(x => x.Name))
            {
                storageClient.DeleteObject(_bucketName, outputFile);
            }
        }
    }

    /// <summary>
    /// Runs tests using files on public URIs.
    /// </summary>
    public class PublicUriTests : CommonTests
    {
        readonly RetryRobot _retryRobot = new RetryRobot()
        {
            ShouldRetry = (Exception e) =>
            {
                // Sometimes the API is not able to access the URL.
                // Is github throttling us?
                var annotateImageException = e as AnnotateImageException;
                return annotateImageException != null &&
                    annotateImageException.Message.Contains("URL");
            }
        };

        readonly CommandLineRunner _detect = new CommandLineRunner()
        {
            VoidMain = DetectProgram.Main,
            Command = "Detect"
        };

        protected override ConsoleOutput Run(params string[] args)
        {
            string uriPrefix =
                "https://raw.githubusercontent.com/GoogleCloudPlatform/";
            string uriSampleFilePath =
                "dotnet-docs-samples/master/vision/api/VisionTest/data/";
            string filePublicUri =
                uriPrefix + uriSampleFilePath + Path.GetFileName(args[1]);
            return _retryRobot.Eventually(
                () => _detect.Run(args[0], filePublicUri));
        }
    }

    /// <summary>
    /// Runs tests on local file.
    /// </summary>
    public class QuickStartTests
    {
        readonly CommandLineRunner _quickStart = new CommandLineRunner()
        {
            VoidMain = QuickStart.Main,
            Command = "QuickStart"
        };

        [Fact]
        public void TestRun()
        {
            var output = _quickStart.Run();
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("cat", output.Stdout);
        }
    }
}
