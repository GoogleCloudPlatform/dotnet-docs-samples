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
using System.IO;
using Xunit;

namespace GoogleCloudSamples
{
    public class ErrorReportingSampleTest
    {
        /// <summary>Runs ErrorReporting.exe with the provided arguments</summary>
        /// <returns>The console output of this program</returns>
        public string Run(params string[] arguments)
        {
            var standardOut = Console.Out;

            using (var output = new StringWriter())
            {
                Console.SetOut(output);

                try
                {
                    ErrorReportingSample.Main(arguments);
                    return output.ToString();
                }
                finally
                {
                    Console.SetOut(standardOut);
                }
            }
        }

        [Fact]
        public void ReportExceptionTest()
        {
            Assert.Contains(
                "Stackdriver Error Report Sent",
                Run()
            );
        }
    }
}