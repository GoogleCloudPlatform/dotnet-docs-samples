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

using Google.Cloud.Functions.Testing;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Logging.Tests;

public class LogHelloWorldTest : FunctionTestBase<LogHelloWorld.Function>
{
    [Fact]
    public async Task LogEntriesWritten()
    {
        var (stdout, stderr) = await RunWithConsoleRedirection(async () =>
        {
            var responseBody = await ExecuteHttpGetRequestAsync("uri");
            Assert.Equal("Messages successfully logged!", responseBody);
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

    public async Task<(string stdout, string stderr)> RunWithConsoleRedirection(Func<Task> func)
    {
        var originalOut = Console.Out;
        var originalError = Console.Error;
        try
        {
            var outWriter = new StringWriter { NewLine = "\n" };
            var errorWriter = new StringWriter { NewLine = "\n" };
            Console.SetOut(outWriter);
            Console.SetError(errorWriter);

            await func().ConfigureAwait(false);
            return (outWriter.ToString(), errorWriter.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }
}
