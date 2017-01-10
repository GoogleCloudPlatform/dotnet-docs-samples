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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Drawing;

namespace GoogleCloudSamples
{
    public class VisionTest
    {
        readonly CommandLineRunner _detectFaces = new CommandLineRunner()
        {
            VoidMain = DetectFaces.Main,
            Command = "DetectFaces"
        };
        readonly CommandLineRunner _detectLabels = new CommandLineRunner()
        {
            VoidMain = DetectLabels.Main,
            Command = "DetectLabels"
        };
        readonly CommandLineRunner _detectLandmarks = new CommandLineRunner()
        {
            VoidMain = DetectLandmarks.Main,
            Command = "DetectLandmarks"
        };

        [Fact]
        public void TestLabelsWithCat()
        {
            var output = _detectLabels.Run(@"data\cat.jpg");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("cat", output.Stdout);
        }
        [Fact]
        public void TestLabelsWithNoArgs()
        {
            var output = _detectLabels.Run();
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Google Cloud Vision API", output.Stdout);
        }

        [Fact]
        public void TestLandmarksWithCat()
        {
            var output = _detectLandmarks.Run(@"data\cat.jpg");
            Assert.Equal(0, output.ExitCode);
            Assert.Equal("", output.Stdout);  // No landmarks.
        }
        [Fact]
        public void TestLandmarksWithTower()
        {
            var output = _detectLandmarks.Run(@"data\tower.jpg");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Eiffel", output.Stdout);
        }
        [Fact]
        public void TestLandmarksWithNoArgs()
        {
            var output = _detectLandmarks.Run();
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Google Cloud Vision API", output.Stdout);
        }

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
            // Tower.jpg contains no faces, so the output file should be 
            // identical to the original.
            File.Delete(@"data\tower.faces.jpg");
            var output = _detectFaces.Run(@"data\tower.jpg");
            Assert.Equal(0, output.ExitCode);
            Assert.Equal(ToPngBytes(@"data\tower.jpg"),
                ToPngBytes(@"data\tower.faces.jpg"));
        }

        [Fact]
        public void TestFacesWithFace()
        {
            // Face.png contains a face, so the output should be different
            // from the original.
            File.Delete(@"data\face.faces.png");
            var output = _detectFaces.Run(@"data\face.png");
            Assert.Equal(0, output.ExitCode);
            Assert.NotEqual(ToPngBytes(@"data\face.png"),
                ToPngBytes(@"data\face.faces.png"));
        }

        public static byte[] ToPngBytes(string imagePath)
        {
            var stream = new MemoryStream();
            var image = Image.FromFile(imagePath);
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
