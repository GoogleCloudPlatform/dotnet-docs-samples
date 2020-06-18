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
using Xunit.Abstractions;

namespace GoogleCloudSamples
{
    public class SearchAllIamPoliciesTest
    {
        static readonly CommandLineRunner s_runner = new CommandLineRunner
        {
            Command = "SearchAllIamPolicies",
            VoidMain = SearchAllIamPolicies.Main,
        };
        private readonly ITestOutputHelper _testOutput;
        public SearchAllIamPoliciesTest(ITestOutputHelper output)
        {
            _testOutput = output;
        }

        [Fact]
        public void TestSearchAllIamPolicies()
        {
            Environment.SetEnvironmentVariable("SCOPE-OF-THE-SEARCH", "projects/" + Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID"));
            Environment.SetEnvironmentVariable("QUERY-STATEMENT", "policy:roles/owner");
            var output = s_runner.Run();
            _testOutput.WriteLine(output.Stdout);
            Assert.Contains("roles/owner", output.Stdout);
        }
    }
}
