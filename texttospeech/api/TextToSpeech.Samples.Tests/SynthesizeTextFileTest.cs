// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.IO;
using Xunit;

public class SynthesizeTextFileTest
{
    [Fact]
    public void SynthesizeTextFile()
    {
        string filePath = Path.Combine("data", "hello.txt");
        string outputFile = "synthesize-text-file.mp3";

        var synthesizeTextFileSample = new SynthesizeTextFileSample();

        synthesizeTextFileSample.SynthesizeTextFile(filePath, outputFile);

        Assert.True(File.Exists(outputFile));

        FileInfo fileInfo = new FileInfo(outputFile);
        Assert.True(fileInfo.Length > 0);

        // Clean up
        File.Delete(outputFile);
    }
}
