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

using System;
using System.Diagnostics;
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
        [InlineData("cloud")]
        [InlineData("api")]
        void TestExplicitComputeEngine(string cmd)
        {
            try
            {
                Console.WriteLine(Google.Api.Gax.Platform.Instance()?.GceDetails?.MetadataJson);
                var output = s_runner.Run(cmd, "-c");
                Assert.Equal(0, output.ExitCode);
                Assert.False(string.IsNullOrWhiteSpace(output.Stdout));
            }
            catch (System.Net.Http.HttpRequestException)
            when (null == Google.Api.Gax.Platform.Instance()?.GceDetails?.InstanceId)
            {
                Console.WriteLine("Cannot test Compute Engine methods because I'm "
                    + "not running on compute engine.");
            }            
            catch (Google.Apis.Auth.OAuth2.Responses.TokenResponseException)
            when (null == Google.Api.Gax.Platform.Instance()?.GceDetails?.InstanceId)
            {
                Console.WriteLine("Cannot test Compute Engine methods because I'm "
                    + "not running on compute engine.");
            }
        }
    }
}
