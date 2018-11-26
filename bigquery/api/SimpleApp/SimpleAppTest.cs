/*
 * Copyright (c) 2018 Google LLC
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

using System.Linq;
using Xunit;

namespace GoogleCloudSamples
{
    public class SimpleAppTest
    {
        readonly CommandLineRunner _simpleApp = new CommandLineRunner()
        {
            VoidMain = Program.Main,
            Command = "dotnet run"
        };

        [Fact]
        public void TestRunSimpleApp()
        {
            var output = _simpleApp.Run();
            Assert.Equal(0, output.ExitCode);
            var outputLines = output.Stdout.Split(new[] { '\n' });
            string rowPrefix = "https://stackoverflow.com/questions/";
            int rowCount = outputLines.Where(
                line => line.StartsWith(rowPrefix)).Count();
            Assert.Equal(10, rowCount);
        }
    }
}
