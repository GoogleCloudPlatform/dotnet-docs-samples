// Copyright 2020 Google LLC
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

using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace Logging.Tests
{
    public class LogHelloWorldTest : FunctionTestBase<LogHelloWorld.Function>
    {
        [Fact]
        public async Task LogEntriesWritten()
        {
            var (stdout, stderr) = await RunWithConsoleRedirection(async () =>
            {
                using (var client = Server.CreateClient())
                {
                    var response = await client.GetAsync("uri");
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Assert.Equal("Messages successfully logged!", responseBody);
                };
            });

            // The console output will include ASP.NET Core logs (including the info and warning
            // logs written in the function) because our test logger doesn't disable the console
            // logger, but we should at least have the expected message as well.
            Assert.Contains("I am a log to stdout!\n", stdout);
            Assert.Contains("I am a log to stderr!\n", stderr);
            
            var logEntries = GetFunctionLogEntries();
            var info = Assert.Single(logEntries, entry => entry.Level == LogLevel.Information);
            Assert.Equal("I am an info log!", info.Message);

            var warning  = Assert.Single(logEntries, entry => entry.Level == LogLevel.Warning);
            Assert.Equal("I am a warning log!", warning.Message);
        }
    }
}
