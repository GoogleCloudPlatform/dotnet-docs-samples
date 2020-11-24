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

using System;
using Xunit;

[Collection(nameof(AssetFixture))]
public class AnalyzeIamPolicyLongrunningTest {
  private readonly AssetFixture _fixture;
  private readonly AnalyzeIamPolicyLongrunningSample _sample;

  public AnalyzeIamPolicyLongrunningTest(AssetFixture fixture) {
    _fixture = fixture;
    _sample = new AnalyzeIamPolicyLongrunningSample();
  }

  [Fact]
  public void TestAnalyzeIamPolicyLongrunningGcs() {
    // Run the sample code.
    string scope = String.Format("projects/{0}", _fixture.ProjectId);
    string fullResourceName =
        String.Format("//cloudresourcemanager.googleapis.com/projects/{0}", _fixture.ProjectId);
    string uri = String.Format("gs://{0}/my-analysis.json", _fixture.BucketName);
    string metadata = _sample.AnalyzeIamPolicyLongrunningGcs(scope, fullResourceName, uri);

    Assert.Contains(uri, metadata);
  }
}
