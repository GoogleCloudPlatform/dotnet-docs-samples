// Copyright (c) 2020 Google LLC
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
// 
// https://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using Xunit;

namespace IAP.Samples.Tests
{
    [CollectionDefinition(nameof(IAPFixture))]
    public class IAPFixture : ICollectionFixture<IAPFixture>
    {
        public string IAPClientId { get; }
        public string IAPUri { get; }
        public string IAPTokenExpectedAudience { get; }

        public IAPFixture()
        {
            IAPClientId = GetEnvVarOrThrow("TEST_IAP_CLIENT_ID");
            IAPUri = GetEnvVarOrThrow("TEST_IAP_URI");
            // The IAP token target audience is of the form /projects/<projectID>/apps/<iap-app-name>
            IAPTokenExpectedAudience = GetEnvVarOrThrow("TEST_IAP_EXPECTED_AUDIENCE");
        }

        private string GetEnvVarOrThrow(string envVarName) =>
            Environment.GetEnvironmentVariable(envVarName) ??
                throw new ArgumentException($"Please set the {envVarName} environment variable.");
    }
}
