// Copyright 2019 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Xunit;
using GoogleCloudSamples;
using CommandLine;

public class TranslateTextWithGlossaryAndModelTests : IDisposable
{
    private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    private readonly string _glossaryInputUri = "gs://cloud-samples-data/translation/glossary_ja.csv";
    private readonly string _modelId = "TRL8772189639420149760";
    private readonly string _glossaryId;

    private readonly CommandLineRunner _sample = new CommandLineRunner()
    {
        VoidMain = TranslateV3Samples.Main
    };

    // Setup
    public TranslateTextWithGlossaryAndModelTests()
    {
        _glossaryId = "translate-v3" + TestUtil.RandomName();
        TranslateV3CreateGlossary.CreateGlossarySample(_projectId, _glossaryId, _glossaryInputUri);
    }

    // TearDown
    public void Dispose()
    {
        TranslateV3DeleteGlossary.DeleteGlossarySample(_projectId, _glossaryId);
    }

    [Fact]
    public void TranslateTextWithGlossaryAndModelTest()
    {
        var output = _sample.Run("translateTextWithGlossaryAndModel",
            "--project_id=" + _projectId,
            "--location=us-central1",
            "--text=That' il do it. deception",
            "--target_language=ja",
            "--glossary_id=" + _glossaryId,
            "--model_id=" + _modelId);
        Assert.True(output.Stdout.Contains("\u3084\u308B\u6B3A\u304F")
            || output.Stdout.Contains("\u305D\u308C\u3058\u3083\u3042")); // custom model
        Assert.Contains("\u6B3A\u304F", output.Stdout); //glossary
    }
}



