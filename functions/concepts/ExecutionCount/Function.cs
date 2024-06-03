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

// [START functions_concepts_stateless]
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExecutionCount;

public class Function : IHttpFunction
{
    // Note that this variable must be static, because a new instance is
    // created for each request. An alternative approach would be to use
    // dependency injection with a singleton resource injected into the function
    // constructor.
    private static int _count;

    public async Task HandleAsync(HttpContext context)
    {
        // Note: the total function invocation count across
        // all servers may not be equal to this value!
        int currentCount = Interlocked.Increment(ref _count);
        await context.Response.WriteAsync($"Server execution count: {currentCount}", context.RequestAborted);
    }
}
// [END functions_concepts_stateless]
