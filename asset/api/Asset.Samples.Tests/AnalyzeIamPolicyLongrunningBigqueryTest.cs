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
public class AnalyzeIamPolicyLongrunningBigqueryTest
{
    private readonly AssetFixture _fixture;
    private readonly AnalyzeIamPolicyLongrunningBigquerySample _sample;

    public AnalyzeIamPolicyLongrunningBigqueryTest(AssetFixture fixture)
    {
        _fixture = fixture;
        _sample = new AnalyzeIamPolicyLongrunningBigquerySample();
    }

    [Fact]
    public void TestAnalyzeIamPolicyLongrunningBigquery()
    {
        // Run the sample code.
        string scope = $"projects/{_fixture.ProjectId}";
        string fullResourceName =
            $"//cloudresourcemanager.googleapis.com/projects/{_fixture.ProjectId}";
        string dataset =
            $"projects/{_fixture.ProjectId}/datasets/{_fixture.DatasetId}";
        string tablePrefix = "client_library_table";
        AnalyzeIamPolicyLongrunningRequest returnedRequest =
            _sample.AnalyzeIamPolicyLongrunning(scope, fullResourceName, dataset, tablePrefix);

        Assert.Equal(dataset, returnedRequest.OutputConfig.BigqueryDestination.Dataset);
        Assert.Equal(tablePrefix, returnedRequest.OutputConfig.BigqueryDestination.TablePrefix);
    }
}
