/*
 * Copyright (c) 2015 Google Inc.
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

using Newtonsoft.Json.Linq;
using System;
using Xunit;

namespace GoogleCloudSamples
{
    public class AuthTest
    {
        static readonly CommandLineRunner s_runner = new CommandLineRunner
        {
            Command = "AuthSample.exe",
            VoidMain = AuthSample.Main,
        };

        string JsonPath
        {
            get
            {
                return System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            }
        }

        [Theory]
        [InlineData("hand")]
        [InlineData("cloud")]
        [InlineData("api")]
        [InlineData("http")]
        void TestImplicit(string cmd)
        {
            var output = s_runner.Run(cmd);
            Assert.Equal(0, output.ExitCode);
            Assert.False(string.IsNullOrWhiteSpace(output.Stdout));
        }

        [Theory]
        [InlineData("hand")]
        [InlineData("cloud")]
        [InlineData("api")]
        [InlineData("http")]
        void TestExplicit(string cmd)
        {
            var output = s_runner.Run(cmd, "-j", JsonPath);
            Assert.Equal(0, output.ExitCode);
            Assert.False(string.IsNullOrWhiteSpace(output.Stdout));
        }

        [Theory]
        [InlineData("hand")]
        [InlineData("cloud")]
        [InlineData("api")]
        void TestExplicitComputeEngine(string cmd)
        {
            string metadataJson =
                Google.Api.Gax.Platform.Instance()?.GceDetails?.MetadataJson;
            if (null == metadataJson)
            {
                Console.WriteLine("Cannot test Compute Engine methods because "
                    + "I'm not running on compute engine.");
                return;
            }
            dynamic metadata = JObject.Parse(metadataJson);
            JObject serviceAccounts = metadata?.instance?.serviceAccounts;
            bool hasServiceAccounts = null == serviceAccounts ?
                false : serviceAccounts.HasValues;
            if (!hasServiceAccounts)
            {
                Console.WriteLine("Cannot test Compute Engine methods because "
                    + "this GCE instance has no service accounts.");
                return;
            }
            var output = s_runner.Run(cmd, "-c");
            Assert.Equal(0, output.ExitCode);
            Assert.False(string.IsNullOrWhiteSpace(output.Stdout));
        }
    }
}
