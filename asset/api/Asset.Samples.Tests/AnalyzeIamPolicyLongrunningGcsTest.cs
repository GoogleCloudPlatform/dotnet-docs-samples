/*
 * Copyright (c) 2020 Google LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using Google.Cloud.Asset.V1;
using System;
using Xunit;

[Collection(nameof(AssetFixture))]
public class AnalyzeIamPolicyLongrunningGcsTest
{
    private readonly AssetFixture _fixture;
    private readonly AnalyzeIamPolicyLongrunningGcsSample _sample;

    public AnalyzeIamPolicyLongrunningGcsTest(AssetFixture fixture)
    {
        _fixture = fixture;
        _sample = new AnalyzeIamPolicyLongrunningGcsSample();
    }

    [Fact]
    public void TestAnalyzeIamPolicyLongrunningGcs()
    {
        // Run the sample code.
        string scope = $"projects/{_fixture.ProjectId}";
        string fullResourceName =
            $"//cloudresourcemanager.googleapis.com/projects/{_fixture.ProjectId}";
        string uri = $"gs://{_fixture.BucketName}/my-analysis.json";
        AnalyzeIamPolicyLongrunningRequest returnedRequest =
            _sample.AnalyzeIamPolicyLongrunning(scope, fullResourceName, uri);

        Assert.Equal(uri, returnedRequest.OutputConfig.GcsDestination.Uri);
    }
}
