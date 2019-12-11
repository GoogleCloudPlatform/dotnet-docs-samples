<<<<<<< HEAD
﻿// Copyright 2019 Google LLC
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

using Xunit;
=======
﻿using Xunit;
>>>>>>> d42fde96... finished IT

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class LanguageEntityExtractionModelManagementIT
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId;
        public LanguageEntityExtractionModelManagementIT(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _modelId = "TEN623594616762925056";
        }

        [Fact]
        public void TestDeployUndeployModel()
        {
            // export dataset
            ConsoleOutput output = _fixture.SampleRunner.Run("undeploy_model", _fixture.ProjectId, _modelId);

            Assert.Contains("Model undeployment finished", output.Stdout);

            output = _fixture.SampleRunner.Run("deploy_model", _fixture.ProjectId, _modelId);

            Assert.Contains("Model deployment finished", output.Stdout);
        }
    }
}
