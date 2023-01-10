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
using System;
using System.Threading.Tasks;
using Xunit;

namespace Concepts.Tests;

// Note: these tests modify the "FOO" environment variable
// without any attempt to restore the original value. We assume
// no other tests will use that environment variable.
public class EnvironmentVariablesTest : FunctionTestBase<EnvironmentVariables.Function>
{
    [Fact]
    public async Task VariableSet()
    {
        Environment.SetEnvironmentVariable("FOO", "Test foo value");
        var responseBody = await ExecuteHttpGetRequestAsync();
        Assert.Equal("Test foo value", responseBody);
    }

    [Fact]
    public async Task VariableNotSet()
    {
        Environment.SetEnvironmentVariable("FOO", null);
        var responseBody = await ExecuteHttpGetRequestAsync();
        Assert.Equal("Specified environment variable is not set.", responseBody);
    }
}
