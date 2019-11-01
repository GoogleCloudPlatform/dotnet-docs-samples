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

public class ListGlossaryTests : IDisposable
{
    private readonly string _projectId = "dotnet-docs-samples-tests";
    private readonly string _glossaryInputUri = "gs://cloud-samples-data/translation/glossary_ja.csv";
    private readonly string _glossaryId;
    private readonly CommandLineRunner _sample = new CommandLineRunner()
    {
        VoidMain = TranslateV3Samples.Main
    };

    // Setup
    public ListGlossaryTests()
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
    public void ListGlossariesTest()
    {
        var output = _sample.Run("listGlossaries",
            "--project_id=" + _projectId);
        Assert.Contains("gs://cloud-samples-data/translation/glossary_ja.csv", output.Stdout);
    }
}



