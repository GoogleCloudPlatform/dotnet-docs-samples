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

public class TranslateTextWithModelTests
{
    private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    private readonly string _modelId = "TRL8772189639420149760";

    private readonly CommandLineRunner _sample = new CommandLineRunner()
    {
        VoidMain = TranslateV3Samples.Main
    };

    [Fact]
    public void TranslateTextWithModelTest()
    {
        var output = _sample.Run("translateTextWithModel",
            "--project_id=" + _projectId,
            "--location=us-central1", "--text=That' il do it",
            "--target_language=ja", "--model_id=" + _modelId);
        Assert.True(output.Stdout.Contains("\u305D\u308C\u306F\u305D\u3046\u3060") || output.Stdout.Contains("\u3084\u308B"));
    }
}



