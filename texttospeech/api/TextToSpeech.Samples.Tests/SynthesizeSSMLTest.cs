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

public class SynthesizeSSMLTest
{
    [Fact]
    public void SynthesizeSSML()
    {
        string ssml = "<speak>This is <say-as interpret-as='characters'>SSML</say-as></speak>";
        string outputFile = "synthesize-ssml.mp3";

        var synthesizeSSMLSample = new SynthesizeSSMLSample();

        synthesizeSSMLSample.SynthesizeSSML(ssml, outputFile);

        Assert.True(File.Exists(outputFile));

        FileInfo fileInfo = new FileInfo(outputFile);
        Assert.True(fileInfo.Length > 0);

        // Clean up
        File.Delete(outputFile);
    }
}
