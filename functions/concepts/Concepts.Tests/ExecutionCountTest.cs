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
using System.Threading.Tasks;
using Xunit;

namespace Concepts.Tests;

public class ExecutionCountTest : FunctionTestBase<ExecutionCount.Function>
{
    // This test relies on it being the *only* test to use ExecutionCount.Function.
    // Without that, we might need some kind of reset capability.
    [Fact]
    public async Task TwoRequests()
    {
        string request1 = await ExecuteHttpGetRequestAsync();
        string request2 = await ExecuteHttpGetRequestAsync();

        Assert.Equal("Server execution count: 1", request1);
        Assert.Equal("Server execution count: 2", request2);
    }
}
